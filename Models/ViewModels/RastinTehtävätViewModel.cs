﻿namespace Kisa_Kuikka.Models.ViewModels
{
    public class RastinTehtävätViewModel
    {
        public List<Tehtava>? TehtäväPohjat { get; set; }
        public List<TehtavaVastaus>?TehtavaVastausKesken { get; set; }
        public List<TehtavaVastaus>? TehtavaVastausTarkistus { get; set; }
        public List<TehtavaVastaus>? TehtavaVastausTarkistetut { get; set; }
        public int KisaId { get; set; }
        public Rasti? Rasti { get; set; }
        public List<Sarja>? Sarjat { get; set; }
        public IQueryable<Vartio>? Vartiot { get; set;}
        public bool OikeusOverrideTarkistusEsto { get; set; }
        
    }
}
