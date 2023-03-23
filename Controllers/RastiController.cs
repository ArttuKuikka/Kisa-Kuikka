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
using Kipa_plus.Models.ViewModels;

namespace Kipa_plus.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [MainController(Group = "Rasti")]
    public class RastiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RastiController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Edit(int? RastiId, [Bind("Id,SarjaId,KisaId,Nimi,OhjeId")] Rasti rasti)
        {
            if (RastiId != rasti.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rasti);
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
            return View(rasti);
        }

        [HttpGet("Tilanne")]
        [DisplayName("Muokkaa ja näytä rastin tilanne")]
        public async Task<IActionResult> Tilanne (int RastiId)
        {
            var rasti = await _context.Rasti.FindAsync(RastiId);
            if(rasti != null)
            {
                var tilanne = _context.Tilanne.First(x => x.Id == rasti.nykyinenTilanneId);
                var ViewModel = new TilanneViewModel() { Rasti = rasti,NykyinenTilanne = tilanne, Tilanteet = _context.Tilanne };
                return View(ViewModel);
            }
            return View("Error");
        }


        [HttpPost("Tilanne")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tilanne(TilanneViewModel model)
        {
            if(model.Rasti?.Id != null && model.Rasti?.nykyinenTilanneId != null)
            {
                var rasti = await _context.Rasti.FindAsync(model.Rasti.Id);
                var tilanne = await _context.Tilanne.FindAsync(model.Rasti.nykyinenTilanneId);
                if(rasti != null && tilanne != null) 
                {
                    if(model.Rasti.nykyinenTilanneId != rasti.nykyinenTilanneId)
                    {
                        if (tilanne.TarvitseeHyvaksynnan)
                        {
                            rasti.edellinenTilanneId = rasti.nykyinenTilanneId;
                            rasti.nykyinenTilanneId = tilanne.Id;
                            rasti.OdottaaTilanneHyvaksyntaa = true;
                        }
                        else
                        {
                            rasti.OdottaaTilanneHyvaksyntaa = false;
                            rasti.nykyinenTilanneId = tilanne.Id;
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
