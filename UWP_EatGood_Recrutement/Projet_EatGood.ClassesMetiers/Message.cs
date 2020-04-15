using System;
using System.Collections.Generic;
using System.Text;

namespace Projet_EatGood_Recrutement.ClassesMetiers
{
    public class Message
    {
        public int IdMessage { get; set; }
        public string ContenuMessage { get; set; }
        public int StatutMessage { get; set; }
        public Utilisateur LeDestinataire{ get; set; }
        public Restaurant LExpediteur { get; set; }

    }
}
