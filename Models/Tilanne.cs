using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kipa_plus.Models
{
    public class Tilanne
    {
        public int Id { get; set; }
        public int KisaId { get; set; }
        public string Nimi { get; set; }
        public bool TarvitseeHyvaksynnan { get; set; }
        public Tilanne() { TarvitseeHyvaksynnan = false; }
    }
}
