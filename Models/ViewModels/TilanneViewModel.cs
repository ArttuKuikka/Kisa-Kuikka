namespace Kipa_plus.Models.ViewModels
{
    public class TilanneViewModel
    {
        public Rasti? Rasti { get; set; }
        public Tilanne NykyinenTilanne { get; set;}
        public IQueryable<Tilanne>? Tilanteet { get; set; }

    }
}
