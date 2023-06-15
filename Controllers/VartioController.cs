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
using Newtonsoft.Json;
using KisaKuikka.Data.Migrations;

namespace Kisa_Kuikka.Controllers
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
            var sarjat = _context.Sarja.Where(k => k.KisaId == kisaId).ToList();
            var numerolista = new List<int>();
            foreach(var vartio in _context.Vartio.Where(x => x.KisaId == kisaId))
            {
                numerolista.Add(vartio.Numero);
            }

            var jsonolemassaOlevatVartiot = JsonConvert.SerializeObject(numerolista);
            return View(new LuoVartioViewModel() { KisaId = kisaId, SarjaId = SarjaId, Sarjat = sarjat, olemassaOlevatVartiot = jsonolemassaOlevatVartiot});
        }

        // POST: Vartio/Luo
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Luo([Bind("Nimi,Numero,SarjaId,KisaId,Lippukunta")] LuoVartioViewModel viewModel)
        {
            
            if (ModelState.IsValid)
            {
                var vartio = new Vartio() { Nimi = viewModel.Nimi, Numero = viewModel.Numero, SarjaId = viewModel.SarjaId, KisaId = viewModel.KisaId, Lippukunta = viewModel.Lippukunta};
                _context.Add(vartio);
                await _context.SaveChangesAsync();
                return Redirect("/Kisa/" + vartio.KisaId + "/Vartiot");
            }
            return View(viewModel);
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

            var sarjat = _context.Sarja.Where(k => k.KisaId == vartio.KisaId).ToList();
            var numerolista = new List<int>();
            foreach (var olemassaOlevaVartio in _context.Vartio.Where(x => x.KisaId == vartio.KisaId))
            {
                if(olemassaOlevaVartio.Id != id)
                {
                    numerolista.Add(olemassaOlevaVartio.Numero);
                }
                
            }

            var jsonolemassaOlevatVartiot = JsonConvert.SerializeObject(numerolista);

            var viewModel = new MuokkaaVartiotaViewModel()
            {
                Id = (int)vartio.Id,
                Nimi = vartio.Nimi,
                Numero = vartio.Numero,
                SarjaId= vartio.SarjaId,
                Lippukunta = vartio.Lippukunta,
                Sarjat = sarjat,
                olemassaOlevatVartiot = jsonolemassaOlevatVartiot
            };
            return View(viewModel);
        }

        // POST: Vartio/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Nimi,Numero,SarjaId,Lippukunta")] MuokkaaVartiotaViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }
            var muokattavaVartio = await _context.Vartio.FindAsync(viewModel.Id);
            if(muokattavaVartio != null)
            {
                if(muokattavaVartio.SarjaId != viewModel.SarjaId)
                {
                    _context.TehtavaVastaus.Where(x => x.VartioId == muokattavaVartio.Id).ToList().ForEach(x => _context.TehtavaVastaus.Remove(x));
                    await _context.SaveChangesAsync();
                }
                if (ModelState.IsValid)
                {

                    muokattavaVartio.Nimi = viewModel.Nimi;
                    muokattavaVartio.Numero = viewModel.Numero;
                    muokattavaVartio.SarjaId = viewModel.SarjaId;
                    muokattavaVartio.Lippukunta = viewModel.Lippukunta;

                    _context.SaveChanges();
                    return Redirect("/Kisa/" + muokattavaVartio.KisaId + "/Vartiot");
                }
                else
                {
                    var sarjat = _context.Sarja.Where(k => k.KisaId == muokattavaVartio.KisaId).ToList();
                    var numerolista = new List<int>();
                    foreach (var olemassaOlevaVartio in _context.Vartio.Where(x => x.KisaId == muokattavaVartio.KisaId))
                    {
                        if (olemassaOlevaVartio.Id != id)
                        {
                            numerolista.Add(olemassaOlevaVartio.Numero);
                        }
                    }

                    var jsonolemassaOlevatVartiot = JsonConvert.SerializeObject(numerolista);


                    viewModel.Sarjat = sarjat;
                    viewModel.olemassaOlevatVartiot = jsonolemassaOlevatVartiot;
                    return View(viewModel);
                }

               
            }
            return NotFound("Vartiota tällä ID llä ei ole olemassa");
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

                foreach(var skannaus in _context.TagSkannaus.Where(x => x.VartioId == vartio.Id))
                {
                    _context.TagSkannaus.Remove(skannaus);
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
