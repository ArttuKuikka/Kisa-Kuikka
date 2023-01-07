using Kipa_plus.Data;
using Kipa_plus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kipa_plus.Controllers
{
    public class TehtavaVastausController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TehtavaVastausController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("[controller]")]
        public async Task<IActionResult> Create([Bind("Id,SarjaId,KisaId,RastiId,TehtavaId,Kesken,TehtavaJson")] TehtavaVastaus TehtavaVastaus)
        {
            if (ModelState.IsValid) 
            {
                _context.Add(TehtavaVastaus);
                await _context.SaveChangesAsync();
                return Redirect("/Kisa/" + TehtavaVastaus.KisaId + "/Vartiot");
            }
            return View(TehtavaVastaus);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
