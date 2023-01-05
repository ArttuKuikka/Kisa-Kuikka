using Microsoft.AspNetCore.Mvc;
using System;

namespace Kipa_plus.Controllers
{
    public class TiedostoController : Controller
    {
        [Route("[controller]")]
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            
            try
            {
                if (file.Length > 0)
                {
                    var path = Path.GetFullPath("/UploadedFiles");
                    if(!Path.Exists(path)) { return Problem(path + " Ei ole olemassa. kai mounttasit volumen"); }
                    string extension = file.Name.Split('.').Last().ToString() ?? ".null";
                    string filename = RandomString(15) + extension;

                    using (var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return Ok("File Uploaded");
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        public async Task<IActionResult> Get(int id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var path = Path.GetFullPath("/UploadedFiles");
            if (!Path.Exists(path)) { return Problem(path + " Ei ole olemassa. kai mounttasit volumen"); }
            var filename = Directory.GetFiles(path).First();
            var file = await System.IO.File.ReadAllBytesAsync(filename);
            return File(file, "text/plain");

        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
