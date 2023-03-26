using System.ComponentModel.DataAnnotations.Schema;

namespace Kipa_plus.Models
{
    public class Rasti
    {
        public int? Id { get; set; }
        public int KisaId { get; set; }
        public string Nimi { get; set; }
        public int nykyinenTilanneId { get; set; }
        public int? edellinenTilanneId { get; set; }
        public bool OdottaaTilanneHyvaksyntaa { get; set; }
        public bool VaadiKahdenKayttajanTarkistus { get; set; }
        public bool TarkistusKaytossa { get; set; }

        public Rasti() { OdottaaTilanneHyvaksyntaa = false; VaadiKahdenKayttajanTarkistus = true; TarkistusKaytossa = true; }
        
    }
}
