using System.ComponentModel.DataAnnotations.Schema;

namespace Kipa_plus.Models.ViewModels
{
    public class MuokkaaVastaustaViewModel
    {
        public string TehtavaJson { get; set; }
        public string? VartionNumeroJaNimi { get; set; }
        
        public string? TehtavaNimi { get; set; }
        public int VartioId { get; set; }
        public int TehtavaId { get; set; }
        [NotMapped]
        public int RastiId { get; set; }

    }
}
