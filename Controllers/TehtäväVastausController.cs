using Kipa_plus.Data;
using Kipa_plus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kipa_plus.Controllers
{
    public class TehtäväVastausController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TehtäväVastausController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("[controller]")]
        public async Task<IActionResult> Create([Bind("Id,SarjaId,KisaId,RastiId,TehtäväId,Kesken,TehtavaJson")] TehtäväVastaus tehtäväVastaus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tehtäväVastaus);
                await _context.SaveChangesAsync();
                return Redirect("/Kisa/" + tehtäväVastaus.KisaId + "/Vartiot");
            }
            return View(tehtäväVastaus);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
