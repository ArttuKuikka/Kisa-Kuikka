using Kipa_plus.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

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

                //etsi joka rasti ja laske niiden kaikkien tehtävien formitemeiden määrä ja laita se rastin headerin pituudeksi
                IRow RastinNimetRow = sheet.CreateRow(0);
                var lastindex = 1;
                foreach (var rasti in _context.Rasti.Where(x => x.KisaId == Kisa.Id))
                {
                    var SarjanTehtävätrastisssa = _context.Tehtava.Where(x => x.SarjaId == sarja.Id).Where(x => x.RastiId == rasti.Id);
                    int rastinpituus = 0;
                    foreach(var item in SarjanTehtävätrastisssa)
                    {
                        if(item.TehtavaJson == null)
                        {
                            continue;
                        }
                        var tehtäväpohjat = JArray.Parse(item.TehtavaJson);

                        foreach(var tehtava in tehtäväpohjat)
                        {
                            rastinpituus++;
                        }
                    }

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, lastindex, lastindex + rastinpituus));
                    var RastinCell = RastinNimetRow.CreateCell(lastindex);
                    RastinCell.SetCellValue(rasti.Nimi);
                    RastinCell.CellStyle = centertext;

                    lastindex = lastindex + rastinpituus +1;


                }
                
                




            }

            using (var fs = new FileStream("test.xlsx", FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }


            return Ok("OK");
        }

        
    }
}
