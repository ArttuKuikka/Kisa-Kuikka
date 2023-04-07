using Kipa_plus.Data;
using Kipa_plus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

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
                rastit.Sort((p1, p2) => p1.Numero.CompareTo(p2.Numero));

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
                        RastiNumeroArray.Add(rasti.Numero);
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




                        //seuraavan rastin tunnistus
                        //luo lista json perusteella rastien järjestyksestä jossa ne kuuluisi mennä
                        var uudetrastit = _context.Rasti.Where(x => x.KisaId == sarja.KisaId).ToList();

                        var rastitJärjestyksessä = new List<Rasti>();
                        if (sarja.RastienJarjestysJSON != null)
                        {
                            foreach (var Jrasti in JArray.Parse(sarja.RastienJarjestysJSON))
                            {
                                var success = int.TryParse(Jrasti["id"]?.ToString(), out var parsedid);
                                if (success)
                                {
                                    var findrasti = await _context.Rasti.FindAsync(parsedid);
                                    if (findrasti != null)
                                    {
                                        rastitJärjestyksessä.Add(findrasti);
                                        uudetrastit.Remove(findrasti);
                                    }
                                }
                            }
                        }
                        //jos puuttuu uusia rasteja lisää ne loppuun
                        uudetrastit.ForEach(x => rastitJärjestyksessä.Add(x));

                        var rastitJärjestyksessäKaikki = rastitJärjestyksessä.ToList();


                        //rastit jotka vartio on suorittanu 
                        var suoritetutrastitSkannaukset = _context.TagSkannaus.Where(x => x.VartioId == vartio.Id).Where(x => x.isTulo == false).ToList();
                        suoritetutrastitSkannaukset = suoritetutrastitSkannaukset.OrderByDescending(x => x.TimeStamp).ToList();
                        var suoritetutRastit = new List<Rasti>();
                        foreach (var suoritus in suoritetutrastitSkannaukset)
                        {
                            var findrasti = await _context.Rasti.FindAsync(suoritus.RastiId);
                            if (findrasti != null)
                            {
                                suoritetutRastit.Add(findrasti);
                            }
                        }

                        //kaikki rastit
                        var kaikkirastit = _context.Rasti.Where(x => x.KisaId == kisa.Id).ToList();

                        //tee järjestys ja käydyt listat sellaiset että niissä on vain samat rastit
                        foreach (var rastiTEMPVAR in suoritetutRastit)
                        {
                            kaikkirastit.Remove(rastiTEMPVAR);
                        }
                        foreach (var RastiTEMPVAR in kaikkirastit)
                        {
                            rastitJärjestyksessä.Remove(RastiTEMPVAR);
                        }

                       

                        //tarkistaa matchaako suoritetutRastit järjestyt siihen mitä sen pitäis olla
                        var onkoJärjestysOikein = rastitJärjestyksessä.SequenceEqual(suoritetutRastit);
                        rastitJärjestyksessäKaikki.Reverse();
                        Rasti? seuraavaRasti = null;
                        bool varmastioikein = false;
                        if (onkoJärjestysOikein)
                        {
                            //tarkista että järjestys on json mukaan oikein 
                            rastitJärjestyksessä.Reverse();
                            var eka = rastitJärjestyksessä.FirstOrDefault();
                            if(eka != null)
                            {
                                var ekaindex = rastitJärjestyksessäKaikki.IndexOf(eka);
                                var listaJossaKaikkiOnOikein = new List<Rasti>();
                                for (int i = ekaindex; i < rastitJärjestyksessä.Count; i++)
                                {
                                    listaJossaKaikkiOnOikein.Add(rastitJärjestyksessäKaikki[i]);
                                }
                                suoritetutRastit.Reverse();
                                if(listaJossaKaikkiOnOikein.SequenceEqual(suoritetutRastit))
                                {
                                    varmastioikein = true;
                                }
                            }
                            foreach (var itemi in suoritetutRastit)
                            {
                                rastitJärjestyksessäKaikki.Remove(itemi);
                            }
                            
                            seuraavaRasti = rastitJärjestyksessäKaikki.FirstOrDefault();
                        }






                        foreach (var rasti in rastit)
                        {
                            var dataelement = new JObject();
                            var skannaukset = _context.TagSkannaus.Where(x => x.RastiId == rasti.Id)?.Where(x => x.VartioId == vartio.Id);
                            if(skannaukset != null)
                            {

                                if(seuraavaRasti != null && rasti == seuraavaRasti && rastitJärjestyksessä.Count > 0 && varmastioikein)
                                {
                                    dataelement.Add("Numero", 4);
                                }
                                else
                                {
                                    //0 - ei mitään
                                    //1 - pelkkä lähtö
                                    //2 - pelkkä tulo
                                    //3 - lähtö ja tulo
                                    //4 - seuraava rasti
                                    var tulo = skannaukset.Where(x => x.isTulo == true).FirstOrDefault();
                                    var lähtö = skannaukset.Where(x => x.isTulo == false).FirstOrDefault();
                                    if (tulo != null && lähtö != null)
                                    {
                                        dataelement.Add("Numero", 3);
                                    }
                                    else if (tulo != null)
                                    {
                                        dataelement.Add("Numero", 2);
                                    }
                                    else if (lähtö != null)
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
