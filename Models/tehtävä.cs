


using System.ComponentModel.DataAnnotations.Schema;

namespace Kipa_plus.Models
{
    public class Tehtava
    {
        public int Id { get; set; }
        public int SarjaId { get; set; }
        public int KisaId { get; set; }
        public int RastiId { get; set; }
        public string? Nimi { get; set; }
        public string? TehtavaJson { get; set; }

        [NotMapped]
        public IList<int>? SarjaIdt{ get; set; }

    }
}
