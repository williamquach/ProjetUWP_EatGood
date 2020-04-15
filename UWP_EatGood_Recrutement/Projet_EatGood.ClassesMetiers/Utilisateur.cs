using System;
using System.Collections.Generic;
using System.Text;

namespace Projet_EatGood_Recrutement.ClassesMetiers
{
    public class Utilisateur
    {
        public int IdUtilisateur { get; set; }
        public string Login { get; set; }
        public string MotDePasse { get; set; }
        public string NomUtilisateur { get; set; }
        public string PrenomUtilisateur { get; set; }
        public string DescriptionCandidat { get; set; }
        public string RoleUtilisateur { get; set; }
        public string VilleCandidat { get; set; }
        public List<Message> MesMessages { get; set; }
        public List<Candidature> MesCandidatures { get; set; }
    }
}
