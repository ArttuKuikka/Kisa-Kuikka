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
    [Static]
    public class VartioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VartioController(ApplicationDbContext context)
        {
            _context = context;
        }
        [DisplayName("Liitä tag")]
        public async Task<IActionResult> LiitaTag(int? VartioId)
        {
            if (_context.Vartio == null)
            {
                return NotFound();
            }

            if(VartioId != null)
            {
                var vartio = await _context.Vartio.FindAsync(VartioId);
                if (vartio == null)
                {
                    return NotFound();
                }

                ViewBag.Vartiot = _context.Vartio.Where(x => x.KisaId == vartio.KisaId);

                return View(new LiitaTagModel() { VartioId = (int)vartio.Id});
            }


            return BadRequest("Ei VartioIdtä");


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LiitaTag([Bind("VartioId, TagSerial")] LiitaTagModel liitaTagModel)
        {

            if (_context.Vartio == null)
            {
                return NotFound();
            }
            var poistettavat = _context.Vartio.Where(x => x.TagSerial == liitaTagModel.TagSerial);
           if (poistettavat != null)
            {
                foreach(var item in poistettavat)
                {
                    item.TagSerial = null;
                }
            }

            var vartio = await _context.Vartio.FindAsync(liitaTagModel.VartioId);
            if (vartio == null)
            {
                return NotFound();
            }

            vartio.TagSerial = liitaTagModel.TagSerial;
            _context.Update(vartio);
            _context.SaveChanges();

            return Redirect("/Kisa/" + vartio.KisaId + "/Vartiot");

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult OnkoTagLiitetty([FromForm] string TagSerial)
        {
            

            if (_context.Vartio == null)
            {
                return NotFound();
            }
            var vartiot = _context.Vartio.Where(x => x.TagSerial == TagSerial);
            var vartio = vartiot.FirstOrDefault();
            if (vartio == null)
            {
                return Ok("Ei ole");
            }
            else
            {
                return Ok(vartio.NumeroJaNimi);
            }

           

        }

        public IActionResult KenenTag()
        {
            return View("KenenTag");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KenenTag([Bind("TagSerial")] KenenTagModel kenenTagModel)
        {

            if (_context.Vartio == null)
            {
                return View("KenenTagTulos");
            }

            var vartio = _context.Vartio.Where(x => x.TagSerial == kenenTagModel.TagSerial);
            if (vartio == null)
            {
                return View("KenenTagTulos");
            }
            if(vartio.FirstOrDefault() == null)
            {
                return View("KenenTagTulos");
            }

            

            return View("KenenTagTulos", vartio.FirstOrDefault());

        }

        // GET: Vartio/Luo
        public IActionResult Luo(int kisaId, int SarjaId)
        {
            ViewBag.Sarjat = _context.Sarja.Where(k => k.KisaId== kisaId).ToList();
            ViewBag.Kisat = _context.Kisa.ToList(); //check mihkä oikeudet
            return View(new Vartio() { KisaId = kisaId, SarjaId = SarjaId});
        }

        // POST: Vartio/Luo
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Luo([Bind("Id,Nimi,Numero,SarjaId,KisaId,Lippukunta,Tilanne")] Vartio vartio)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(vartio);
                await _context.SaveChangesAsync();
                return Redirect("/Kisa/" + vartio.KisaId + "/Vartiot");
            }
            return View(vartio);
        }

        // GET: Vartio/Edit/5
        [DisplayName("Muokkaa")]
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
        public async Task<IActionResult> Edit(int? id, [Bind("Id,KisaId,Nimi,Numero,SarjaId,Lippukunta,Tilanne")] Vartio vartio)
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
                return Redirect("/Kisa/" + vartio.KisaId + "/Vartiot");
            }
            return View(vartio);
        }

        // GET: Vartio/Delete/5
        [DisplayName("Poista")]
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

                foreach(var vastaus in _context.TehtavaVastaus.Where(x => x.VartioId == vartio.Id))
                {
                    _context.TehtavaVastaus.Remove(vastaus);
                }
            }
            
            await _context.SaveChangesAsync();
            return Redirect("/Kisa/" + vartio.KisaId + "/Vartiot");
        }

        public async Task<IActionResult> Keskeyta(int? id)
        {
            if(id != null)
            {
                var vartio = await _context.Vartio.FindAsync(id);
                if(vartio != null)
                {
                    if (vartio.Keskeytetty)
                    {
                        vartio.Keskeytetty = false;
                    }
                    else
                    {
                        vartio.Keskeytetty = true;
                    }
                    
                    _context.Vartio.Update(vartio);
                    _context.SaveChanges();

                    return Redirect("/Kisa/" + vartio.KisaId + "/Vartiot");
                }
            }
            return BadRequest();
        }

        private bool VartioExists(int? id)
        {
          return (_context.Vartio?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
