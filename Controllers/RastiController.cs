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

       

        private bool RastiExists(int? id)
        {
            return (_context.Rasti?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
