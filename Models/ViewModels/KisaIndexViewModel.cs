namespace Kisa_Kuikka.Models.ViewModels
{
    public class KisaIndexViewModel
    {
        public Kisa Kisa { get; set; }
        public bool OikeusYhteenRastiin { get; set; }
        public Rasti? OikeusRasti { get; set; }
        public IQueryable<Tilanne> Tilanteet { get; set;}

    }
}
