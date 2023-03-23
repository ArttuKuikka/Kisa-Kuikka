﻿using System;
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

        public KisaController(ApplicationDbContext context, IRoleAccessStore roleAccessStore, DynamicAuthorizationOptions authorizationOptions)
        {
            _context = context;
            _roleAccessStore = roleAccessStore;
            _authorizationOptions = authorizationOptions;
        }

        [HttpGet("{kisaId:int}/HyvaksyTilanne")]
        [DisplayName("Hyväksy rastin tilanne muutokset")]
        public async Task<IActionResult> HyvaksyTilanne(int RastiId, int kielto)
        {
            var rasti = await _context.Rasti.FindAsync(RastiId);
            if(rasti != null)
            {
               if(kielto == 0)
                {
                    rasti.OdottaaTilanneHyvaksyntaa = false;
                    _context.Rasti.Update(rasti);
                    await _context.SaveChangesAsync();
                    return Redirect($"/Kisa/{rasti.KisaId}/Rastit");
                }
                else
                {
                    rasti.nykyinenTilanneId = (int)rasti.edellinenTilanneId;
                    rasti.OdottaaTilanneHyvaksyntaa = false;
                    _context.Rasti.Update(rasti);
                    await _context.SaveChangesAsync();
                    return Redirect($"/Kisa/{rasti.KisaId}/Rastit");
                }
            }
            return View("Error");
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
            if (!_context.Tilanne.Any())
            {
                var oletustilanteet = new List<Tilanne>
                {
                    new Tilanne() { KisaId = kisaId, Nimi = "Rakentamatta", TarvitseeHyvaksynnan = false },
                    new Tilanne() { KisaId = kisaId, Nimi = "Rakennettu", TarvitseeHyvaksynnan = false },
                    new Tilanne() { KisaId = kisaId, Nimi = "Valmis", TarvitseeHyvaksynnan = false },
                    new Tilanne() { KisaId = kisaId, Nimi = "Purettu", TarvitseeHyvaksynnan = true }
                };
                foreach (var tilanne in oletustilanteet)
                {
                    _context.Tilanne.Add(tilanne);
                }
                await _context.SaveChangesAsync();
            }


            var tilanteet = _context.Tilanne;

            return View(new LuoRastiViewModel() { KisaId = kisaId, Tilanteet = tilanteet });
        }

        // POST: Rasti/Luo
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("LuoRasti")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuoRasti([Bind("KisaId,Nimi,NykyinenTilanneId")] LuoRastiViewModel luoRastiViewModel)
        {
            
            if (ModelState.IsValid)
            {
                if (_context.Rasti.Where(x => x.Nimi == luoRastiViewModel.Nimi).Where(x => x.KisaId == luoRastiViewModel.KisaId).Any())
                {
                    ViewBag.Error = "Rasti tällä nimellä on jo olemassa";
                    return View(luoRastiViewModel);
                }
                var rasti = new Rasti() { KisaId = luoRastiViewModel.KisaId, Nimi = luoRastiViewModel.Nimi, nykyinenTilanneId = luoRastiViewModel.NykyinenTilanneId, OdottaaTilanneHyvaksyntaa = false};
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
            Kisa? kisa; 
            if(User.Identity.Name == _authorizationOptions.DefaultAdminUser)
            {
                kisa = await _context.Kisa
                .FirstOrDefaultAsync(m => m.Id == kisaId);
            }
            else
            {
                kisa = await _context.Kisa
                .FirstOrDefaultAsync(m => m.Id == kisaId);

                var roles = await (
               from usr in _context.Users
               join userRole in _context.UserRoles on usr.Id equals userRole.UserId
               join role in _context.Roles on userRole.RoleId equals role.Id
               where usr.UserName == User.Identity.Name
               select role.Id.ToString()
           ).ToArrayAsync();

                var rastit = await _roleAccessStore.HasAccessToRastiIdsAsync(roles);

                kisa.OikeusRasteihin = rastit;

            }
            return View(kisa);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nimi")] Kisa kisa)
        {
            if (id != kisa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kisa);
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
            

            var viewModel = new ListaaRastitViewModel() { KisaId= kisaId, Rastit = rastit, Tilanteet = _context.Tilanne };
            return View(viewModel);
        }
    }
}
