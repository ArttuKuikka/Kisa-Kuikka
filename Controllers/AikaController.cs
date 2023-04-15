using Microsoft.AspNetCore.Mvc;

namespace Kisa_Kuikka.Controllers
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
