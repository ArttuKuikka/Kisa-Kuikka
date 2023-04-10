using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kipa_plus.Data;
using Kipa_plus.Models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using Kipa_plus.Models.DynamicAuth;
using Kipa_plus.Models.ViewModels;
using Kipaplus.Data.Migrations;
using Microsoft.AspNetCore.Identity;
using Kipa_plus.Services;
using System.Reflection.Metadata.Ecma335;

namespace Kipa_plus.Controllers
{
    [Route("[controller]")]
    [Static]
    [Authorize]
    public class KisaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoleAccessStore _roleAccessStore;
        private readonly DynamicAuthorizationOptions _authorizationOptions;
        private readonly IilmoitusService _IlmoitusService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public KisaController(ApplicationDbContext context, IRoleAccessStore roleAccessStore, DynamicAuthorizationOptions authorizationOptions, IilmoitusService ilmoitusService, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleAccessStore = roleAccessStore;
            _authorizationOptions = authorizationOptions;
            _IlmoitusService = ilmoitusService;
            _roleManager = roleManager;
        }

        [HttpGet("{kisaId:int}/Tilannevaltuudet")]
        [DisplayName("Oikeus valtuuksia tarviviin rasti-tilanteisiin")]
        public IActionResult TilanneValtuudet(int RastiId)
        {
            return NotFound(); //tässä vain että rooli näkyisi, roolia käytetään RastiController tilan muokkauksessa katsomaan onko oikeudet vaihtaa valtuuksia tarvivaan tilanteeseen
        }




        [HttpGet("{kisaId:int}/LiittymisId")]
        [DisplayName("Näytä liittymisID")]
        public async Task<IActionResult> LiittymisId(int kisaId)
        {
            if (kisaId == null || _context.Kisa == null)
            {
                return NotFound();
            }

            var kisa = await _context.Kisa.FindAsync(kisaId);

            if(kisa.LiittymisId == null)
            {
                kisa.LiittymisId = Guid.NewGuid().ToString();
                _context.Update(kisa);
                _context.SaveChanges();
            }
            (string, int) returnitem = (kisa.LiittymisId, kisaId);


            return View("LiittymisId",returnitem);
        }

        [HttpGet("{kisaId:int}/LiittymisIdUudelleenluonti")]
        [DisplayName("Uudelleenluo liittymisID")]
        public async Task<IActionResult> LiittymisIdUudelleenluonti(int kisaId)
        {
            if (kisaId == null || _context.Kisa == null)
            {
                return NotFound();
            }

            var kisa = await _context.Kisa.FindAsync(kisaId);

            kisa.LiittymisId = Guid.NewGuid().ToString();
            _context.Update(kisa);
            _context.SaveChanges();


            return Redirect($"/Kisa/{kisaId}/LiittymisId");
        }

        [HttpGet("{kisaId:int}/Lataukset")]
        [DisplayName("Latausvaihtoedot")]
        public async Task<IActionResult> Lataukset(int kisaId)
        {
            if (kisaId == null || _context.Kisa == null)
            {
                return NotFound();
            }


            return View(kisaId);
        }

        [DisplayName("Luo Rasti")]
        [HttpGet("{kisaId:int}/LuoRasti")]
        // GET: Rasti/Luo
        public async Task<IActionResult> LuoRasti(int kisaId)
        {
            if (!_context.Tilanne.Where(x => x.KisaId == kisaId).Any())
            {
                var oletustilanteet = new List<Tilanne>
                {
                    new Tilanne() { KisaId = kisaId, Nimi = "Rakentamatta", TarvitseeValtuudet = false },
                    new Tilanne() { KisaId = kisaId, Nimi = "Rakennettu", TarvitseeValtuudet = false },
                    new Tilanne() { KisaId = kisaId, Nimi = "Lupa purkaa", TarvitseeValtuudet = true },
                    new Tilanne() { KisaId = kisaId, Nimi = "Purettu", TarvitseeValtuudet = false }
                };
                foreach (var tilanne in oletustilanteet)
                {
                    _context.Tilanne.Add(tilanne);
                }
                await _context.SaveChangesAsync();
            }


            var tilanteet = _context.Tilanne.Where(x => x.KisaId == kisaId);

            return View(new LuoRastiViewModel() { KisaId = kisaId, Tilanteet = tilanteet, TarkistusKaytossa = true, VaadiKahdenKayttajanTarkistus = true });
        }

