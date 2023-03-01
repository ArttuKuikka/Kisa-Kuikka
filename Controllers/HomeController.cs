using Kipa_plus.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kipa_plus.Models;
using Microsoft.AspNetCore.Identity;

namespace Kipa_plus.Controllers
{
    [Authorize]
    [AllowAllAuthorized]
    
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(UserManager<IdentityUser> userManager) 
        {
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Index() //redirectaa käyttäjän oikeaan paikka
        {
           
            if(User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null) 
                    {
                        var claims = await _userManager.GetClaimsAsync(user);
                        var kisaclaims = claims.Where(x => x.Type == "OikeusKisaan");
                        if (kisaclaims != null)
                        {
                            if(kisaclaims.Count() == 1)
                            {
                                return Redirect("/Kisa/" + kisaclaims.FirstOrDefault()?.Value);
                            }
                        }
                    }
                   
                }
            }
            return Redirect("/Kisat");
        }

        
    }
}
