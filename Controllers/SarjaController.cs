using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kipa_plus.Data;
using Kipa_plus.Models;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Metrics;
using System.Net;
using Newtonsoft.Json;

namespace Kipa_plus.Controllers
{
    [Route("[controller]")]
    public class SarjaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SarjaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("TagTilastot")]
        public IActionResult TagTilastot(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rastit = _context.Rasti.Where(r => r.SarjaId == id).ToList(); 
            var skannaukset = _context.TagSkannaus.ToList();
            var vartiot = _context.Vartio.Where(x => x.SarjaId == id).ToList();

            return View(new TagTilastoModel() { SarjanRastit = rastit, Skannaukset = skannaukset, Vartio = vartiot});
        }

        



        [HttpGet("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sarja == null)
            {
                return NotFound();
            }

            var sarja = await _context.Sarja
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sarja == null)
            {
                return NotFound();
            }

            return View(sarja);
        }

        // GET: Sarja/Create
        [HttpGet("Luo")]
        public IActionResult Luo(int kisaId)
        {
            
            ViewBag.Kisat = _context.Kisa.ToList(); //check mihkä oikeudet
            
            return View(new Sarja() { KisaId = kisaId });
        }

        // POST: Sarja/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Luo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Luo([Bind("Id,Nimi,KisaId,VartionMaksimiko,VartionMinimikoko")] Sarja sarja)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(sarja);
                await _context.SaveChangesAsync();
                return Redirect("/Kisa/" + sarja.KisaId + "/Sarjat");
            }
            return View(sarja);
        }
        [HttpGet("Edit")]
        // GET: Sarja/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Sarja == null)
            {
                return NotFound();
            }

            var sarja = await _context.Sarja.FindAsync(id);
            if (sarja == null)
            {
                return NotFound();
            }
            return View(sarja);
        }

        // POST: Sarja/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Nimi,KisaId,VartionMaksimiko,VartionMinimikoko")] Sarja sarja)
        {
            if (id != sarja.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sarja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SarjaExists(sarja.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("/Kisa/" + sarja.KisaId + "/Sarjat");
            }
            return View(sarja);
        }
        [HttpGet("Delete")]
        // GET: Sarja/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sarja == null)
            {
                return NotFound();
            }

            var sarja = await _context.Sarja
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sarja == null)
            {
                return NotFound();
            }

            return View(sarja);
        }

        // POST: Sarja/Delete/5
        [HttpPost("Delete"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Sarja == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Sarja'  is null.");
            }
            var sarja = await _context.Sarja.FindAsync(id);
            if (sarja != null)
            {
                _context.Sarja.Remove(sarja);
            }
            
            await _context.SaveChangesAsync();
            return Redirect("/Kisa/" + sarja.KisaId + "/Sarjat");
        }

        private bool SarjaExists(int? id)
        {
          return (_context.Sarja?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
