namespace Kipa_plus.Models.ViewModels
{
    public class VartiotViewModel
    {
        public IEnumerable<Vartio>? Vartiot { get; set; }

        public IEnumerable<Sarja>? Sarjat { get; set; }

        public int? KisaId { get; set; }
    }
}