        // POST: Rasti/Luo
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("LuoRasti")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuoRasti([Bind("KisaId,Nimi,Numero,NykyinenTilanneId,VaadiKahdenKayttajanTarkistus,TarkistusKaytossa,tehtavaPaikat")] LuoRastiViewModel luoRastiViewModel)
        {
            luoRastiViewModel.Tilanteet = _context.Tilanne.Where(x => x.KisaId == luoRastiViewModel.KisaId);
            if (ModelState.IsValid)
            {
                if (_context.Rasti.Where(x => x.Nimi == luoRastiViewModel.Nimi).Where(x => x.KisaId == luoRastiViewModel.KisaId).Any())
                {
                    ViewBag.Error = "Rasti tällä nimellä on jo olemassa";
                    return View(luoRastiViewModel);
                }
                if (_context.Rasti.Where(x => x.Numero == luoRastiViewModel.Numero).Where(x => x.KisaId == luoRastiViewModel.KisaId).Any())
                {
                    ViewBag.NumeroError = "Rasti tällä numerolla on jo olemassa";
                    return View(luoRastiViewModel);
                }
                var rasti = new Rasti() { KisaId = luoRastiViewModel.KisaId,
                    Nimi = luoRastiViewModel.Nimi,
                    Numero = luoRastiViewModel.Numero,
                    tehtavaPaikat = luoRastiViewModel.tehtavaPaikat,
                    TilanneId = luoRastiViewModel.NykyinenTilanneId,
                    TarkistusKaytossa = luoRastiViewModel.TarkistusKaytossa,
                    VaadiKahdenKayttajanTarkistus = luoRastiViewModel.VaadiKahdenKayttajanTarkistus};
                _context.Add(rasti);
                await _context.SaveChangesAsync();
                return Redirect("/Kisa/" + luoRastiViewModel.KisaId + "/Rastit");
            }
            return View(luoRastiViewModel);
        }


        // GET: Rasti/Delete/5
        [HttpGet("{kisaId:int}/PoistaRasti")]
        [DisplayName("Poista rasti")]
        public async Task<IActionResult> PoistaRasti(int? RastiId)
        {
            if (RastiId == null || _context.Rasti == null)
            {
                return NotFound();
            }

            var rasti = await _context.Rasti
                .FirstOrDefaultAsync(m => m.Id == RastiId);
            if (rasti == null)
            {
                return NotFound();
            }

            return View(rasti);
        }

