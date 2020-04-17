using System;
using System.Collections.Generic;
using System.Text;

namespace Projet_EatGood_Recrutement.Classes
{
    public class Message
    {
        public int IdMessage { get; set; }
        public string ContenuMessage { get; set; }
        public string StatutMessage { get; set; }
        public Utilisateur LeDestinataire{ get; set; }
        public Restaurant LExpediteur { get; set; }

    }
}
