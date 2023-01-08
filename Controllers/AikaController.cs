using Microsoft.AspNetCore.Mvc;

namespace Kipa_plus.Controllers
{
    public class AikaController : Controller
    {
        [Route("[controller]")]
        public IActionResult Index()
        {
            return Json(new { aika = DateTime.Now });
        }
    }
}
