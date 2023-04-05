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
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using Kipa_plus.Models.ViewModels;
using Kipaplus.Data.Migrations;

namespace Kipa_plus.Controllers
{
    [Authorize]
    [Static]
    [Route("[controller]")]
    public class SarjaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SarjaController(ApplicationDbContext context)
        {
            _context = context;
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

        // GET: Sarja/Luo
        [HttpGet("Luo")]
        public IActionResult Luo(int kisaId)
        {
            var rastit = _context.Rasti.Where(x => x.KisaId == kisaId).ToList();
            rastit.Sort((p1, p2) => p1.Numero.CompareTo(p2.Numero));
            var viewModel = new Models.ViewModels.SarjaViewModel { KisaId = kisaId, Rastit = rastit };
            return View(viewModel);
        }

        // POST: Sarja/Luo
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Luo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Luo([Bind("Nimi,KisaId,VartionMaksimiko,VartionMinimikoko,Numero,KaytaSeuraavanRastinTunnistusta,RastienJarjestysJSON,Rastit")] SarjaViewModel viewModel)
        {
            
            if (ModelState.IsValid)
            {
                
                _context.Sarja.Add((Sarja)viewModel);
                await _context.SaveChangesAsync();
                return Redirect("/Kisa/" + viewModel.KisaId + "/Sarjat");
            }
            return View(viewModel);
        }
        [HttpGet("Edit")]
        [DisplayName("Muokkaa")]
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

            var viewModel = new SarjaViewModel() { Id= sarja.Id, Nimi = sarja.Nimi, KisaId = sarja.KisaId, Numero = sarja.Numero, VartionMaksimiko = sarja.VartionMaksimiko, VartionMinimikoko = sarja.VartionMinimikoko, KaytaSeuraavanRastinTunnistusta = sarja.KaytaSeuraavanRastinTunnistusta};

            var uudetrastit = _context.Rasti.Where(x => x.KisaId == sarja.KisaId).ToList();

            var uusilista = new List<Rasti>();
            if(sarja.RastienJarjestysJSON != null)
            {
                foreach (var rasti in JArray.Parse(sarja.RastienJarjestysJSON))
                {
                    var success = int.TryParse(rasti["id"]?.ToString(), out var parsedid);
                    if (success)
                    {
                        var findrasti = await _context.Rasti.FindAsync(parsedid);
                        if (findrasti != null)
                        {
                            uusilista.Add(findrasti);
                            uudetrastit.Remove(findrasti);
                        }
                    }
                }
            }
            //jos puuttuu uusia rasteja lisää ne loppuun
            uudetrastit.ForEach(x => uusilista.Add(x));

            viewModel.Rastit = uusilista;
            return View(viewModel);
            
        }

        // POST: Sarja/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Nimi,KisaId,VartionMaksimiko,VartionMinimikoko,Numero,KaytaSeuraavanRastinTunnistusta,RastienJarjestysJSON,Rastit")] SarjaViewModel viewModel)
        {
           

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update((Sarja)viewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SarjaExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("/Kisa/" + viewModel.KisaId + "/Sarjat");
            }
            return View(viewModel);
        }
        [HttpGet("Delete")]
        [DisplayName("Poista")]
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

                foreach(var vastaus in _context.TehtavaVastaus.Where(x => x.SarjaId == sarja.Id)) 
                {
                    _context.TehtavaVastaus.Remove(vastaus);
                }

                foreach(var tehtpohja in _context.Tehtava.Where(x => x.SarjaId == sarja.Id))
                {
                    _context.Tehtava.Remove(tehtpohja);
                }
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
