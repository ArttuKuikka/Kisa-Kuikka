﻿using Kisa_Kuikka.Data;
using Microsoft.AspNetCore.Mvc;
using Kisa_Kuikka.Models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using Kisa_Kuikka.Models.ViewModels;
using System.Collections.Generic;
using Kisa_Kuikka.Models.DynamicAuth;
using System.Globalization;

namespace Kisa_Kuikka.Controllers
{
    [Authorize]
    [SubController(Group = "Rasti")]
    public class TagController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoleAccessStore _roleAccessStore;

        public TagController(ApplicationDbContext context, IRoleAccessStore roleAccessStore)
        {
            _context = context;
            _roleAccessStore = roleAccessStore;
        }

        [DisplayName("Valintasivu")]
        public async Task<IActionResult> Index(int? RastiId)
        {
            if(RastiId != null)
            {
                var rasti = await _context.Rasti.FindAsync(RastiId);
                if(rasti != null)
                {
                    var skannatut = _context.TagSkannaus.Where(x => x.RastiId == RastiId);
                    var skannaukset = new List<SkannatutViewModel>();

                    var skannatutIdt = new List<int?>();
                    skannatut.ToList().ForEach(x => skannatutIdt.Add(x.VartioId));

                    foreach (var vartio in _context.Vartio.ToList().Where(x => skannatutIdt.Contains(x.Id)))
                    {
                        var tulo = skannatut.Where(x => x.VartioId == vartio.Id).FirstOrDefault(x => x.isTulo == true);
                        var lähtö = skannatut.Where(x => x.VartioId == vartio.Id).FirstOrDefault(x => x.isTulo == false);

                        var scan = new SkannatutViewModel() { Lahto = lähtö?.TimeStamp, Tulo = tulo?.TimeStamp, Vartio = vartio, TuloId = tulo?.Id, LahtoId = lähtö?.Id };

                        skannaukset.Add(scan);
                    }

                    //järjestä lista niin että ensimmäisenä näkyy kohdat joissa on tulo mutta ei lähtö, sitten kohdat joissa pelkkä lähtö, sitten kohdat jossa kummatkin
                    skannaukset = skannaukset.OrderBy(x => x.Lahto == null).ThenBy(x => x.Tulo == null).ThenBy(x => x.Lahto).ToList();
                    skannaukset.Reverse();

                    return View(new TagIndexViewModel() { RastiId = (int)RastiId, Skannatut = skannaukset, RastiNimi = rasti.NumeroJaNimi });
                }
            }
            return BadRequest();
        }

