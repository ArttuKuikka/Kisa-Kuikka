using Kipa_plus.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kipa_plus.Controllers
{
    public class HomeController : Controller //home controller näyttä käytejellä oman kotisivun jossa painikkeet kaikkin käyttäjän roolille hyödyllisiin ominaisuukisiin ja tarvittaessa redirectaa kisa näkymään
    {
        
        public async Task<IActionResult> Index()
        {
            
            if(User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    
                   
                }
            }
            return Redirect("/Kisat");
        }
    }
}
