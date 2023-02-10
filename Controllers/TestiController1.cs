using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kipa_plus.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class TestiController1 : Controller
    {
        public async Task<IActionResult> Index()
        {
            var str = "";
            foreach(var claim in User.Claims)
            {
                str+=  " " +claim.Type + ":" + claim.Value;
            }
            return Ok(str);  
        }
        
        
    }
}
