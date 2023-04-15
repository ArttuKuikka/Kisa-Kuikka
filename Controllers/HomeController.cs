using Kisa_Kuikka.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kisa_Kuikka.Models;
using Microsoft.AspNetCore.Identity;
using Kisa_Kuikka.Models.DynamicAuth;

namespace Kisa_Kuikka.Controllers
{
    [Authorize]
    [AllowAllAuthorized]
    [Route("[controller]")]
    
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRoleAccessStore _roleAccessStore;
        private readonly ApplicationDbContext _dbContext;
        public HomeController(UserManager<IdentityUser> userManager, IRoleAccessStore roleAccessStore, ApplicationDbContext dbContext) 
        {
            _userManager = userManager;
            _roleAccessStore = roleAccessStore;
            _dbContext = dbContext;
        }

        [HttpGet("/")]
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
                                var roles = await (
                from usr in _dbContext.Users
                join userRole in _dbContext.UserRoles on usr.Id equals userRole.UserId
                join role in _dbContext.Roles on userRole.RoleId equals role.Id
                where usr.UserName == user.UserName
                select role.Id.ToString()
            ).ToArrayAsync();
                                var kisaid = int.Parse(kisaclaims.First().Value);
                                if (await _roleAccessStore.HasAccessToActionAsync(":Kisa:Index", roles)) //vaihda että tarkistaa KisaId sit ku auth tukee montaa kisaa
                                {
                                    return Redirect("/Kisa/" + kisaid.ToString());
                                }
                                else
                                {
                                    return Redirect("/Tervetuloa");
                                }
                                
                            }
                        }
                    }
                   
                }
            }
            return Redirect("/Kisat");
        }
        [HttpGet("/Tervetuloa")]
        public IActionResult Tervetuloa()
        {
            return View();
        }

        
    }
}
