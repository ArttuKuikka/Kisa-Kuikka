namespace Kisa_Kuikka.Models.ViewModels
{
    public class TilanneViewModel
    {
        public Rasti? Rasti { get; set; }
        public Tilanne NykyinenTilanne { get; set;}
        public IQueryable<Tilanne>? Tilanteet { get; set; }

    }
}
