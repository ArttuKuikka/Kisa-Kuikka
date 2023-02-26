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
using Kipa_plus.Models.ViewModels;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;

namespace Kipa_plus.Controllers
{
    [Authorize]
    public class TehtavaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TehtavaController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET: Tehtava
        [DisplayName("Listaa rastin tehtävät")]
        public async Task<IActionResult> Index(int? RastiId)
        {
            if(RastiId == null || _context.Tehtava == null)
            {
                return NotFound();
            }

            

            var ViewModel = new RastinTehtävätViewModel();
            ViewModel.TehtäväPohjat = _context.Tehtava.Where(k => k.RastiId == RastiId).ToList();
            ViewModel.TehtavaVastausKesken = _context.TehtavaVastaus.Where(k => k.RastiId == RastiId).Where(x => x.Kesken == true).ToList();
            ViewModel.TehtavaVastausTarkistus = _context.TehtavaVastaus.Where(k => k.RastiId == RastiId).Where(x => x.Tarkistettu == false).Where(x => x.Kesken == false).ToList();
            ViewModel.TehtavaVastausTarkistetut = _context.TehtavaVastaus.Where(k => k.RastiId == RastiId).Where(x => x.Tarkistettu == true).Where(x => x.Kesken == false).ToList();
            ViewModel.KisaId = _context.Rasti.Where(x => x.Id== RastiId).First().KisaId;
            ViewModel.RastiId = RastiId;
            ViewModel.Sarjat = _context.Sarja.ToList();
            ViewModel.Vartiot = _context.Vartio;

           

            return View(ViewModel);
        }


        [DisplayName("Näytä")]
        public IActionResult Nayta(int? TehtavaVastausId)
        {
            if (TehtavaVastausId == null)
            {
                return NotFound();
            }

            
            var tehtava = _context.TehtavaVastaus.Where(x => x.Id == TehtavaVastausId).FirstOrDefault();
            ViewBag.VartioNimi = _context.Vartio.Where(x => x.Id == tehtava.VartioId).FirstOrDefault().Nimi;
            return View(tehtava);
        }

        //GET: Tayta
        [DisplayName("Täytä")]
        public async Task<IActionResult> Tayta(int? TehtavaId)
        {
            if (TehtavaId == null || _context.Tehtava == null)
            {
                return NotFound();
            }
            var Tehtava = _context.Tehtava.First(x => x.Id == TehtavaId);

            var vt = new Tayta() { Nimi = Tehtava.Nimi, PohjaJson = Tehtava.TehtavaJson, TehtavaId = TehtavaId };

            vt.VartioList = _context.Vartio.Where(x => x.SarjaId == Tehtava.SarjaId).ToList();

            var vast = _context.TehtavaVastaus.Where(x => x.TehtavaId == TehtavaId).ToList();
            var VIdList = new List<int>();  
            foreach(var v in vast)
            {
                VIdList.Add(v.VartioId);
            }

            vt.VartioList.RemoveAll( x => VIdList.Contains((int)x.Id));


            return View(vt);
        }


        //GET: Tarkista
        public async Task<IActionResult> Tarkista(int? TehtavaId, int? VartioId)
        {
            if (TehtavaId == null || _context.Tehtava == null)
            {
                return NotFound();
            }
            if (VartioId == null || _context.Vartio == null)
            {
                return NotFound();
            }
            var Tehtava = _context.Tehtava.First(x => x.Id == TehtavaId);
            var Vartio = await _context.Vartio.FindAsync(VartioId);
            if(Vartio == null)
            {
                return BadRequest();
            }

            var Malli = _context.TehtavaVastaus.Where(x => x.TehtavaId == TehtavaId).Where(x => x.VartioId == VartioId).First();
            if(Malli == null)
            {
                return BadRequest("Tätä tehtävää ei ole syötetty vielä, joten sitä ei voi tarkistaa");
            }

            var vm = new TarkistaTehtäväViewModel();

            vm.TehtavaJson = Tehtava.TehtavaJson;
            vm.VartioId = (int)Vartio.Id;
            vm.VartionNumeroJaNimi = Vartio.NumeroJaNimi;
            vm.VertausMalli = Malli;
            vm.TehtavaNimi = Tehtava.Nimi;
            vm.TehtavaId = (int)TehtavaId;


            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tarkista([Bind("VartioId, TehtavaJson, TehtavaId")] TarkistaTehtäväViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                var aiempitehtva = _context.TehtavaVastaus.Where(x => x.TehtavaId == ViewModel.TehtavaId).Where(x => x.VartioId == ViewModel.VartioId).First();
                var tehtäväpohja = await _context.Tehtava.FindAsync(ViewModel.TehtavaId);

                var json = JArray.Parse(ViewModel.TehtavaJson);
                foreach (var row in json)
                {
                    var rowobj = row as JObject;
                    if(rowobj != null)
                    {
                        var userdata = rowobj.Property("userData");
                        if (userdata != null)
                        {
                            userdata.Remove();
                        }
                    }
                }

                var newjson = json.ToString();
                

               if(tehtäväpohja.TehtavaJson != null)
                {
                    if (newjson != JArray.Parse(tehtäväpohja.TehtavaJson).ToString())
                    {
                        return BadRequest("Palautettu Json on virheellisessä muodossa");
                    }
                }
                else
                {
                    return BadRequest("Palautettu Json on virheellistä");
                }

                aiempitehtva.TehtavaJson = ViewModel.TehtavaJson;
                aiempitehtva.Tarkistettu = true;
                _context.SaveChanges();

                return Redirect("/Tehtava/?RastiId=" + aiempitehtva.RastiId);
            }
            return BadRequest();

        }



