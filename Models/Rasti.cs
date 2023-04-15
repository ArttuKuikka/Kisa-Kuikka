using System.ComponentModel.DataAnnotations.Schema;

namespace Kisa_Kuikka.Models
{
    public class Rasti
    {
        public int? Id { get; set; }
        public int KisaId { get; set; }
        public string Nimi { get; set; }
        public int Numero { get; set; }
        public int TilanneId { get; set; }
        public bool VaadiKahdenKayttajanTarkistus { get; set; }
        public bool TarkistusKaytossa { get; set; }
        public int? tehtavaPaikat { get; set; }

        [NotMapped]
        public string NumeroJaNimi { get { return $"{Numero}. {Nimi}"; } }
        public Rasti() { VaadiKahdenKayttajanTarkistus = true; TarkistusKaytossa = true; }
        
    }
}
