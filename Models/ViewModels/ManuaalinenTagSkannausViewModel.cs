using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kipa_plus.Models.ViewModels
{
    public class ManuaalinenTagSkannausViewModel
    {
        public IQueryable<Vartio>? Vartiot { get; set; }
        public int RastiId { get; set; }
        public int ValittuVartioId { get; set; }
        [DisplayName("Aika")]
        [Required(ErrorMessage = "Tämä kenttä on pakollinen")]
        public string ValittuAika { get; set; }
        public bool OnkoTulo { get; set; }
    }
}
