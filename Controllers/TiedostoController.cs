using Kipa_plus.Data;
using Kipa_plus.Models;
using Kipaplus.Data.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;


namespace Kipa_plus.Controllers
{
    [Authorize]
    public class TiedostoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TiedostoController(ApplicationDbContext context) 
        {
            _context = context;
        }

        [Route("[controller]")]
        public IActionResult Index()
        {

            var tiedostot = _context.Tiedosto.ToList();
                
            if (tiedostot == null)
            {
                return NotFound();
            }
            

           
            return View(tiedostot);
        }
        public async Task<IActionResult> Post()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if(file == null)
            {
                return BadRequest();
            }
            try
            {
                if (file.Length > 0)
                {
                    if(file.Length > 60000000){
                        return Forbid("Liian iso tiedosto");
                    }
                    var path = Path.GetFullPath("/UploadedFiles");
                    if(!Path.Exists(path)) { return Problem(path + " Ei ole olemassa. kai mounttasit volumen"); }
                    string extension = file.FileName.Split('.').Last().ToString() ?? ".null";
                    string random = RandomString(15);
                    string filename = random + "." + extension;

                    MimeTypes.TryGetMimeType(filename, out var mimeType);
                    if(mimeType== null) { mimeType = "unknown"; }

                    var Ti = new Tiedosto() { Extension= extension, FileName = random, MimeType = mimeType };
                    _context.Add(Ti);
                    _context.SaveChanges();
                    

                    using (var fileStream = new FileStream(Path.Combine(path, random), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return Ok(Ti.Id);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var path = Path.GetFullPath("/UploadedFiles");
            if (!Path.Exists(path)) { return Problem(path + " Ei ole olemassa. kai mounttasit volumen"); }

            var Ti = await _context.FindAsync<Tiedosto>(id);
            if (Ti == null)
            {
                return NotFound();
            }


            try
            {
                System.IO.File.Delete(Path.Combine(path, Ti.FileName));
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public async Task<IActionResult> Get(int id)
        {
            
            var path = Path.GetFullPath("/UploadedFiles");
            if (!Path.Exists(path)) { return Problem(path + " Ei ole olemassa. kai mounttasit volumen"); }

            var Ti = await _context.FindAsync<Tiedosto>(id);
            if(Ti == null)
            {
                return NotFound();
            }

            
            var file = await System.IO.File.ReadAllBytesAsync(Path.Combine(path, Ti.FileName));
            return File(file, Ti.MimeType, fileDownloadName: Ti.FileName + "." + Ti.Extension);

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
