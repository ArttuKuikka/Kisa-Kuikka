using Microsoft.AspNetCore.Identity;

namespace Kipa_plus.Models
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
        public string? JatkajaUserId { get; set; }
        public string? TarkistajaUserId { get; set; }
    }
}
