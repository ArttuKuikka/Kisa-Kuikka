namespace Kisa_Kuikka.Models.ViewModels
{
    public class ListaaRastitViewModel
    {
        public int KisaId { get; set; }
        public IEnumerable<Rasti>? Rastit { get; set; }

        public IQueryable<Tilanne>? Tilanteet { get; set;}

    }
}
