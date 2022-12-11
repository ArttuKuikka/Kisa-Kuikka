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
    public class KisaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KisaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Kisa
        [HttpGet("{kisaId:int}/")]
        public async Task<IActionResult> Index(int kisaId)
        {
            if(kisaId == 0)
            {
                return Redirect("/");
            }
            var kisa = await _context.Kisa
                .FirstOrDefaultAsync(m => m.Id == kisaId);
            return View(kisa);
        }

        // GET: Kisa/Details/5
        [HttpGet("{kisaId:int}/Details")]
        public async Task<IActionResult> Details(int kisaId)
        {
            if (kisaId == null || _context.Kisa == null)
            {
                return NotFound();
            }

            var kisa = await _context.Kisa
                .FirstOrDefaultAsync(m => m.Id == kisaId);
            if (kisa == null)
            {
                return NotFound();
            }

            return View(kisa);
        }

        // GET: Kisa/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kisa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nimi")] Kisa kisa)
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
            ViewBag.KisaId = kisaId;
            return View(vartiot);
        }

        [HttpGet("{kisaId:int}/Rastit")]
        public async Task<IActionResult> Rastit(int kisaId)
        {
            if (kisaId == 0 || _context.Rasti == null)
            {
                return NotFound();
            }

            var rastit = _context.Rasti
                .Where(m => m.KisaId == kisaId);
            if (rastit == null)
            {
                return NotFound();
            }
            ViewData["Sarjat"] = _context.Sarja.ToList();

            ViewBag.KisaId = kisaId;
            return View(rastit);
        }
    }
}
