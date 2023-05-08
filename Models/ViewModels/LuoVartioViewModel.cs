namespace Kisa_Kuikka.Models.ViewModels
{
    public class LuoVartioViewModel
    {
        public string Nimi { get; set; }
        public int Numero { get; set; }
        public int SarjaId { get; set; }
        public int KisaId { get; set; }
        public string? Lippukunta { get; set; }
        public List<Sarja>? Sarjat { get; set; }
        public string? olemassaOlevatVartiot { get; set; }
    }
}