        // POST: Rasti/Delete/5
        [HttpPost("{kisaId:int}/PoistaRasti"), ActionName("PoistaRasti")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RastiDeleteConfirmed(Rasti viewModel)
        {
            if (_context.Rasti == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Rasti'  is null.");
            }
            var rasti = await _context.Rasti.FindAsync(viewModel.Id);

            if (rasti != null)
            {
                var rId = rasti.Id;
                var KisaId = rasti.KisaId;
                _context.Rasti.Remove(rasti);

                _context.Tehtava.Where(x => x.RastiId == rId).ToList().ForEach(x => _context.Tehtava.Remove(x));
                _context.TehtavaVastaus.Where(x => x.RastiId == rId).ToList().ForEach(x => _context.TehtavaVastaus.Remove(x));

                
                

                await _context.SaveChangesAsync();
                return Redirect("/Kisa/" + KisaId + "/Rastit");
            }

            return BadRequest();
        }

        private bool RastiExists(int? id)
        {
            return (_context.Rasti?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        // GET: Kisa
        [HttpGet("{kisaId:int}/")]
        [DisplayName("Etusivu")]
        public async Task<IActionResult> Index(int kisaId)
        {
            if(kisaId == 0)
            {
                return Redirect("/");
            }
            var viewModel = new KisaIndexViewModel();

            viewModel.Kisa = _context.Kisa
               .First(m => m.Id == kisaId);

            if (User.Identity.Name != _authorizationOptions.DefaultAdminUser)
            {
                var roles = await (
               from usr in _context.Users
               join userRole in _context.UserRoles on usr.Id equals userRole.UserId
               join role in _context.Roles on userRole.RoleId equals role.Id
               where usr.UserName == User.Identity.Name
               select role.Id.ToString()
           ).ToArrayAsync();

                var rastit = await _roleAccessStore.HasAccessToRastiIdsAsync(roles);

                if(rastit.Count == 1)
                {
                    viewModel.OikeusYhteenRastiin = true;

                    viewModel.OikeusRasti = _context.Rasti.First(x => x.Id == rastit.First());

                    viewModel.Tilanteet = _context.Tilanne.Where(x => x.KisaId == kisaId);
                }


            }
            
            return View(viewModel);
        }

        

        // GET: Kisa/Luo
        [HttpGet("Luo")]
        public IActionResult Luo()
        {
            return View();
        }

        // POST: Kisa/Luo
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Luo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Luo([Bind("Id,Nimi")] Kisa kisa)
        {
            kisa.JaaTagTilastot = false;
            kisa.NaytaIlmoitusSuositusEtusivulla = true;
            kisa.LahetaIlmoituksiaRastinTilanvaihdosta = true;
            if (ModelState.IsValid)
            {
                _context.Add(kisa);
               
                await _context.SaveChangesAsync();
                return Redirect("/");
            }
            return View(kisa);
        }

        // GET: Kisa/Edit/5
        [HttpGet("{kisaId:int}/Edit")]
        [DisplayName("Muokkaa")]
        public async Task<IActionResult> Edit(int kisaId)
        {
            if (kisaId == null || _context.Kisa == null)
            {
                return NotFound();
            }

            var kisa = await _context.Kisa.FindAsync(kisaId);
            if (kisa == null)
            {
                return NotFound();
            }
            return View(kisa);
        }

        // POST: Kisa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("{kisaId:int}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nimi,JaaTagTilastot,TilanneSeurantaKuvaURL,NaytaIlmoitusSuositusEtusivulla,LahetaIlmoituksiaRastinTilanvaihdosta")] Kisa kisa)
        {
            if (id != kisa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var olemassaolevakisa = await _context.Kisa.FindAsync(kisa.Id);
                    olemassaolevakisa.Nimi = kisa.Nimi;
                    olemassaolevakisa.JaaTagTilastot= kisa.JaaTagTilastot;
                    olemassaolevakisa.TilanneSeurantaKuvaURL = kisa.TilanneSeurantaKuvaURL;
                    olemassaolevakisa.NaytaIlmoitusSuositusEtusivulla = kisa.NaytaIlmoitusSuositusEtusivulla;
                    olemassaolevakisa.LahetaIlmoituksiaRastinTilanvaihdosta = kisa.LahetaIlmoituksiaRastinTilanvaihdosta;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KisaExists(kisa.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("/Kisa/" + kisa.Id.ToString());
            }
            return View(kisa);
        }

        // GET: Kisa/Delete/5
        [HttpGet("{kisaId:int}/Delete")]
        [DisplayName("Poista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Kisa == null)
            {
                return NotFound();
            }

            var kisa = await _context.Kisa
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kisa == null)
            {
                return NotFound();
            }

            return View(kisa);
        }

        // POST: Kisa/Delete/5
        [HttpPost("{kisaId:int}/Delete"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Kisa == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Kisa'  is null.");
            }
            var kisa = await _context.Kisa.FindAsync(id);
            if (kisa != null)
            {
                _context.Kisa.Remove(kisa);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KisaExists(int? id)
        {
          return (_context.Kisa?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet("{kisaId:int}/Sarjat")]
        [DisplayName("Listaa sarjat")]
        public async Task<IActionResult> Sarjat(int kisaId)
        {
            if (kisaId == 0 || _context.Sarja == null)
            {
                return NotFound();
            }

            var sarjat = _context.Sarja
                .Where(m => m.KisaId == kisaId);
            if (sarjat == null)
            {
                return NotFound();
            }
            ViewBag.KisaId = kisaId;
            return View(sarjat);
        }

        [HttpGet("{kisaId:int}/Vartiot")]
        [DisplayName("Listaa vartiot")]
        public async Task<IActionResult> Vartiot(int kisaId)
        {
            if (kisaId == 0 || _context.Vartio == null)
            {
                return NotFound();
            }

            var vartiot = _context.Vartio
                .Where(m => m.KisaId == kisaId);
            if (vartiot == null)
            {
                return NotFound();
            }
            

            var sarjat = _context.Sarja
                .Where(m => m.KisaId == kisaId);
            if (sarjat == null)
            {
                return NotFound();
            }

            var ViewModel = new VartiotViewModel() { Vartiot = vartiot, Sarjat = sarjat, KisaId = kisaId};

            return View(ViewModel);
        }

        [HttpGet("{kisaId:int}/Rastit")]
        [DisplayName("Listaa rastit")]
        public async Task<IActionResult> Rastit(int kisaId)
        {
            if (kisaId == 0 || _context.Rasti == null)
            {
                return NotFound();
            }

            var roles = await (
                from usr in _context.Users
                join userRole in _context.UserRoles on usr.Id equals userRole.UserId
                join role in _context.Roles on userRole.RoleId equals role.Id
                where usr.UserName == User.Identity.Name
                select role.Id.ToString()
            ).ToArrayAsync();

            var rastitjoihinoikeudet = await _roleAccessStore.HasAccessToRastiIdsAsync(roles);

            List<Rasti> rastit;

            if(User.Identity.Name == _authorizationOptions.DefaultAdminUser)
            {
                rastit = _context.Rasti
                .Where(m => m.KisaId == kisaId).ToList();
            }
            else
            {
                rastit = _context.Rasti
                .Where(m => m.KisaId == kisaId).Where(x => rastitjoihinoikeudet.Contains((int)x.Id)).ToList();
            }


            if (rastit == null)
            {
                return NotFound();
            }
            rastit.Sort((p1, p2) => p1.Numero.CompareTo(p2.Numero));
            

            var viewModel = new ListaaRastitViewModel() { KisaId= kisaId, Rastit = rastit, Tilanteet = _context.Tilanne };
            return View(viewModel);
        }

        [HttpGet("{kisaId:int}/LahetaIlmoitus")]
        [DisplayName("Lähetä ilmoituksia")]
        public async Task <IActionResult> LahetaIlmoitus(int kisaId)
        {
            var kisa = await _context.Kisa.FindAsync(kisaId);
            if(kisa != null)
            {
                var roles = _roleManager.Roles.ToList(); //tunnistus sile että on vain kisan roolit sitten kun monen kisan tuki on lisätty

                var checkboxlista = new List<CheckboxViewModel>();
                foreach (var role in roles)
                {
                    checkboxlista.Add(new CheckboxViewModel() { Id = role.Id, DisplayName = role.Name ?? role.Id, IsChecked = false });
                }

                var checkboxlista2 = new List<CheckboxViewModel>();
                foreach (var rasti in _context.Rasti.Where(x => x.KisaId == kisaId))
                {
                    checkboxlista2.Add(new CheckboxViewModel() { Id = rasti.Id.ToString(), DisplayName = rasti.NumeroJaNimi, IsChecked = false });
                }
                checkboxlista2.Sort((p1, p2) => p1.DisplayName.CompareTo(p2.DisplayName));
                var viewModel = new SendPushViewModel() { Roles = checkboxlista, Rastit = checkboxlista2 };
                return View(viewModel);
            }
            return BadRequest();
           
        }

        [HttpPost("{kisaId:int}/LahetaIlmoitus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LahetaIlmoitus([Bind("message,title,refUrl,Roles,Rastit")] SendPushViewModel viewModel)
        {
            //TODO: varmistus että ei voi lähettää ilmoituksia toisen kisan rooleihin
            if (ModelState.IsValid)
            {
                var roleIdList = new List<string>();
                var checklist = viewModel.Roles?.Where(x => x.IsChecked == true).ToList();
                var checklis2 = viewModel.Rastit?.Where(x => x.IsChecked == true).ToList();

                if (checklis2 != null)
                {
                    var rastidilist = new List<int>();
                    foreach(var rasti in checklis2)
                    {
                        rastidilist.Add(int.Parse(rasti.Id));
                    }
                    roleIdList.AddRange(await _IlmoitusService.GetRoleIdsFromRastiIds(rastidilist.ToArray()));
                }

                if (checklist != null)
                {
                    

                    foreach (var role in checklist)
                    {
                        roleIdList.Add(role.Id.ToString());
                    }

                    
                }
                if(roleIdList != null)
                {
                    var succesRate = await _IlmoitusService.SendNotifToRoleIdsAsync(roleIdList.ToArray(), viewModel.title, viewModel.message, viewModel.refUrl);

                    ViewBag.Message = "WebPush ilmoitus lähetetty onnistuneesti " + succesRate.ToString() + " käyttäjälle ja normaali ilmoitus lähetetty kaikille";
                    var roles = _roleManager.Roles.ToList(); //tunnistus sile että on vain kisan roolit sitten kun monen kisan tuki on lisätty

                    return View(viewModel);
                }
            }


            return BadRequest();
        }
    }
}