        public async Task<IActionResult> Jatka(int? TehtavaId)
        {
            if (TehtavaId == null || _context.Tehtava == null)
            {
                return NotFound();
            }
            var Tehtava = _context.TehtavaVastaus.First(x => x.Id == TehtavaId);

            var ViewModel = new JatkaTehtävääViewModel();
            ViewModel.VartioNimi = _context.Vartio.First(x => x.Id == Tehtava.VartioId).Nimi;
            ViewModel.TehtäväNimi = _context.Tehtava.First(x => x.Id == Tehtava.TehtavaId).Nimi;
            ViewModel.TehtäväVastausId = (int)TehtavaId;
            ViewModel.TehtäväJson = Tehtava.TehtavaJson;
            ViewModel.RastiId = Tehtava.RastiId;

            
            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Jatka([Bind("TehtäväVastausId, TehtäväJson, RastiId")] JatkaTehtävääViewModel jatkaTehtävääViewModel)
        {
            if (ModelState.IsValid)
            {
                var tehtvastaus = _context.TehtavaVastaus.First(x => x.Id == jatkaTehtävääViewModel.TehtäväVastausId);

                tehtvastaus.TehtavaJson = jatkaTehtävääViewModel.TehtäväJson;
                tehtvastaus.Kesken = false;

                _context.Update(tehtvastaus);
                _context.SaveChanges();

                return Redirect("/Tehtava/?RastiId=" + jatkaTehtävääViewModel.RastiId);
            }
            return BadRequest();
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
                if(_context.TehtavaVastaus.Where(x => x.TehtavaId == vastausTemp.TehtavaId).Where(X => X.VartioId == vastausTemp.VartioId).Any())
                {
                    return BadRequest("Vartiolla on jo tehtävä vastaus, jatka olemassa olevaa vastausta tai tarkista odottava vastaus.");
                }

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
                if(Tehtava.Nimi == null || Tehtava.Nimi == "")
                {
                    Tehtava.Nimi = _context.Sarja.First(x => x.Id == Tehtava.SarjaId).Nimi + "-sarjan tehtävä";
                }
                _context.Add(Tehtava);
                await _context.SaveChangesAsync();
                return Redirect("/Tehtava/?RastiId=" + Tehtava.RastiId);
            }
            return View(Tehtava);
        }

        // GET: Tehtava/Edit/5
        [DisplayName("Muokkaa")]
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

                var poista = _context.TehtavaVastaus.Where(x => x.TehtavaId == Tehtava.Id);
                foreach(var item in poista)
                {
                    _context.TehtavaVastaus.Remove(item);
                }
                await _context.SaveChangesAsync();

                return Redirect("/Tehtava/?RastiId=" + Tehtava.RastiId);
            }
            return View(Tehtava);
        }

        // GET: Tehtava/Delete/5
        [DisplayName("Poista")]
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
        [HttpPost, ActionName("Poista")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Tehtava == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tehtava'  is null.");
            }
            var Tehtava = await _context.Tehtava.FindAsync(id);
            var rid = Tehtava.RastiId;
            if (Tehtava != null)
            {
                _context.Tehtava.Remove(Tehtava);
            }
            
            await _context.SaveChangesAsync();
            return Redirect("/Tehtava/?RastiId=" + rid);
        }

        [DisplayName("Poista vastaus")]
        public async Task<IActionResult> PoistaVastaus(int? id)
        {
            if (id == null || _context.Tehtava == null)
            {
                return NotFound();
            }

            var Tehtava = await _context.TehtavaVastaus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Tehtava == null)
            {
                return NotFound();
            }

            return View(Tehtava);
        }

        // POST: Tehtava/Delete/5
        [HttpPost, ActionName("PoistaVastaus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PoistaVastausConf(int? id)
        {
            if (_context.Tehtava == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tehtava'  is null.");
            }
            var Tehtava = await _context.TehtavaVastaus.FindAsync(id);
            var rid = Tehtava.RastiId;
            if (Tehtava != null)
            {
                _context.TehtavaVastaus.Remove(Tehtava);
            }

            await _context.SaveChangesAsync();
            return Redirect("/Tehtava/?RastiId=" + rid);
        }

        private bool TehtavaExists(int? id)
        {
          return (_context.Tehtava?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
