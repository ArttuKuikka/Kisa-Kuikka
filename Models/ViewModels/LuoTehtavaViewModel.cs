namespace Kisa_Kuikka.Models.ViewModels
{
    public class LuoTehtavaViewModel
    {
        public int KisaId { get; set; }
        public int RastiId { get; set; }
        public List<CheckboxViewModel>? Sarjat { get; set; }
        public string? Nimi { get; set; }
        public string? TehtavaJson { get; set; }


    }
}
