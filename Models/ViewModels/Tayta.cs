using Microsoft.EntityFrameworkCore.Query;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kisa_Kuikka.Models.ViewModels
{
    public class Tayta
    {
        public int? TehtavaId { get; set; }
        [NotMapped]
        public int? RastiId { get; set; }
        [NotMapped]
        public string? RastiNimi { get; set; }
        public string? Nimi { get; set; }
        public int VartioId { get; set; }
        public bool Kesken { get; set; }
        public string? PohjaJson { get; set; }
        public List<Vartio>? VartioList { get; set;}
    }
}
