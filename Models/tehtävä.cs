


namespace Kipa_plus.Models
{
    public class tehtävä
    {
        public int? Id { get; set; }
        public int? sarjaId { get; set; }
        public string? Nimi { get; set; }
        public int järjestysnumero { get; set; }
        public string lyhenne { get; set; }
        public string Rastikäsky { get; set; }
        public bool tarkistettu { get; set; }

    }
}