        [DisplayName("Lue lähtö")]
        public async Task<IActionResult> LueLahto(int RastiId)
        {
            var rasti = await _context.Rasti.FindAsync(RastiId);
            if(rasti != null)
            {
                return View(new TagSkannausViewModel() { RastiId = RastiId, RastiNimi = rasti.NumeroJaNimi  });
            }
            return BadRequest("Virheellinen RastiID");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LueLahto([Bind("RastiId, TagSerial")] TagSkannausViewModel viewModel)
        {
            bool HyvaSkannusTulos = false;
            if(ModelState.IsValid)
            {
                if (viewModel != null)
                {

                    if (!await _roleAccessStore.OikeudetRastiIdhen(viewModel.RastiId, User?.Identity?.Name))
                    {
                        return BadRequest("Ei oikeusia tähän rastiin");
                    }

                    ViewBag.RastiId = viewModel.RastiId;
                    if (viewModel.TagSerial != null)
                    {
                        var tagSkannaus = new TagSkannaus();
                        tagSkannaus.TimeStamp = DateTime.Now;
                        tagSkannaus.isTulo = false;
                        tagSkannaus.RastiId = viewModel.RastiId;

                        var vartio = _context.Vartio.FirstOrDefault(x => x.TagSerial == viewModel.TagSerial);

                        if (vartio == null)
                        {
                            return View("SkannausTulos", HyvaSkannusTulos); 
                        }

                        tagSkannaus.VartioId = vartio.Id;

                        var find = _context.TagSkannaus.Where(x => x.RastiId == tagSkannaus.RastiId).Where(x => x.VartioId == tagSkannaus.VartioId).Where(x => x.isTulo == tagSkannaus.isTulo);
                        if (find.FirstOrDefault() != null)
                        {
                          
                            var item = find.FirstOrDefault();
                            if (item == null)
                            {
                                return View("SkannausTulos", HyvaSkannusTulos);
                            }
                            
                            item.TimeStamp = tagSkannaus.TimeStamp;

                        }
                        else
                        {
                            _context.Add(tagSkannaus);
                        }


                        _context.SaveChanges();

                        HyvaSkannusTulos = true;
                        ViewBag.SkannattuVartio = vartio.NumeroJaNimi;
                        return View("SkannausTulos", HyvaSkannusTulos);
                    }
                }
            }

            return View("SkannausTulos", HyvaSkannusTulos);
        }

        [DisplayName("Lue tulo")]
        public async Task<IActionResult> LueTulo(int RastiId)
        {
            var rasti = await _context.Rasti.FindAsync(RastiId);
            if (rasti != null)
            {
                return View(new TagSkannausViewModel() { RastiId = RastiId, RastiNimi = rasti.NumeroJaNimi });
            }
            return BadRequest("Virheellinen RastiID");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LueTulo([Bind("RastiId, TagSerial")] TagSkannausViewModel viewModel)
        {
            bool HyvaSkannusTulos = false;
            if (ModelState.IsValid)
            {
                if (viewModel != null)
                {

                    if (!await _roleAccessStore.OikeudetRastiIdhen(viewModel.RastiId, User?.Identity?.Name))
                    {
                        return BadRequest("Ei oikeusia tähän rastiin");
                    }

                    ViewBag.RastiId = viewModel.RastiId;
                    if (viewModel.TagSerial != null)
                    {
                        var tagSkannaus = new TagSkannaus();
                        tagSkannaus.TimeStamp = DateTime.Now;
                        tagSkannaus.isTulo = true;
                        tagSkannaus.RastiId = viewModel.RastiId;

                        var vartio = _context.Vartio.FirstOrDefault(x => x.TagSerial == viewModel.TagSerial);

                        if (vartio == null)
                        {
                            return View("SkannausTulos", HyvaSkannusTulos);
                        }

                        tagSkannaus.VartioId = vartio.Id;

                        var find = _context.TagSkannaus.Where(x => x.RastiId == tagSkannaus.RastiId).Where(x => x.VartioId == tagSkannaus.VartioId).Where(x => x.isTulo == tagSkannaus.isTulo);
                        if (find.FirstOrDefault() != null)
                        {

                            var item = find.FirstOrDefault();
                            if (item == null)
                            {
                                return View("SkannausTulos", HyvaSkannusTulos);
                            }

                            item.TimeStamp = tagSkannaus.TimeStamp;

                        }
                        else
                        {
                            _context.Add(tagSkannaus);
                        }


                        _context.SaveChanges();

                        HyvaSkannusTulos = true;
                        ViewBag.SkannattuVartio = vartio.NumeroJaNimi;
                        return View("SkannausTulos", HyvaSkannusTulos);
                    }
                }
            }

            return View("SkannausTulos", HyvaSkannusTulos);
        }

        [DisplayName("Manuaalinen luku (ADMIN)")]
        public async Task<IActionResult> ManuaalinenLuku(int RastiId)
        {
            var rasti = await _context.Rasti.FindAsync(RastiId);
           if(rasti != null)
            {
                ViewBag.Rastit = _context.Rasti.ToArray();
                var vartiot = _context.Vartio.Where(x => x.KisaId == rasti.KisaId);
                
                return View(new ManuaalinenTagSkannausViewModel() { RastiId = RastiId, Vartiot = vartiot, RastiNimi = rasti.NumeroJaNimi });
            }
            return BadRequest("Virheellinen RastiId");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManuaalinenLuku([Bind("RastiId, ValittuVartioId, ValittuAika, OnkoTulo")] ManuaalinenTagSkannausViewModel viewModel)
        {
            bool HyvaSkannusTulos = false;
            if (ModelState.IsValid)
            {
                if (viewModel != null)
                {

                    if (!await _roleAccessStore.OikeudetRastiIdhen(viewModel.RastiId, User?.Identity?.Name))
                    {
                        return BadRequest("Ei oikeusia tähän rastiin");
                    }

                    var tagSkannaus = new TagSkannaus();

                    ViewBag.RastiId = viewModel.RastiId;
                    tagSkannaus.TimeStamp = DateTime.Parse(viewModel.ValittuAika, CultureInfo.GetCultureInfo("fi-FI"));
                    tagSkannaus.isTulo = viewModel.OnkoTulo;
                    tagSkannaus.RastiId = viewModel.RastiId;

                    var vartio = await _context.Vartio.FindAsync(viewModel.ValittuVartioId);

                    if (vartio == null)
                    {
                        return View("SkannausTulos", HyvaSkannusTulos);
                    }

                    tagSkannaus.VartioId = vartio.Id;


                    var find = _context.TagSkannaus.Where(x => x.RastiId == tagSkannaus.RastiId).Where(x => x.VartioId == tagSkannaus.VartioId).Where(x => x.isTulo == tagSkannaus.isTulo);
                    if (find.FirstOrDefault() != null)
                    {

                        var item = find.FirstOrDefault();
                        if (item == null)
                        {
                            return View("SkannausTulos", HyvaSkannusTulos);
                        }
                        
                        item.TimeStamp = tagSkannaus.TimeStamp;

                    }
                    else
                    {
                        _context.Add(tagSkannaus);
                    }


                    _context.SaveChanges();

                    HyvaSkannusTulos = true;
                    ViewBag.SkannattuVartio = vartio.NumeroJaNimi;
                    return View("SkannausTulos", HyvaSkannusTulos);
                }
            }

            return View("SkannausTulos", HyvaSkannusTulos);
        }

        [DisplayName("Poista skannaus")]
        public async Task<IActionResult> Poista(int SkannausId)
        {
            var skannaus = await _context.TagSkannaus.FindAsync(SkannausId);
            if (skannaus != null)
            {
                
                _context.TagSkannaus.Remove(skannaus);
                _context.SaveChanges();

                return Redirect("/Tag?RastiId=" + skannaus.RastiId);
            }
            return BadRequest("Virheellinen SkannausId");
        }
    }
}
