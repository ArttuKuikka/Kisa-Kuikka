using Kipa_plus.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kipa_plus.Controllers
{
    public class HomeController : Controller
    {

        public async Task<IActionResult> Index()
        {
            return Redirect("/Kisat");
        }
    }
}
