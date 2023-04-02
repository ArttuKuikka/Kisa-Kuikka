namespace Kipa_plus.Models.ViewModels
{
    public class LuoRastiViewModel
    {
        public int KisaId { get; set; }
        public string Nimi { get; set; }
        public int Numero { get; set; }
        public int NykyinenTilanneId { get; set; }
        public bool VaadiKahdenKayttajanTarkistus { get; set; }
        public bool TarkistusKaytossa { get; set; }
        public IQueryable<Tilanne>? Tilanteet { get; set; }
    }
}
