namespace Kipa_plus.Models
{
    public class TehtäväVastaus
    {
        public int Id { get; set; }
        public int SarjaId { get; set; }
        public int KisaId { get; set; }
        public int RastiId { get; set; }
        public int TehtäväId { get; set; }
        public bool Kesken { get; set; }
        public string? TehtavaJson { get; set; }
    }
}
