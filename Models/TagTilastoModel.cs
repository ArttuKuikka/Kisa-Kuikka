﻿namespace Kipa_plus.Models
{
    public class TagTilastoModel
    {
        public List<Rasti>? SarjanRastit { get; set; }
        public List<TagSkannaus>? Skannaukset { get; set; }

        public List<Vartio>? Vartio { get; set;}
        public int id { get; set; }
        public string? DateTimeFormat { get; set; }
    }
}
