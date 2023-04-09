using Kipa_plus.Data;
using Kipa_plus.Models;
using Kipa_plus.Models.ViewModels;
using Kipa_plus.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Host;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using System.Security.Cryptography.X509Certificates;
using WebPush;

namespace Kipa_plus.Controllers
{
    [Authorize]
    [AllowAllAuthorized]

    public class IlmoitusController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IilmoitusService _IlmoitusService;
        public IlmoitusController(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager, IilmoitusService ilmoitusService) 
        {
            _context= applicationDbContext;
            _userManager = userManager;
            _IlmoitusService = ilmoitusService;
        }
        
        public async Task <IActionResult> Index()
        {
            if(_context.VapidStore?.Count() == 0)
            {
                VapidDetails vapidKeys = VapidHelper.GenerateVapidKeys();
                var subject = Request.Host.ToString();
                _context.VapidStore.Add(new Models.VapidDetailsWithId() { Expiration = vapidKeys.Expiration, PrivateKey = vapidKeys.PrivateKey, PublicKey = vapidKeys.PublicKey, Subject = subject});
                _context.SaveChanges();
            }


            var keys = _context.VapidStore?.FirstOrDefault();
            ViewBag.applicationServerKey = keys?.PublicKey;
            return View();
            
        }

        [HttpPost]
        
        public async Task<IActionResult> Index(string endpoint, string p256dh, string auth)
        {
            
            var user = await _userManager.GetUserAsync(User);
            if(user != null) 
            {
                var claims = await _userManager.GetClaimsAsync(user);
                if(claims.FirstOrDefault(x => x.Type == "WebPush_endpoint") != null)
                {
                    await _userManager.ReplaceClaimAsync(user, claims.First(x => x.Type == "WebPush_endpoint"), new System.Security.Claims.Claim("WebPush_endpoint", endpoint));
                    await _userManager.ReplaceClaimAsync(user, claims.First(x => x.Type == "WebPush_p256dh"), new System.Security.Claims.Claim("WebPush_p256dh", p256dh));
                    await _userManager.ReplaceClaimAsync(user, claims.First(x => x.Type == "WebPush_auth"), new System.Security.Claims.Claim("WebPush_auth", auth));
                }
                else
                {
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("WebPush_endpoint", endpoint));
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("WebPush_p256dh", p256dh));
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("WebPush_auth", auth));
                }

                
                
                return Redirect("/");
            }
            
            return BadRequest();
        }

    }
}
