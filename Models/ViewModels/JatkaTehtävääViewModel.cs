namespace Kisa_Kuikka.Models.ViewModels
{
    public class JatkaTehtävääViewModel
    {
        public string? VartioNimi { get; set; }
        public string? TehtäväNimi { get; set; }
        public int TehtäväVastausId { get; set; }
        public string TehtäväJson { get; set; }
        public bool Kesken { get; set; }
        public int RastiId { get; set; }
    }
}
