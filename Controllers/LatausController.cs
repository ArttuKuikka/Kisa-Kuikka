﻿using Kisa_Kuikka.Data;
using Kisa_Kuikka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Kisa_Kuikka.Controllers
{
    [Authorize]
    [Static]
    [Route("[controller]")]
    [DisplayName("Kisan tietojen lataus")]
    public class LatausController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LatausController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("/Lataus")]
        [DisplayName("Lataa excel tiedosto")]
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

            //etsi kisan sarjat ja luo jokaiselle oma sivu työkirjassa
            foreach (var sarja in Sarjat)
            {
                ISheet sheet = workbook.CreateSheet(sarja.Nimi);
                var kokosheetpituus = 1;

                var sarjanvartiot = _context.Vartio.Where(x => x.SarjaId == sarja.Id).OrderBy(x => x.Numero).ToList();

                //aseta vartioiden nimet
                var vartionnimiindex = 3;
                var rowlist = new List<IRow>();
                foreach(var vartio in sarjanvartiot)
                {
                    IRow vartioRow = sheet.CreateRow(vartionnimiindex);
                    var nimicell = vartioRow.CreateCell(0);
                    nimicell.SetCellValue(vartio.NumeroJaNimi);

                    rowlist.Add(vartioRow);

                    vartionnimiindex++;
                }

                //etsi joka rasti ja laske niiden kaikkien tehtävien formitemeiden määrä ja laita se rastin headerin pituudeksi
                IRow RastinNimetRow = sheet.CreateRow(0);
                IRow RastinTehtävätRow = sheet.CreateRow(1);
                IRow FormItemRow = sheet.CreateRow(2); //rivi jossa jokaisen asian joka on lisäty form builderissa, nimi
                var lastindex = 1;
                var RastinTehtävätRowlastindex = 1;
                var FormItemRowlastindex = 1;

                //etsi joka rasti kisasta
                var rastilista = _context.Rasti.Where(x => x.KisaId == Kisa.Id).OrderBy(x => x.Numero).ToList();
                foreach (var rasti in rastilista)
                {
                    //hae kaikki rastin tehtäväpohjat
                    var SarjanTehtävätrastisssa = _context.Tehtava.Where(x => x.SarjaId == sarja.Id).Where(x => x.RastiId == rasti.Id).ToList();
                    int rastinpituus = 0;

                    
                    foreach (var item in SarjanTehtävätrastisssa)
                    {
                        if(item.TehtavaJson == null)
                        {
                            continue;
                        }
                        var TehtäväNimiHeaderPituus = 0;

                        //tehtäväpohjan json tiedosto(ei sisällä vastauksia)
                        var tehtäväpohjat = JArray.Parse(item.TehtavaJson);

                        //käy läpi jokainen form item lisäten niiden pitudet otsikoita varten 
                        var tehtindex = 0;
                        foreach (var tehtava in tehtäväpohjat)
                        {
                            bool lisääLataukseen = true;
                            var latausFlag = tehtava["lataa"];
                            if (latausFlag != null)
                            {
                                var val = latausFlag.Value<string>();
                                if(val == "False")
                                {
                                    lisääLataukseen = false;
                                }
                            }
                            if (lisääLataukseen)
                            {
                                //lisää pituudet
                                rastinpituus++;
                                TehtäväNimiHeaderPituus++;
                                kokosheetpituus++;

                                //aseta userData joka vartiolle
                                var vartiovastausrowindex = 0;
                                foreach (var vartio in sarjanvartiot)
                                {
                                    var tehtvastaus = _context.TehtavaVastaus.Where(x => x.SarjaId == sarja.Id).Where(x => x.RastiId == rasti.Id).Where(x => x.TehtavaId == item.Id).Where(x => x.VartioId == vartio.Id).Where(x => x.Tarkistettu == true).FirstOrDefault();
                                    if (tehtvastaus != null)
                                    {
                                        var formitem = JArray.Parse(tehtvastaus.TehtavaJson)[tehtindex];

                                        var userData = formitem["userData"];
                                        if (userData != null && formitem != null)
                                        {
                                            var data0 = userData[0];


                                            var vastausrow = rowlist[vartiovastausrowindex]; //ota vartion oikea row
                                            var vastauscell = vastausrow.CreateCell(FormItemRowlastindex);


                                            if (data0 != null && data0.ToString() != "")
                                            {
                                                switch (formitem["type"].ToString())
                                                {
                                                    case "currentTime":
                                                        if (DateTime.TryParse(data0.ToString(), out var time))
                                                        {
                                                            vastauscell.SetCellValue(time.ToLocalTime());
                                                        }
                                                        else
                                                        {
                                                            vastauscell.SetCellValue(data0.ToString());
                                                        }
                                                        break;
                                                    case "fileUpload":
                                                        vastauscell.SetCellValue("https://" + Request.Host + "/Tiedosto/Get?id=" + data0.ToString());
                                                        break;
                                                    case "number":

                                                        if (double.TryParse(data0.ToString(), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed))
                                                        {
                                                            vastauscell.SetCellValue(parsed);
                                                        }
                                                        else
                                                        {
                                                            vastauscell.SetCellValue(data0.ToString());
                                                        }

                                                        break;
                                                    default:
                                                        vastauscell.SetCellValue(data0.ToString());
                                                        break;

                                                }
                                            }
                                        }
                                    }

                                    vartiovastausrowindex++;
                                }

                                //luo cell missä lukee form item nimi
                                var formitemname = FormItemRow.CreateCell(FormItemRowlastindex);

                                var label = tehtava["label"];
                                if (label != null)
                                {
                                    formitemname.SetCellValue(label.ToString());
                                }
                                

                                FormItemRowlastindex++;
                            }
                            tehtindex++;
                        }

                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, RastinTehtävätRowlastindex, RastinTehtävätRowlastindex + TehtäväNimiHeaderPituus - 1));
                        var TehtäväNimiCell = RastinTehtävätRow.CreateCell(RastinTehtävätRowlastindex);
                        TehtäväNimiCell.SetCellValue(item.Nimi);
                        TehtäväNimiCell.CellStyle = centertext;

                        RastinTehtävätRowlastindex = RastinTehtävätRowlastindex + TehtäväNimiHeaderPituus;

                    }

                    //lisää rasti vain jossa siinä on 1 tai enemmän tehtävää
                    if(SarjanTehtävätrastisssa.Count >= 1)
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, lastindex, lastindex + rastinpituus - 1));
                        var RastinCell = RastinNimetRow.CreateCell(lastindex);
                        RastinCell.SetCellValue(rasti.NumeroJaNimi);
                        RastinCell.CellStyle = centertext;

                        lastindex = lastindex + rastinpituus;
                    }


                }

               

                //vaihda joka columnin leveyttä isommaksi
                for (int i = 0; i < kokosheetpituus; i++)
                {
                    sheet.SetColumnWidth(i, 20 * 256);
                }

            }


            var xssfworkbook = workbook as NPOI.XSSF.UserModel.XSSFWorkbook;
            var properties = xssfworkbook.GetProperties();
            var coreProperties = properties.CoreProperties;
            coreProperties.Creator = "Kisa-Kuikka";
            
            using (var fs = new FileStream("output.xlsx", FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            var aika = DateTime.Now.ToString("HH.mm");
            return File(System.IO.File.ReadAllBytes("output.xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{Kisa.Nimi}-{aika}-Export.xlsx");



        }

       


    }
}
