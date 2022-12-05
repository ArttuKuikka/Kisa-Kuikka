using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kipa_plus.Data;
using Kipa_plus.Models;

namespace Kipa_plus.Controllers
{
    [Route("[controller]")]
    public class RastiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RastiController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("Details")]
        // GET: Rasti/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rasti == null)
            {
                return NotFound();
            }

            var rasti = await _context.Rasti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rasti == null)
            {
                return NotFound();
            }

            return View(rasti);
        }

        [HttpGet("Create")]
        // GET: Rasti/Create
        public IActionResult Create(int kisaId, int SarjaId)
        {
            // ViewBag.Rastit = _context.Rasti.ToList();
            ViewBag.Sarjat = _context.Sarja.ToList();
            ViewBag.Kisat = _context.Kisa.ToList();

            return View(new Rasti() { KisaId = kisaId, SarjaId = SarjaId});
        }

        // POST: Rasti/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SarjaId,KisaId,Nimi,OhjeId")] Rasti rasti)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rasti);
                await _context.SaveChangesAsync();
                return Redirect("/Kisa/" + rasti.KisaId);
            }
            return View(rasti);
        }

        // GET: Rasti/Edit/5
        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rasti == null)
            {
                return NotFound();
            }

            var rasti = await _context.Rasti.FindAsync(id);
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
        public async Task<IActionResult> Edit(int? id, [Bind("Id,SarjaId,KisaId,Nimi,OhjeId")] Rasti rasti)
        {
            if (id != rasti.Id)
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
                return RedirectToAction(nameof(Index));
            }
            return View(rasti);
        }

        // GET: Rasti/Delete/5
        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Rasti == null)
            {
                return NotFound();
            }

            var rasti = await _context.Rasti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rasti == null)
            {
                return NotFound();
            }

            return View(rasti);
        }

        // POST: Rasti/Delete/5
        [HttpPost("Delete"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Rasti == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Rasti'  is null.");
            }
            var rasti = await _context.Rasti.FindAsync(id);
            if (rasti != null)
            {
                _context.Rasti.Remove(rasti);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RastiExists(int? id)
        {
          return (_context.Rasti?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
