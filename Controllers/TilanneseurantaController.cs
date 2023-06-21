using Kisa_Kuikka.Data;
using Kisa_Kuikka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace Kisa_Kuikka.Controllers
{
    [Route("[controller]")]
    [DisplayName("Tilanneseuranta")]
    [Static]
    public class TilanneseurantaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TilanneseurantaController(ApplicationDbContext context)
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



        [HttpGet("/TilanneSeuranta")]
        [DisplayName("Vartioiden seuranta(etusivu)")]
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
                var rastit = _context.Rasti.Where(x => x.KisaId == kisaId).Where(x => x.PiilotaTilanneseurannasta == false).ToList();
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
                        if(rasti.tehtavaPaikat != null)
                        {
                            var rastilla = _context.TagSkannaus.Where(x => x.RastiId == rasti.Id).Where(x => x.isTulo == true)?.Count();
                            RastiNumeroArray.Add($"{rasti.Numero} ({rastilla}/{rasti.tehtavaPaikat})");
                        }
                        else
                        {
                            RastiNumeroArray.Add(rasti.Numero);
                        }
                        
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
                            {"Keskeytetty", vartio.Keskeytetty },
                            {"Lippukunta", vartio.Lippukunta }
                        };
                        var VartionArray = new JArray() { vartioobject };




                        //seuraavan rastin tunnistus
                        //luo lista json perusteella rastien järjestyksestä jossa ne kuuluisi mennä
                        var uudetrastit = _context.Rasti.Where(x => x.KisaId == sarja.KisaId).Where(x => x.PiilotaTilanneseurannasta == false).ToList();

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
                        


                        //rastit jotka vartio on suorittanu 
                        var suoritetutrastitSkannaukset = _context.TagSkannaus.Where(x => x.VartioId == vartio.Id).ToList();
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
                        suoritetutRastit.Reverse();

                        Rasti? seuraavaRasti = null;
                        //poista duplicatet
                        suoritetutRastit = suoritetutRastit.Distinct().ToList();
                        var eka = suoritetutRastit.FirstOrDefault();
                        if(eka != null)
                        {
                            var ekaindex = rastitJärjestyksessä.IndexOf(eka);

                            var lista = new List<Rasti>();
                            for (int i = ekaindex; i < suoritetutRastit.Count; i++)
                            {
                                lista.Add(rastitJärjestyksessä[i]);
                            }
                            //tarkista onko järjestys sama
                            if (lista.SequenceEqual(suoritetutRastit))
                            {
                                var pos = ekaindex + suoritetutRastit.Count;
                                if (rastitJärjestyksessä.ElementAtOrDefault(pos) != null)
                                {
                                    seuraavaRasti = rastitJärjestyksessä[pos];
                                }
                            }

                        }






                        foreach (var rasti in rastit)
                        {
                            var dataelement = new JObject();
                            var skannaukset = _context.TagSkannaus.Where(x => x.RastiId == rasti.Id)?.Where(x => x.VartioId == vartio.Id);
                            if(skannaukset != null)
                            {

                                if(seuraavaRasti != null && seuraavaRasti == rasti)
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

        [HttpGet("/TilanneSeuranta/TulostenSyotto")]
        [DisplayName("Tulosten syötön seuranta")]
        public async Task<IActionResult> TulostenSyotto(int kisaId)
        {
            var kisa = await _context.Kisa.FindAsync(kisaId);
            if (kisa != null)
            {
                var viewModel = new Models.ViewModels.TilanneseurantaTaulukkoViewModel() { Kisa = kisa };

                //pää array
                var MainArray = new JArray();

                //sarjat ja rastit listat
                var sarjat = _context.Sarja.Where(x => x.KisaId == kisaId).ToList();
                var rastit = _context.Rasti.Where(x => x.KisaId == kisaId).Where(x => x.PiilotaTilanneseurannasta == false).ToList();
                rastit.Sort((p1, p2) => p1.Numero.CompareTo(p2.Numero));

                //rastien nimet
                var RastiNimetArray = new JArray() { " " };

                foreach (var rasti in rastit)
                {
                    RastiNimetArray.Add(rasti.Nimi);
                }
                viewModel.Headers = RastiNimetArray.ToString();



                foreach (var sarja in sarjat)
                {

                    //rastien numero ja vapaat paikat
                    bool sarjaRivi = true;
                    var RastiNumeroArray = new JArray() { sarja.Nimi };

                    foreach (var rasti in rastit)
                    {
                        if (rasti.tehtavaPaikat != null)
                        {
                            var rastilla = _context.TagSkannaus.Where(x => x.RastiId == rasti.Id).Where(x => x.isTulo == true)?.Count();
                            RastiNumeroArray.Add($"{rasti.Numero} ({rastilla}/{rasti.tehtavaPaikat})");
                        }
                        else
                        {
                            RastiNumeroArray.Add(rasti.Numero);
                        }

                    }
                    RastiNumeroArray.Add(sarjaRivi);
                    MainArray.Add(RastiNumeroArray);



                    var vartiotSarjassa = _context.Vartio.Where(x => x.KisaId == kisaId).Where(x => x.SarjaId == sarja.Id);
                    //kaiken datan luonti
                    foreach (var vartio in vartiotSarjassa)
                    {
                        var vartioobject = new JObject
                        {
                            { "Nimi", vartio.NumeroJaNimi },
                            {"Keskeytetty", vartio.Keskeytetty },
                            {"Lippukunta", vartio.Lippukunta }
                        };
                        var VartionArray = new JArray() { vartioobject };

                        foreach (var rasti in rastit)
                        {
                            var dataelement = new JObject();
                            // !!!!!!!!! RASTEILLA VOI OLLA MONTA TEHTÄVÄÄ, korjaa
                            //tee ehk 4 numero ja joku muu väri ja laita sitten ruutun kuinka monta monesta on tehty esim 2/4 
                            var vastaus = _context.TehtavaVastaus.Where(x => x.RastiId == rasti.Id)?.Where(x => x.VartioId == vartio.Id).FirstOrDefault();
                            if (vastaus != null)
                            {

                                //0 - ei mitään
                                //1 - tulos odottaa jatkamista
                                //2 - tulos odottaa tarkistusta
                                //3 - tulos tarkistettu

                                
                                if (vastaus.Tarkistettu)
                                {
                                    dataelement.Add("Numero", 3);
                                }
                                else if (!vastaus.Tarkistettu && !vastaus.Kesken)
                                {
                                    dataelement.Add("Numero", 2);
                                }
                                else if (vastaus.Kesken)
                                {
                                    dataelement.Add("Numero", 1);
                                }
                                else
                                {
                                    dataelement.Add("Numero", 0);
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
