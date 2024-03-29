﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Kisa_Kuikka.Models
{
    public class Vartio
    {
        public int? Id { get; set; }
        public string Nimi { get; set; }
        public int Numero { get; set; }
        public int SarjaId { get; set; }
        public int KisaId { get; set; }
        public string? Lippukunta { get; set; }
        [NotMapped]
        public string NumeroJaNimi { get { return Numero + " " + Nimi; } }
        [NotMapped]
        public string NumeroJaNimiJaTilanne { get { var teksti = Numero + " " + Nimi; if (Keskeytetty) { teksti += " (Keskeyttänyt)"; } return teksti; } }
        public int? Tilanne { get; set; } //0 = kisassa, 1 = Keskeytetty, 2 = ulkopuolella
        public string? TagSerial { get; set; }
        public bool Keskeytetty { get; set; }

        public Vartio() 
        { 
            Keskeytetty = false;
        }
    }
}
