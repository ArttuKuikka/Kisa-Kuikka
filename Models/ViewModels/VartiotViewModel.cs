namespace Kisa_Kuikka.Models.ViewModels
{
    public class VartiotViewModel
    {
        public IEnumerable<Vartio>? Vartiot { get; set; }

        public IEnumerable<Sarja>? Sarjat { get; set; }

        public int? KisaId { get; set; }
    }
}
