using Kipa_plus.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Kipa_plus.Controllers
{
    [Route("[controller]")]
    public class LatausController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LatausController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> index(int? kisaid, string? format)
        {
            if (kisaid == null) { return BadRequest("KisaId null"); }
            if (format == null) { return BadRequest("Format null"); }
            if (format != "xlsx") { return BadRequest("väärä format"); }

            var Kisa = await _context.Kisa.FindAsync(kisaid);

            if (Kisa == null) { return NotFound("Ei kisaa"); }

            var Sarjat = _context.Sarja.Where(x => x.KisaId == Kisa.Id);

            if(Sarjat == null) { return NotFound("Ei sarjoja"); }

            IWorkbook workbook = new XSSFWorkbook();

            ICellStyle centertext = workbook.CreateCellStyle();
            centertext.Alignment = HorizontalAlignment.Center;

            foreach (var sarja in Sarjat)
            {
                ISheet sheet = workbook.CreateSheet(sarja.Nimi);
                var kokosheetpituus = 1;

                //etsi joka rasti ja laske niiden kaikkien tehtävien formitemeiden määrä ja laita se rastin headerin pituudeksi
                IRow RastinNimetRow = sheet.CreateRow(0);
                IRow RastinTehtävätRow = sheet.CreateRow(1);
                IRow FormItemRow = sheet.CreateRow(2);
                var lastindex = 1;
                var RastinTehtävätRowlastindex = 1;
                var FormItemRowlastindex = 1;
                foreach (var rasti in _context.Rasti.Where(x => x.KisaId == Kisa.Id))
                {
                    var SarjanTehtävätrastisssa = _context.Tehtava.Where(x => x.SarjaId == sarja.Id).Where(x => x.RastiId == rasti.Id).ToList();
                    int rastinpituus = 0;

                    
                    foreach (var item in SarjanTehtävätrastisssa)
                    {
                        if(item.TehtavaJson == null)
                        {
                            continue;
                        }
                        var TehtäväNimiHeaderPituus = 0;


                        var tehtäväpohjat = JArray.Parse(item.TehtavaJson); 

                        foreach(var tehtava in tehtäväpohjat)
                        {
                            rastinpituus++;
                            TehtäväNimiHeaderPituus++;
                            kokosheetpituus++;

                            var formitemname = FormItemRow.CreateCell(FormItemRowlastindex);

                            var label = tehtava["label"];
                            if (label != null)
                            {
                                formitemname.SetCellValue(label.ToString());
                            }
                            
                            
                            FormItemRowlastindex++;
                        }

                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, RastinTehtävätRowlastindex, RastinTehtävätRowlastindex + TehtäväNimiHeaderPituus - 1));
                        var TehtäväNimiCell = RastinTehtävätRow.CreateCell(RastinTehtävätRowlastindex);
                        TehtäväNimiCell.SetCellValue(item.Nimi);
                        TehtäväNimiCell.CellStyle = centertext;

                        RastinTehtävätRowlastindex = RastinTehtävätRowlastindex + TehtäväNimiHeaderPituus;

                    }

                    if(SarjanTehtävätrastisssa.Count >= 1)
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, lastindex, lastindex + rastinpituus - 1));
                        var RastinCell = RastinNimetRow.CreateCell(lastindex);
                        RastinCell.SetCellValue(rasti.Nimi);
                        RastinCell.CellStyle = centertext;

                        lastindex = lastindex + rastinpituus;
                    }


                }

                //aseta kaikki userData riveiihin
                var VartioDataRowlastindex = 3;
                foreach(var vartio in _context.Vartio.Where(x => x.SarjaId == sarja.Id))
                {
                    IRow VartioDataRow = sheet.CreateRow(VartioDataRowlastindex);
                    VartioDataRow.CreateCell(0).SetCellValue(vartio.Numero.ToString() + " " + vartio.Nimi);
                    var rowindex = 1;
                    foreach (var vastaus in _context.TehtavaVastaus.Where(x => x.VartioId == vartio.Id))
                    {
                        if(vastaus.TehtavaJson == null)
                        {
                            continue;
                        }
                        foreach(var item in JArray.Parse(vastaus.TehtavaJson))
                        {
                            var cell = VartioDataRow.CreateCell(rowindex);
                            var data = item["userData"];
                            if(data != null)
                            {
                                var data0 = data[0];
                                if (data0 != null)
                                {
                                    switch (item["type"].ToString()){
                                        case "currentTime":
                                            cell.SetCellValue(DateTime.Parse(data0.ToString()));
                                            break;
                                        case "fileUpload":
                                            cell.SetCellValue("https://" + Request.Host + "/Tiedosto/Get?id=" + data0.ToString());
                                            break;
                                        default:
                                            cell.SetCellValue(data0.ToString());
                                            break;

                                    }
                                      
                                    
                                }
                            }
                            

                            rowindex++;
                        }
                    }

                    VartioDataRowlastindex++;
                }

                for (int i = 0; i < kokosheetpituus; i++)
                {
                    sheet.SetColumnWidth(i, 20 * 256);
                }

            }

            

            using (var fs = new FileStream("output.xlsx", FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }


            return File(System.IO.File.ReadAllBytes("output.xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Kisa.Nimi + "-Export.xlsx");









        }

        
    }
}
