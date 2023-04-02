﻿using Kipa_plus.Data;
using Kipa_plus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Kipa_plus.Controllers
{
    [Route("[controller]")]
    [DisplayName("Tilanneseuranta")]
    [Static]
    public class TagTilastotController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TagTilastotController(ApplicationDbContext context)
        {
            _context = context;
        }
        [DisplayName("vanha tilanneseuranta näkymä")]
        [HttpGet("Vanha")]
        public IActionResult Vanha(int kisaId)
        {
            var id = kisaId;

            var rastit = _context.Rasti.Where(r => r.KisaId == id).ToList();
            var skannaukset = _context.TagSkannaus.ToList();
            var vartiot = _context.Vartio.Where(x => x.KisaId == id).ToList();

            string datetimeformat = "HH.mm";

            var cookie = Request.Cookies["datetimeformat"];
            if (cookie != null)
            {
                switch (cookie)
                {
                    case "1":
                        {
                            datetimeformat = "HH.mm";
                            break;
                        }
                    case "2":
                        {
                            datetimeformat = "dd.MM.yyyy HH.mm";
                            break;
                        }
                    case "3":
                        {
                            datetimeformat = "dd.MM HH.mm";
                            break;
                        }
                    case "4":
                        {
                            datetimeformat = "dd HH.mm";
                            break;
                        }
                    default:
                        datetimeformat = "HH.mm";
                        break;
                }
            }

            return View("Vanha", new TagTilastoModel() { SarjanRastit = rastit, Skannaukset = skannaukset, Vartio = vartiot, id = (int)id, DateTimeFormat = datetimeformat, Sarja = _context.Sarja.ToList() });
        }




        [DisplayName("Etusivu (taulukko)")]
        public async Task<IActionResult> Index(int kisaId)
        {
            var kisa = await _context.Kisa.FindAsync(kisaId);
            if (kisa != null)
            {
                var viewModel = new Models.ViewModels.TilanneseurantaTaulukkoViewModel() { Kisa = kisa};

                //pää array
                var MainArray = new JArray();

                //sarjat ja rastit listat
                var sarjat = _context.Sarja.Where(x => x.KisaId == kisaId).ToList();
                var rastit = _context.Rasti.Where(x => x.KisaId == kisaId).ToList();

                //rastien nimet
                var RastiNimetArray = new JArray() { " " };

                foreach(var rasti in rastit)
                {
                    RastiNimetArray.Add(rasti.Nimi);
                }
                viewModel.Headers = RastiNimetArray.ToString();



                foreach(var sarja in sarjat)
                {

                    //rastien numero ja vapaat paikat
                    bool sarjaRivi = true;
                    var RastiNumeroArray = new JArray() { sarja.Nimi };

                    foreach (var rasti in rastit)
                    {
                        RastiNumeroArray.Add(rasti.Id); //vaihda numeroon
                    }
                    RastiNumeroArray.Add(sarjaRivi);
                    MainArray.Add(RastiNumeroArray);

                    

                    var vartiotSarjassa = _context.Vartio.Where(x => x.KisaId == kisaId).Where(x => x.SarjaId == sarja.Id);
                    //kaiken datan luonti
                    foreach(var vartio in vartiotSarjassa)
                    {
                        var vartioobject = new JObject
                        {
                            { "Nimi", vartio.NumeroJaNimi },
                            {"Keskeytetty", vartio.Keskeytetty }
                        };
                        var VartionArray = new JArray() { vartioobject }; 
                        foreach(var rasti in rastit)
                        {
                            var dataelement = new JObject();
                            var skannaukset = _context.TagSkannaus.Where(x => x.RastiId == rasti.Id)?.Where(x => x.VartioId == vartio.Id);
                            if(skannaukset != null)
                            {
                                //0 - ei mitään
                                //1 - pelkkä lähtö
                                //2 - pelkkä tulo
                                //3 - lähtö ja tulo
                                var tulo = skannaukset.Where(x => x.isTulo == true).FirstOrDefault();
                                var lähtö = skannaukset.Where(x => x.isTulo == false).FirstOrDefault();
                                if(tulo != null && lähtö != null)
                                {
                                    dataelement.Add("Numero",3);
                                }
                                else if(tulo != null)
                                {
                                    dataelement.Add("Numero", 2);
                                }
                                else if(lähtö != null)
                                {
                                    dataelement.Add("Numero", 1);
                                }
                                else
                                {
                                    dataelement.Add("Numero", 0);
                                }
                                dataelement.Add("Tulo", tulo?.TimeStamp);
                                dataelement.Add("Lahto", lähtö?.TimeStamp);
                            }
                            else
                            {
                                dataelement.Add("Numero", 0);
                            }
                            VartionArray.Add(dataelement);
                        }
                        MainArray.Add(VartionArray);
                    }
                }

                

                viewModel.Json = MainArray.ToString();
                return View(viewModel);
            }

            return BadRequest();
        }

    }
}
