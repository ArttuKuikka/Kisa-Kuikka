using Kipa_plus.Data;
using Kipa_plus.Models.DynamicAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Newtonsoft.Json.Linq;
using WebPush;

namespace Kipa_plus.Services
{
    public class IlmoitusService: IilmoitusService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleAccessStore _roleAccessStore;
        public IlmoitusService(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager, IRoleAccessStore roleAccessStore, RoleManager<IdentityRole> roleManager)
        {
            _context = applicationDbContext;
            _userManager = userManager;
            _roleAccessStore = roleAccessStore;
            _roleManager = roleManager; 
        }

        /// <summary>
        /// Lähetä ilmoitus kaikille käyttäjille joilla on oikeus tietyille rasteille
        /// </summary>
        /// <param name="RastiIds">Lista kaikista Rasti idistä joiden käyttäjille haluat lähettää ilmoituksen</param>
        /// <param name="title">Otsikko</param>
        /// <param name="message">Viesti</param>
        /// <param name="refurl">URL johon selain vie kun ilmoitusta painetaan</param>
        /// <returns>
        /// Numeron kuinka monelle käyttäjälle webPush ilmoitus lähetettiin onnistuneesti
        /// </returns>
        public async Task<int> SendNotifToRastiIdsAsync(int[]? RastiIds, string? title, string? message, string? refurl)
        {
            var roles = new List<string>();
            var rastiIdLista = RastiIds?.ToList();
            foreach (var role in _roleManager.Roles) 
            {
                var access = await _roleAccessStore.GetRoleAccessAsync(role.Id);
                foreach(var rooli in access.RastiAccess)
                {
                   if(rooli.RastiId != null)
                    {
                        if (rastiIdLista?.Contains((int)rooli.RastiId) ?? false)
                        {
                            roles.Add(role.Id);
                        }
                    }
                }
            }
           
            return await SendNotifToRoleIdsAsync(roles, title, message, refurl);
        }


        /// <summary>
        /// Lähetä ilmoitus kaikille käyttäjille jotka ovat tietyissä rooleissa
        /// </summary>
        /// <param name="RoleIds">Lista kaikista Rooli idistä joiden käyttäjille haluat lähettää ilmoituksen</param>
        /// <param name="title">Otsikko</param>
        /// <param name="message">Viesti</param>
        /// <param name="refurl">URL johon selain vie kun ilmoitusta painetaan</param>
        /// <returns>
        /// Numeron kuinka monelle käyttäjälle webPush ilmoitus lähetettiin onnistuneesti
        /// </returns>
        public async Task<int> SendNotifToRoleIdsAsync(string[]? RoleIds, string? title, string? message, string? refurl)
        {
            var users = new List<IdentityUser>();
           if(RoleIds != null)
            {
                //listaa kaikki roolit ja listaa niistä käyttäjät
                foreach (var roleId in RoleIds)
                {
                    var role = _roleManager.Roles.FirstOrDefault(x => x.Id == roleId);
                    if(role != null)
                    {
                        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                        foreach(var user in usersInRole)
                        {
                            if (!users.Contains(user))
                            {
                                users.Add(user);
                            }
                            
                        }
                    }
                }

                if (users.Any())
                {
                    var count = 0;
                    //lähetä notif kaikille käyttäjille
                    foreach(var user in users)
                    {
                        if (await SendNotifToUser(user, title, message, refurl))
                        {
                            count++;
                        }
                    }
                    return count;
                }
            }
            return 0;
        }



        /// <summary>
        /// Lähetä ilmoitus tietylle käyttäjälle
        /// </summary>
        /// <param name="user">IdentityUser käyttäjä</param>
        /// <param name="title">Otsikko</param>
        /// <param name="message">Viesti</param>
        /// <param name="refurl">URL johon selain vie kun ilmoitusta painetaan</param>
        /// <returns>True jos webpush ilmoituksen lähetys onnistui, false jos ei</returns>
        public async Task<bool> SendNotifToUser(IdentityUser user, string? title, string? message, string? refurl)
        {
            title = title ?? "Kipa-plus ilmoitus";
            message = message ?? "<Ei sisältöä>";
            refurl = refurl ?? "/";

            //luo ilmoitus omaan ilmoitus järjestelmään
            _context.Ilmoitukset.Add(new Models.Ilmoitus() {CreatedAt = DateTime.Now, Luettu = false, Message = message, Title = title, RefUrl = refurl, User = user });
            await _context.SaveChangesAsync();

            //webPush
            var claims = await _userManager.GetClaimsAsync(user);
            var endpoint = claims.FirstOrDefault(x => x.Type == "WebPush_endpoint");
            var p256dh = claims.FirstOrDefault(x => x.Type == "WebPush_p256dh");
            var auth = claims.FirstOrDefault(x => x.Type == "WebPush_auth");

            if (endpoint?.Value != null && p256dh?.Value != null && auth?.Value != null)
            {
                var subscription = new PushSubscription(endpoint.Value, p256dh.Value, auth.Value);
                var keys = _context.VapidStore?.FirstOrDefault();

                var payloadobject = new { title = title, message = message, refurl = refurl };
                var payload = JObject.FromObject(payloadobject);

                var vapidDetails = new VapidDetails(keys?.Subject, keys?.PublicKey, keys?.PrivateKey);
                var webPushClient = new WebPushClient();
                try
                {
                    webPushClient.SendNotification(subscription, payload.ToString(), vapidDetails);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }

            return false;
        }
    }

    

}
