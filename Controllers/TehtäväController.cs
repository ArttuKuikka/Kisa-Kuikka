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
    public class TehtäväController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TehtäväController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tehtävä
        public async Task<IActionResult> Index(int? RastiId)
        {
            if(RastiId == null || _context.Tehtävä == null)
            {
                return NotFound();
            }
            var tehtävä = _context.Tehtävä.Where(k => k.RastiId == RastiId);
            return View(tehtävä);
        }

        // GET: Tehtävä/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tehtävä == null)
            {
                return NotFound();
            }

            var tehtävä = await _context.Tehtävä
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tehtävä == null)
            {
                return NotFound();
            }

            return View(tehtävä);
        }

        // GET: Tehtävä/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tehtävä/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SarjaId,KisaId,RastiId,Nimi,Tarkistettu,TehtavaJson")] Tehtävä tehtävä)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tehtävä);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tehtävä);
        }

        // GET: Tehtävä/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tehtävä == null)
            {
                return NotFound();
            }

            var tehtävä = await _context.Tehtävä.FindAsync(id);
            if (tehtävä == null)
            {
                return NotFound();
            }
            return View(tehtävä);
        }

        // POST: Tehtävä/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,SarjaId,KisaId,RastiId,Nimi,Tarkistettu,TehtavaJson")] Tehtävä tehtävä)
        {
            if (id != tehtävä.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tehtävä);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TehtäväExists(tehtävä.Id))
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
            return View(tehtävä);
        }

        // GET: Tehtävä/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tehtävä == null)
            {
                return NotFound();
            }

            var tehtävä = await _context.Tehtävä
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tehtävä == null)
            {
                return NotFound();
            }

            return View(tehtävä);
        }

        // POST: Tehtävä/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Tehtävä == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tehtävä'  is null.");
            }
            var tehtävä = await _context.Tehtävä.FindAsync(id);
            if (tehtävä != null)
            {
                _context.Tehtävä.Remove(tehtävä);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TehtäväExists(int? id)
        {
          return (_context.Tehtävä?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
