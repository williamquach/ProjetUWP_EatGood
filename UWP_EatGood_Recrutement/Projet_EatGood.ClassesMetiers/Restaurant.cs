using System;
using System.Collections.Generic;
using System.Text;

namespace Projet_EatGood_Recrutement.ClassesMetiers
{
    public class Restaurant
    {
        public int IdResto { get; set; }
        public string LibelleResto { get; set; }
        public string NumeroRueResto { get; set; }
        public string NomRueResto { get; set; }
        public string VilleResto { get; set; }
        public string CodePostalResto { get; set; }
        public string NumTelResto { get; set; }
        public Dictionary<string, int> lesPostes { get; set; }
        public List<Message> LesMessagesEnvoyes { get; set; }
    }
}
