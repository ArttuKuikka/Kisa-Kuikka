using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kipa_plus.Models
{
    public class Tilanne
    {
        public int Id { get; set; }
        public int KisaId { get; set; }
        public string Nimi { get; set; }
        //kertoo onko tilanne sellainen että sen voi laittaa rastilta (false) vai sellainen johon tarvitsee luvan ja jonka vain kisatoimisto voi laittaa(true)
        public bool TarvitseeValtuudet { get; set; }
        public Tilanne() { TarvitseeValtuudet = false; }
    }
}
