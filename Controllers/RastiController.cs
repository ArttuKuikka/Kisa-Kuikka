using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kisa_Kuikka.Data;
using Kisa_Kuikka.Models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using Kisa_Kuikka.Models.ViewModels;
using Kisa_Kuikka.Models.DynamicAuth;
using Kisa_Kuikka.Services;

namespace Kisa_Kuikka.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [MainController(Group = "Rasti")]
    public class RastiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoleAccessStore _roleAccessStore;
        private readonly DynamicAuthorizationOptions _authorizationOptions;
        private readonly IilmoitusService _IlmoitusService;

        public RastiController(ApplicationDbContext context, IRoleAccessStore roleAccessStore, DynamicAuthorizationOptions authorizationOptions, IilmoitusService ilmoitusService)
        {
            _context = context;
            _roleAccessStore = roleAccessStore;
            _authorizationOptions = authorizationOptions;
            _IlmoitusService = ilmoitusService;
        }


       
        // GET: Rasti/Edit/5
        [HttpGet("Edit")]
        [DisplayName("Muokkaa")]
        public async Task<IActionResult> Edit(int? RastiId)
        {
            if (RastiId == null || _context.Rasti == null)
            {
                return NotFound();
            }

            var rasti = await _context.Rasti.FindAsync(RastiId);
            if (rasti == null)
            {
                return NotFound();
            }
            return View(rasti);
        }

        // POST: Rasti/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? RastiId, [Bind("Id,SarjaId,KisaId,Nimi,Numero,VaadiKahdenKayttajanTarkistus,TarkistusKaytossa,tehtavaPaikat,PiilotaTilanneseurannasta")] Rasti rasti)
        {
            if (RastiId != rasti.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var findrasti = await _context.Rasti.FindAsync(RastiId);
               
                if(findrasti != null)
                {
                    if (rasti.Nimi != findrasti.Nimi)
                    {
                        if (_context.Rasti.Where(x => x.Nimi == rasti.Nimi).Where(x => x.KisaId == rasti.KisaId).Any())
                        {
                            ViewBag.Error = "Rasti tällä nimellä on jo olemassa";
                            return View(rasti);
                        }
                       
                    }
                    if(rasti.Numero != findrasti.Numero)
                    {
                        if (_context.Rasti.Where(x => x.Numero == rasti.Numero).Where(x => x.KisaId == rasti.KisaId).Any())
                        {
                            ViewBag.NumeroError = "Rasti tällä numerolla on jo olemassa";
                            return View(rasti);
                        }
                    }
                    try
                    {
                        findrasti.Nimi = rasti.Nimi;
                        findrasti.Numero = rasti.Numero;
                        findrasti.TarkistusKaytossa = rasti.TarkistusKaytossa;
                        findrasti.VaadiKahdenKayttajanTarkistus = rasti.VaadiKahdenKayttajanTarkistus;
                        findrasti.tehtavaPaikat = rasti.tehtavaPaikat;
                        findrasti.PiilotaTilanneseurannasta = rasti.PiilotaTilanneseurannasta;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RastiExists(rasti.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return Redirect("/Kisa/" + rasti.KisaId + "/Rastit");
                }
            }
            return View(rasti);
        }

        [HttpGet("Tilanne")]
        [DisplayName("Vaihda rastin tilannetta")]
        public async Task<IActionResult> Tilanne (int RastiId)
        {
            var rasti = await _context.Rasti.FindAsync(RastiId);
            if(rasti != null)
            {
                var tilanne = _context.Tilanne.First(x => x.Id == rasti.TilanneId);
                var tilanteet = _context.Tilanne.Where(x => x.KisaId == rasti.KisaId);


                //tarkista onko oikeus tilanteisiin jotka tarvitsee valtuudet
                var roles = await (
         from usr in _context.Users
         join userRole in _context.UserRoles on usr.Id equals userRole.UserId
         join role in _context.Roles on userRole.RoleId equals role.Id
         where usr.UserName == User.Identity.Name
         select role.Id.ToString()
     ).ToArrayAsync();

                if(!(User.Identity.Name.Equals(_authorizationOptions.DefaultAdminUser, StringComparison.CurrentCultureIgnoreCase) || await _roleAccessStore.HasAccessToActionAsync(":Kisa:TilanneValtuudet", roles)))
                {
                    tilanteet = tilanteet.Where(x => x.TarvitseeValtuudet == false);
                }
                

                var ViewModel = new TilanneViewModel() { Rasti = rasti,NykyinenTilanne = tilanne, Tilanteet = tilanteet };
                return View(ViewModel);
            }
            return View("Error");
        }


        [HttpPost("Tilanne")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tilanne(TilanneViewModel model)
        {
            if(model.Rasti?.Id != null && model.Rasti?.TilanneId != null)
            {
                var rasti = await _context.Rasti.FindAsync(model.Rasti.Id);
                var tilanne = await _context.Tilanne.FindAsync(model.Rasti.TilanneId);
                if(rasti != null && tilanne != null) 
                {
                    if(model.Rasti.TilanneId != rasti.TilanneId)
                    {
                        if (tilanne.TarvitseeValtuudet)
                        {

                            //tarkista onko oikeus tilanteisiin jotka tarvitsee valtuudet
                            var roles = await (
                     from usr in _context.Users
                     join userRole in _context.UserRoles on usr.Id equals userRole.UserId
                     join role in _context.Roles on userRole.RoleId equals role.Id
                     where usr.UserName == User.Identity.Name
                     select role.Id.ToString()
                 ).ToArrayAsync();

                            if (User.Identity.Name.Equals(_authorizationOptions.DefaultAdminUser, StringComparison.CurrentCultureIgnoreCase) || await _roleAccessStore.HasAccessToActionAsync(":Kisa:TilanneValtuudet", roles))
                            {
                                var kisa = await _context.Kisa.FindAsync(rasti.KisaId);
                                if (kisa?.LahetaIlmoituksiaRastinTilanvaihdosta ?? false)
                                {
                                    var rastiIdArray = new int[] {(int)rasti.Id };
                                    var title = "Rastisi tilanne vaihdettiin. (" + rasti.NumeroJaNimi + ")";
                                    var body = "Uusi tilanne: " + tilanne.Nimi;
                                    var refUrl = "https://" + Request.Host.ToString() + "/Rasti/Tilanne?RastiId=" + rasti.Id.ToString();
                                    await _IlmoitusService.SendNotifToRastiIdsAsync(rastiIdArray, title, body, refUrl, kisa.LahetaIlmoituksiaRastinTilanvaihdostaValtuudetOmaaville);
                                }
                                rasti.TilanneId = tilanne.Id;
                            }
                            else
                            {
                                return BadRequest("Ei oikeuksia tähän tilanteeseen");
                            }

                        }
                        else
                        {
                            
                            rasti.TilanneId = tilanne.Id;
                        }
                        _context.Rasti.Update(rasti);
                        _context.SaveChanges();
                    }
                    
                    return Redirect($"/Rasti/Tilanne?RastiId={rasti.Id}");
                }
                
            }
            return View("Error");
        }
       

        private bool RastiExists(int? id)
        {
            return (_context.Rasti?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
