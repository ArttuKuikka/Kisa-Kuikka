using Kipa_plus.Data;
using Kipa_plus.Models;
using Kipa_plus.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Host;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
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
        public IlmoitusController(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager) 
        {
            _context= applicationDbContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            if(_context.VapidStore?.Count() == 0)
            {
                VapidDetails vapidKeys = VapidHelper.GenerateVapidKeys();
                _context.VapidStore.Add(new Models.VapidDetailsWithId() { Expiration = vapidKeys.Expiration, PrivateKey = vapidKeys.PrivateKey, PublicKey = vapidKeys.PublicKey, Subject = vapidKeys.Subject});
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

                
                
                return Ok(JArray.FromObject(await _userManager.GetClaimsAsync(user)));
            }
            
            return BadRequest();
        }

        public IActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send([Bind("message")] SendPushViewModel viewModel)
        {
            var userlist = _userManager.Users.ToList();
            int proo = 0;

            foreach(var user in userlist) 
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var endpoint = claims.FirstOrDefault(x => x.Type == "WebPush_endpoint");
                var p256dh = claims.FirstOrDefault(x => x.Type == "WebPush_p256dh");
                var auth = claims.FirstOrDefault(x => x.Type == "WebPush_auth");

                if(endpoint?.Value != null && p256dh?.Value != null && auth?.Value != null)
                {
                    var subscription = new PushSubscription(endpoint.Value, p256dh.Value, auth.Value);
                    var keys = _context.VapidStore?.FirstOrDefault();

                    var vapidDetails = new VapidDetails(keys?.Subject, keys?.PublicKey, keys?.PrivateKey);
                    var webPushClient = new WebPushClient();
                    try
                    {
                        webPushClient.SendNotification(subscription, viewModel.message ?? "NULL", vapidDetails);
                        proo++;
                    }
                    catch (Exception exception)
                    {
                        // Log error
                    }
                    
                }
            }
            return Ok("Ilmoitus lähetty " + proo.ToString() + " käyttäjälle");


            
        }
    }
}
