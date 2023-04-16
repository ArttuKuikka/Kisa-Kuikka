using Microsoft.AspNetCore.Identity;

namespace Kisa_Kuikka.Models
{
    public class TehtavaVastaus
    {
        public int Id { get; set; }
        public int VartioId { get; set; }
        public int SarjaId { get; set; }
        public int KisaId { get; set; }
        public int RastiId { get; set; }
        public int TehtavaId { get; set; }
        public bool Kesken { get; set; }
        public string? TehtavaJson { get; set; }
        public bool Tarkistettu { get; set; }
        public string? TäyttäjäUserId { get; set; }
        public string? JatkajatJson { get; set; }
        public string? TarkistajaUserId { get; set; }
        public DateTime TäyttämisAika { get; set; }
        public DateTime? TarkistusAika { get; set; }
    }
}
