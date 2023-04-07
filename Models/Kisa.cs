using System.ComponentModel.DataAnnotations.Schema;

namespace Kipa_plus.Models
{
    public class Kisa
    {
        public int? Id { get; set; }
        public string Nimi { get; set; }
       
        public string? LiittymisId { get; set; }

        public bool JaaTagTilastot { get; set; }

        public string? TilanneSeurantaKuvaURL { get; set; }



    }
}
