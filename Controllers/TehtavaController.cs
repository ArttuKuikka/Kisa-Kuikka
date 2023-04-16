using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kisa_Kuikka.Data;
using Kisa_Kuikka.Models;
using KisaKuikka.Data.Migrations;
using Kisa_Kuikka.Models.ViewModels;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Kisa_Kuikka.Models.DynamicAuth;
using Kisa_Kuikka.Services;

namespace Kisa_Kuikka.Controllers
{
    [Authorize]
    [SubController(Group = "Rasti")]
    [DisplayName("Tehtävä")]
    public class TehtavaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRoleAccessStore _roleAccessStore;
        

        public TehtavaController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IRoleAccessStore roleAccessStore)
        {
            _context = context;
            _roleAccessStore = roleAccessStore;
            _userManager = userManager;
            
        }



        // GET: Tehtava
        [DisplayName("Listaa rastin tehtävät")]
        public async Task<IActionResult> Index(int? RastiId)
        {
            if (RastiId == null || _context.Tehtava == null)
            {
                return NotFound();
            }

            var rasti = _context.Rasti.Where(x => x.Id == RastiId).FirstOrDefault();
            if (rasti == null)
            {
                return BadRequest("Rastia ei löytynyt");
            }

            var ViewModel = new RastinTehtävätViewModel();
            ViewModel.TehtäväPohjat = _context.Tehtava.Where(k => k.RastiId == RastiId).ToList();
            ViewModel.TehtavaVastausKesken = _context.TehtavaVastaus.Where(k => k.RastiId == RastiId).Where(x => x.Kesken == true).ToList();
            ViewModel.TehtavaVastausTarkistus = _context.TehtavaVastaus.Where(k => k.RastiId == RastiId).Where(x => x.Tarkistettu == false).Where(x => x.Kesken == false).ToList();
            ViewModel.TehtavaVastausTarkistetut = _context.TehtavaVastaus.Where(k => k.RastiId == RastiId).Where(x => x.Tarkistettu == true).Where(x => x.Kesken == false).ToList();
            ViewModel.KisaId = rasti.KisaId;
            ViewModel.Rasti = rasti;
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
            ViewBag.VartioNimi = _context.Vartio.Where(x => x.Id == tehtava.VartioId).FirstOrDefault().NumeroJaNimi;
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
            foreach (var v in vast)
            {
                VIdList.Add(v.VartioId);
            }

            vt.VartioList.RemoveAll(x => VIdList.Contains((int)x.Id));

            vt.RastiId = Tehtava.RastiId;
            return View(vt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tayta([Bind("Nimi, VartioId, Kesken, PohjaJson, TehtavaId")] Tayta vastausTemp)
        {
            int? TehtavaId = vastausTemp.TehtavaId; //TODO: varmista että on oikeudet
            if (TehtavaId == null)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                if (await _context.Vartio.FindAsync(vastausTemp.VartioId) == null)
                {
                    return BadRequest("Vartiota ei ole olemassa");
                }
                if (_context.TehtavaVastaus.Where(x => x.TehtavaId == vastausTemp.TehtavaId).Where(X => X.VartioId == vastausTemp.VartioId).Any())
                {
                    return BadRequest("Vartiolla on jo tehtävä vastaus, jatka olemassa olevaa vastausta tai tarkista odottava vastaus.");
                }

                var TV = new TehtavaVastaus() { VartioId = vastausTemp.VartioId, Kesken = vastausTemp.Kesken, TehtavaJson = vastausTemp.PohjaJson };

                var TehtavaPohja = await _context.Tehtava.FindAsync(TehtavaId);

                if (TehtavaPohja == null)
                {
                    return StatusCode(500);
                }

                if (!await _roleAccessStore.OikeudetRastiIdhen(TehtavaPohja.RastiId, User?.Identity?.Name))
                {
                    return BadRequest("Ei oikeusia tähän rastiin");
                }

                if (TV.TehtavaJson.Length < TehtavaPohja.TehtavaJson.Length)
                {
                    return BadRequest();
                }
                //TODO: varmista että on userdatas on json required

                


                TV.TehtavaId = TehtavaPohja.Id;
                TV.SarjaId = TehtavaPohja.SarjaId;
                TV.KisaId = TehtavaPohja.KisaId;
                TV.RastiId = TehtavaPohja.RastiId;

                var rasti = await _context.Rasti.FindAsync(TV.RastiId);
                if(rasti != null && !rasti.TarkistusKaytossa)
                {
                    TV.Tarkistettu = true;
                }


                var user = await _userManager.GetUserAsync(User);
                TV.TäyttäjäUserId = user?.Id;
                TV.TäyttämisAika = DateTime.Now;

                _context.TehtavaVastaus.Add(TV);
                _context.SaveChanges();

                return Redirect("/Tehtava/?RastiId=" + TehtavaPohja.RastiId);
            }
            return BadRequest();

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
            if (Vartio == null)
            {
                return BadRequest();
            }

            var Malli = _context.TehtavaVastaus.Where(x => x.TehtavaId == TehtavaId).Where(x => x.VartioId == VartioId).First();
            if (Malli == null)
            {
                return BadRequest("Tätä tehtävää ei ole syötetty vielä, joten sitä ei voi tarkistaa");
            }

            
            int index = 0;
            var tehtArr = JArray.Parse(Tehtava.TehtavaJson);
            var vastArr = JArray.Parse(Malli.TehtavaJson) ;
            foreach (var field in tehtArr)
            {
                var tarkistafield = field["tarkista"];
                if(tarkistafield != null)
                {
                    var val = tarkistafield.Value<string>();
                    if (val == "False")
                    {
                        field["userData"] = vastArr[index]["userData"];
                    }
                }
                index++;
            }
            var vm = new TarkistaTehtäväViewModel();

            vm.TehtavaJson = tehtArr.ToString();
            vm.VartioId = (int)Vartio.Id;
            vm.VartionNumeroJaNimi = Vartio.NumeroJaNimi;
            vm.VertausMalli = Malli;
            vm.TehtavaNimi = Tehtava.Nimi;
            vm.TehtavaId = (int)TehtavaId;
            vm.RastiId = Malli.RastiId;


            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tarkista([Bind("VartioId, TehtavaJson, TehtavaId")] TarkistaTehtäväViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var aiempitehtva = _context.TehtavaVastaus.Where(x => x.TehtavaId == ViewModel.TehtavaId).Where(x => x.VartioId == ViewModel.VartioId).First();
                var tehtäväpohja = await _context.Tehtava.FindAsync(ViewModel.TehtavaId);

                if (!await _roleAccessStore.OikeudetRastiIdhen(aiempitehtva.RastiId, User?.Identity?.Name))
                {
                    return BadRequest("Ei oikeusia tähän rastiin");
                }
                var rasti = await _context.Rasti.FindAsync(aiempitehtva.RastiId);
                var jatkajat = new List<(string, DateTime)>();
                if(aiempitehtva.JatkajatJson != null)
                {
                    var arr = JArray.Parse(aiempitehtva.JatkajatJson);
                    foreach(var item in arr)
                    {
                        jatkajat.Add((item["UserId"].ToString(), DateTime.Parse(item["Aika"].ToString())));
                    }
                }
                if(aiempitehtva.TäyttäjäUserId == user?.Id || jatkajat.Where(x => x.Item1 == user?.Id).Any())
                {
                    if(rasti != null)
                    {
                        if (rasti.VaadiKahdenKayttajanTarkistus)
                        {
                            return BadRequest("Et voi tarkistaa tehtävää jonka olet itse täyttänyt");
                        }
                    }
                }


                var json = JArray.Parse(ViewModel.TehtavaJson);
                foreach (var row in json)
                {
                    var rowobj = row as JObject;
                    if (rowobj != null)
                    {
                        var userdata = rowobj.Property("userData");
                        if (userdata != null)
                        {
                            userdata.Remove();
                        }
                    }
                }

                var newjson = json.ToString();


                if (tehtäväpohja.TehtavaJson != null)
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
                
                aiempitehtva.TarkistajaUserId = user?.Id;
                aiempitehtva.TarkistusAika = DateTime.Now;
                _context.SaveChanges();

                return Redirect("/Tehtava/?RastiId=" + aiempitehtva.RastiId);
            }
            return BadRequest();

        }

        [DisplayName("Muokkaa vastausta(ADMIN)")]
        public async Task<IActionResult> MuokkaaVastausta(int VastausId)
        {
            if (VastausId == null || _context.Tehtava == null)
            {
                return NotFound();
            }
            var vastaus = await _context.TehtavaVastaus.FindAsync(VastausId);
            if (vastaus == null)
            {
                return BadRequest();
            }
            var Tehtava = _context.Tehtava.First(x => x.Id == vastaus.TehtavaId);
            var Vartio = await _context.Vartio.FindAsync(vastaus.VartioId);
            if (Vartio == null)
            {
                return BadRequest();
            }

            var vm = new MuokkaaVastaustaViewModel();

            vm.TehtavaJson = vastaus.TehtavaJson;
            vm.VartioId = (int)Vartio.Id;
            vm.VartionNumeroJaNimi = Vartio.NumeroJaNimi;

            vm.TehtavaNimi = Tehtava.Nimi;
            vm.TehtavaId = (int)vastaus.TehtavaId;
            vm.RastiId = vastaus.RastiId;


            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MuokkaaVastausta([Bind("VartioId, TehtavaJson, TehtavaId")] TarkistaTehtäväViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                var aiempitehtva = _context.TehtavaVastaus.Where(x => x.TehtavaId == ViewModel.TehtavaId).Where(x => x.VartioId == ViewModel.VartioId).First();
                var tehtäväpohja = await _context.Tehtava.FindAsync(ViewModel.TehtavaId);

                if (!await _roleAccessStore.OikeudetRastiIdhen(aiempitehtva.RastiId, User?.Identity?.Name))
                {
                    return BadRequest("Ei oikeusia tähän rastiin");
                }


                var json = JArray.Parse(ViewModel.TehtavaJson);
                foreach (var row in json)
                {
                    var rowobj = row as JObject;
                    if (rowobj != null)
                    {
                        var userdata = rowobj.Property("userData");
                        if (userdata != null)
                        {
                            userdata.Remove();
                        }
                    }
                }

                var newjson = json.ToString();


                if (tehtäväpohja.TehtavaJson != null)
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
        public async Task<IActionResult> Jatka([Bind("TehtäväVastausId, TehtäväJson, RastiId, Kesken")] JatkaTehtävääViewModel jatkaTehtävääViewModel)
        {
            if (ModelState.IsValid)
            {
                var tehtvastaus = _context.TehtavaVastaus.First(x => x.Id == jatkaTehtävääViewModel.TehtäväVastausId);

                if (!await _roleAccessStore.OikeudetRastiIdhen(tehtvastaus.RastiId, User?.Identity?.Name))
                {
                    return BadRequest("Ei oikeusia tähän rastiin");
                }


                tehtvastaus.TehtavaJson = jatkaTehtävääViewModel.TehtäväJson;
                tehtvastaus.Kesken = jatkaTehtävääViewModel.Kesken;
                var user = await _userManager.GetUserAsync(User);

                if(tehtvastaus.JatkajatJson == null)
                {
                    tehtvastaus.JatkajatJson = "[]";
                }
                var jatkajat = JArray.Parse(tehtvastaus.JatkajatJson);
                jatkajat.Add(new JObject() { { "UserId", user?.Id }, { "Aika", DateTime.Now } });

                tehtvastaus.JatkajatJson = jatkajat.ToString();


                _context.Update(tehtvastaus);
                _context.SaveChanges();

                return Redirect("/Tehtava/?RastiId=" + jatkaTehtävääViewModel.RastiId);
            }
            return BadRequest();
        }

        



        // GET: Tehtava/Luo
        public IActionResult Luo(int? KisaId, int? RastiId)
        {
            if (KisaId != null && RastiId != null)
            {
                var viewModel = new LuoTehtavaViewModel() { KisaId = (int)KisaId, RastiId = (int)RastiId };

                var sarjaList = new List<CheckboxViewModel>();
                foreach (var sarja in _context.Sarja)
                {
                    if (sarja.Id != null)
                    {
                        sarjaList.Add(new CheckboxViewModel() { Id = sarja.Id?.ToString(), DisplayName = sarja.Nimi, IsChecked = false });
                    }
                }
                viewModel.Sarjat = sarjaList;

                return View(viewModel);
            }



            return BadRequest();
        }

        // POST: Tehtava/Luo
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Luo([Bind("RastiId,KisaId,Sarjat,Nimi,TehtavaJson")] LuoTehtavaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.KisaId == 0 || viewModel.RastiId == 0)
                {
                    return BadRequest("Virheellinen KisaId tai RastiId");
                }
                if (viewModel.Sarjat != null && !viewModel.Sarjat.Any())
                {
                    return BadRequest("Ei sarjoja");
                }

                if (!await _roleAccessStore.OikeudetRastiIdhen(viewModel.RastiId, User?.Identity?.Name))
                {
                    return BadRequest("Ei oikeusia tähän rastiin");
                }


                var checklist = viewModel.Sarjat?.Where(x => x.IsChecked == true).ToList();
                if (checklist != null)
                {
                    foreach (var sarja in checklist)
                    {
                        var testisarja = await _context.Sarja.FindAsync(int.Parse(sarja.Id));
                        if (testisarja == null)
                        {
                            return BadRequest("Sarjaa ei ole olemassa");
                        }

                        var teht = new Tehtava() { KisaId = viewModel.KisaId, RastiId = viewModel.RastiId, TehtavaJson = viewModel.TehtavaJson, SarjaId = int.Parse(sarja.Id) };

                        if (viewModel.Nimi == null || viewModel.Nimi == "")
                        {
                            teht.Nimi = _context.Sarja.First(x => x.Id == int.Parse(sarja.Id)).Nimi + "-sarjan tehtävä";
                        }
                        else
                        {
                            teht.Nimi = viewModel.Nimi;
                        }
                        _context.Add(teht);
                    }

                    _context.SaveChanges();
                }

                return Redirect("/Tehtava/?RastiId=" + viewModel.RastiId);
            }
            return View(viewModel);
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
                if (!await _roleAccessStore.OikeudetRastiIdhen(Tehtava.RastiId, User?.Identity?.Name))
                {
                    return BadRequest("Ei oikeusia tähän rastiin");
                }

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
                foreach (var item in poista)
                {
                    _context.TehtavaVastaus.Remove(item);
                }
                await _context.SaveChangesAsync();

                return Redirect("/Tehtava/?RastiId=" + Tehtava.RastiId);
            }
            return View(Tehtava);
        }

        // GET: Tehtava/Delete/5
        [DisplayName("Poista tehtävä")]
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([Bind("Id")] int id)
        {
            if (_context.Tehtava == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tehtava'  is null.");
            }
            var Tehtava = await _context.Tehtava.FindAsync(id);

            if (Tehtava != null)
            {
                var rid = Tehtava.RastiId;

                if (!await _roleAccessStore.OikeudetRastiIdhen(rid, User?.Identity?.Name))
                {
                    return BadRequest("Ei oikeusia tähän rastiin");
                }

                _context.Tehtava.Remove(Tehtava);

                var poistettavatVastaukset = _context.TehtavaVastaus.Where(x => x.TehtavaId == Tehtava.Id).ToList();
                foreach (var vastaus in poistettavatVastaukset)
                {
                    _context.TehtavaVastaus.Remove(vastaus);
                }

                await _context.SaveChangesAsync();
                return Redirect("/Tehtava/?RastiId=" + rid);
            }

            return BadRequest("Tehtävää ei ole olemassa");
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
                if(!await _roleAccessStore.OikeudetRastiIdhen(rid, User?.Identity?.Name))
                {
                    return BadRequest("Ei oikeusia tähän rastiin");
                }

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
