using Kipa_plus.Data;
using Microsoft.AspNetCore.Mvc;

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
    }
}
