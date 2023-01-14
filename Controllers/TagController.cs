using Microsoft.AspNetCore.Mvc;

namespace Kipa_plus.Controllers
{
    public class TagController : Controller
    {
        public IActionResult Index(int? RastiId)
        {
            ViewBag.RastiId = RastiId;
            return View();
        }

        public IActionResult LueLahto(int? RastiId)
        {
            
            return View();
        }
    }
}
