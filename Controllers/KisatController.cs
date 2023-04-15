using Kisa_Kuikka.Data;
using Kisa_Kuikka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kisa_Kuikka.Controllers
{
    [Authorize]
    [AllowAllAuthorized]
    [Route("[controller]")]
    public class KisatController : Controller
    {
        private readonly ApplicationDbContext _context;
        public KisatController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return _context.Kisa != null ?
                          View(await _context.Kisa.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Kisa'  is null.");
        }
    }
}
