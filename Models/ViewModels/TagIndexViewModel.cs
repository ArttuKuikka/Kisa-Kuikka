namespace Kipa_plus.Models.ViewModels
{
    public class TagIndexViewModel
    {
        public int RastiId { get; set; }
        public string? RastiNimi { get; set; }
        public List<TagSkannaus>? Skannatut { get; set; }

        public IQueryable<Vartio> Vartiot { get; set; }
    }
}
