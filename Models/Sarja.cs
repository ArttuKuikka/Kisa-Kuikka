using System.ComponentModel.DataAnnotations.Schema;

namespace Kisa_Kuikka.Models
{
    public class Sarja
    {
        public int? Id { get; set; }
        public string Nimi { get; set; }
        public int KisaId { get; set; }
        public int Numero { get; set; }
        public int? VartionMaksimiko { get; set; }
        public int? VartionMinimikoko { get; set; }
        public string? RastienJarjestysJSON { get; set; }

        [NotMapped]
        public string NimiJaKokoNumero { get { return $"{Nimi} ({Numero}00)"; } }


    }
}
