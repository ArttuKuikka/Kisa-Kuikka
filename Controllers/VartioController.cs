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
    public class VartioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VartioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vartio/Create
        public IActionResult Create(int kisaId)
        {
            ViewBag.Sarjat = _context.Sarja.ToList();
            return View(new Vartio() { KisaId = kisaId});
        }

        // POST: Vartio/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nimi,Numero,SarjaId,KisaId,Lippukunta,Tilanne")] Vartio vartio)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(vartio);
                await _context.SaveChangesAsync();
                return Redirect("/Kisa/" + vartio.KisaId);
            }
            return View(vartio);
        }

        // GET: Vartio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vartio == null)
            {
                return NotFound();
            }

            var vartio = await _context.Vartio.FindAsync(id);
            if (vartio == null)
            {
                return NotFound();
            }
            return View(vartio);
        }

        // POST: Vartio/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Nimi,Numero,SarjaId,Lippukunta,Tilanne")] Vartio vartio)
        {
            if (id != vartio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vartio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VartioExists(vartio.Id))
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
            return View(vartio);
        }

        // GET: Vartio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vartio == null)
            {
                return NotFound();
            }

            var vartio = await _context.Vartio
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vartio == null)
            {
                return NotFound();
            }

            return View(vartio);
        }

        // POST: Vartio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Vartio == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vartio'  is null.");
            }
            var vartio = await _context.Vartio.FindAsync(id);
            if (vartio != null)
            {
                _context.Vartio.Remove(vartio);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VartioExists(int? id)
        {
          return (_context.Vartio?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
