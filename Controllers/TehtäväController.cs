using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kipa_plus.Data;
using Kipa_plus.Models;
using Kipaplus.Data.Migrations;

namespace Kipa_plus.Controllers
{
    public class TehtavaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TehtavaController(ApplicationDbContext context)
        {
            _context = context;
        }

       

        // GET: Tehtava
        public async Task<IActionResult> Index(int? RastiId)
        {
            if(RastiId == null || _context.Tehtava == null)
            {
                return NotFound();
            }
            var Tehtava = _context.Tehtava.Where(k => k.RastiId == RastiId);
            ViewBag.KisaId = _context.Rasti.Where(x => x.Id== RastiId).First().KisaId;
            ViewBag.RastiId = RastiId;
            return View(Tehtava);
        }

       //GET: Tayta
       public async Task<IActionResult> Tayta(int? TehtavaId)
        {
            if (TehtavaId == null || _context.Tehtava == null)
            {
                return NotFound();
            }
            var Tehtava = _context.Tehtava.First(x => x.Id == TehtavaId);

            ViewBag.Vartiot = _context.Vartio.Where(x => x.SarjaId == Tehtava.SarjaId);

            var vt = new Tayta() { Nimi = Tehtava.Nimi, PohjaJson = Tehtava.TehtavaJson, TehtavaId = TehtavaId };
            return View(vt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tayta([Bind("Nimi, VartioId, Kesken, PohjaJson, TehtavaId")] Tayta vastausTemp)
        {
            int? TehtavaId = vastausTemp.TehtavaId; //TODO: varmista että on oikeudet
            if(TehtavaId == null)
            {
                return BadRequest();
            }
            if (ModelState.IsValid) 
            {
                var TV = new TehtavaVastaus() { VartioId = vastausTemp.VartioId, Kesken = vastausTemp.Kesken, TehtavaJson = vastausTemp.PohjaJson };

                var TehtavaPohja = await _context.Tehtava.FindAsync(TehtavaId);

                if(TehtavaPohja== null)
                {
                    return StatusCode(500);
                }

                if(TV.TehtavaJson.Length < TehtavaPohja.TehtavaJson.Length)
                {
                    return BadRequest();
                }
                //TODO: varmista että on userdatas on json required


                TV.TehtavaId = TehtavaPohja.Id;
                TV.SarjaId = TehtavaPohja.SarjaId;
                TV.KisaId = TehtavaPohja.KisaId;
                TV.RastiId= TehtavaPohja.RastiId;
                
                _context.TehtavaVastaus.Add(TV);
                _context.SaveChanges();

                return Redirect("/Tehtava/?RastiId=" + TehtavaPohja.RastiId);
            }
            return BadRequest();
            
        }

        

        // GET: Tehtava/Luo
        public IActionResult Luo(int KisaId, int SarjaId, int RastiId)
        {

            ViewBag.Sarjat = _context.Sarja.Where(x => x.KisaId == KisaId).ToList(); //check että mihkä on oikeudet //ottaa defaulttina kaikki kisan sarjat ja rastit mutta jos ei saa id:tä querystä niin fallback siihen että ei mitään ja tulee valitun kisan perusteel (TAI mihkä oikeudet)
            ViewBag.Kisat = _context.Kisa.ToList();
            ViewBag.Rastit = _context.Rasti.Where(x => x.KisaId == KisaId).ToList();
            return View(new Tehtava() { KisaId = KisaId, SarjaId = SarjaId, RastiId = RastiId});
        }

        // POST: Tehtava/Luo
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Luo([Bind("Id,SarjaId,KisaId,RastiId,Nimi,Tarkistettu,TehtavaJson")] Tehtava Tehtava)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Tehtava);
                await _context.SaveChangesAsync();
                return Redirect("/Tehtava/?RastiId=" + Tehtava.RastiId);
            }
            return View(Tehtava);
        }

        // GET: Tehtava/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tehtava == null)
            {
                return NotFound();
            }

            var Tehtava = await _context.Tehtava.FindAsync(id);
           
            if (Tehtava == null)
            {
                return NotFound();
            }

            ViewBag.Sarjat = _context.Sarja.Where(x => x.KisaId == Tehtava.KisaId).ToList(); //check että mihkä on oikeudet //ottaa defaulttina kaikki kisan sarjat ja rastit mutta jos ei saa id:tä querystä niin fallback siihen että ei mitään ja tulee valitun kisan perusteel (TAI mihkä oikeudet)
            ViewBag.Kisat = _context.Kisa.ToList();
            ViewBag.Rastit = _context.Rasti.Where(x => x.KisaId == Tehtava.KisaId).ToList();
            return View(Tehtava);
        }

        // POST: Tehtava/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,SarjaId,KisaId,RastiId,Nimi,Tarkistettu,TehtavaJson")] Tehtava Tehtava)
        {
            if (id != Tehtava.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Tehtava);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TehtavaExists(Tehtava.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("/Teht%C3%A4v%C3%A4/?RastiId=" + Tehtava.RastiId);
            }
            return View(Tehtava);
        }

        // GET: Tehtava/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tehtava == null)
            {
                return NotFound();
            }

            var Tehtava = await _context.Tehtava
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Tehtava == null)
            {
                return NotFound();
            }

            return View(Tehtava);
        }

        // POST: Tehtava/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Tehtava == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tehtava'  is null.");
            }
            var Tehtava = await _context.Tehtava.FindAsync(id);
            if (Tehtava != null)
            {
                _context.Tehtava.Remove(Tehtava);
            }
            
            await _context.SaveChangesAsync();
            return Redirect("/Teht%C3%A4v%C3%A4/?RastiId=" + Tehtava.RastiId);
        }

        private bool TehtavaExists(int? id)
        {
          return (_context.Tehtava?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
