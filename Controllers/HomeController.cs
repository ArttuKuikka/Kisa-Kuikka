using Kipa_plus.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kipa_plus.Models;

namespace Kipa_plus.Controllers
{
    [Authorize]
    [AllowAllAuthorized]
    
    public class HomeController : Controller //home controller näyttä käytejellä oman kotisivun jossa painikkeet kaikkin käyttäjän roolille hyödyllisiin ominaisuukisiin ja tarvittaessa redirectaa kisa näkymään
    {
        
        public async Task<IActionResult> Index() //redirectaa käyttäjän oikeaan paikka
        {
            //if user has acces to 1 or more or 0 kisat redirectaa /kisat
            if(User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    
                   
                }
            }
            return Redirect("/Kisat");
        }

        public async Task<IActionResult> Koti()
        {

            if (User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {


                }
            }
            return BadRequest();
        }
    }
}
