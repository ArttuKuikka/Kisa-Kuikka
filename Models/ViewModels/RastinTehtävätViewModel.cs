namespace Kipa_plus.Models.ViewModels
{
    public class RastinTehtävätViewModel
    {
        public List<Tehtava>? TehtäväPohjat { get; set; }
        public List<TehtavaVastaus>?TehtavaVastausKesken { get; set; }
        public List<TehtavaVastaus>? TehtavaVastausTarkistus { get; set; }
        public List<TehtavaVastaus>? TehtavaVastausTarkistetut { get; set; }
        public int KisaId { get; set; }
        public int? RastiId { get; set; }
        public List<Sarja>? Sarjat { get; set; }
    }
}
