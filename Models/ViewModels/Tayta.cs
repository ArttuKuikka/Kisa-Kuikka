using Microsoft.EntityFrameworkCore.Query;

namespace Kipa_plus.Models.ViewModels
{
    public class Tayta
    {
        public int? TehtavaId { get; set; }
        public string? Nimi { get; set; }
        public int VartioId { get; set; }
        public bool Kesken { get; set; }
        public string? PohjaJson { get; set; }
        public List<Vartio>? VartioList { get; set;}
    }
}
