using System;
using System.Collections.Generic;
using System.Text;

namespace Projet_EatGood.ModelMetier
{
    public class Candidature
    {
        public int IdCandidature { get; set; }
        public Utilisateur LeCandidat { get; set; }
        public Restaurant LeResto { get; set; }
        public Poste LePosteVoulu { get; set; }
        public string MessageMotivations { get; set; }
        public string StatutMessage { get; set; }
        public string MessageReponse { get; set; }

    }
}
