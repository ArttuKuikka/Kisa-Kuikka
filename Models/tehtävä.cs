


namespace Kipa_plus.Models
{
    public class tehtävä
    {
        public int? Id { get; set; }
        public int SarjaId { get; set; }
        public int KisaId { get; set; }
        public int RastiId { get; set; }
        public string Nimi { get; set; }
        public bool Tarkistettu { get; set; }
        public string? TehtavaJson { get; set; }

    }
}
