using Microsoft.AspNetCore.Mvc;

namespace Kipa_plus.Controllers
{
    [Route("[controller]")]
    public class TestiController1 : Controller
    {
        public async Task<IActionResult> Index()
        {
            return Ok("K");  
        }
        [HttpGet("Testi")]
        public async Task<IActionResult> Testi()
        {
            return Ok("KK");  
        }

        

        [HttpGet("{kisaId:int}/Vartiot/Details")]
        public async Task<IActionResult> Vartio(int kisaId, [FromQuery] int id)
        {
            return Ok(kisaId.ToString() + " " + id.ToString());
        }
    }
}
