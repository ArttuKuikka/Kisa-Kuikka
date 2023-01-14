using Kipa_plus.Data;
using Microsoft.AspNetCore.Mvc;
using Kipa_plus.Models;

namespace Kipa_plus.Controllers
{
    public class TagController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TagController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? RastiId)
        {
            ViewBag.RastiId = RastiId;
            return View();
        }

        public IActionResult LueLahto(int RastiId)
        {
            ViewBag.Rastit = _context.Rasti.ToArray();
            return View(new TagSkannaus() { RastiId = RastiId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LueLahto([Bind("RastiId, TagSerial")] TagSkannaus tagSkannaus)
        {
            bool HyvaSkannusTulos = false;
            if(ModelState.IsValid)
            {
                if (tagSkannaus != null)
                {
                    ViewBag.RastiId = tagSkannaus.RastiId;
                    if (tagSkannaus.TagSerial != null)
                    {
                        tagSkannaus.TimeStamp = DateTime.Now.ToString();
                        tagSkannaus.isTulo = false;

                        var vartio = _context.Vartio.FirstOrDefault(x => x.TagSerial == tagSkannaus.TagSerial);

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
                            item.TagSerial = tagSkannaus.TagSerial;
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

        public IActionResult LueTulo(int RastiId)
        {
            ViewBag.Rastit = _context.Rasti.ToArray();
            return View(new TagSkannaus() { RastiId = RastiId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LueTulo([Bind("RastiId, TagSerial")] TagSkannaus tagSkannaus)
        {
            bool HyvaSkannusTulos = false;
            if (ModelState.IsValid)
            {
                if (tagSkannaus != null)
                {
                    ViewBag.RastiId = tagSkannaus.RastiId;
                    if (tagSkannaus.TagSerial != null)
                    {
                        tagSkannaus.TimeStamp = DateTime.Now.ToString();
                        tagSkannaus.isTulo = true;

                        var vartio = _context.Vartio.FirstOrDefault(x => x.TagSerial == tagSkannaus.TagSerial);

                        if (vartio == null)
                        {
                            return View("SkannausTulos", HyvaSkannusTulos); 
                        }

                        tagSkannaus.VartioId = vartio.Id;


                        var find = _context.TagSkannaus.Where(x => x.RastiId == tagSkannaus.RastiId).Where(x => x.VartioId == tagSkannaus.VartioId).Where(x => x.isTulo == tagSkannaus.isTulo);
                        if (find.FirstOrDefault() != null)
                        {
                            
                            var item = find.FirstOrDefault();
                            if(item == null)
                            {
                                return View("SkannausTulos", HyvaSkannusTulos);
                            }
                            item.TagSerial = tagSkannaus.TagSerial;
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
    }
}
