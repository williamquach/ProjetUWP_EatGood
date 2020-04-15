using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projet_EatGood_Recrutement.Classes;
using System.Net.Http;
using Newtonsoft.Json;

namespace Projet_EatGood_Recrutement.App.API
{
    public class Data_EatGood
    {
        public List<Utilisateur> lesUtilisateurs { get; set; }
        public List<Candidature> lesCandidatures { get; set; }

        HttpClient hc;

        public Data_EatGood()
        {
            lesUtilisateurs = new List<Utilisateur>();
            lesCandidatures = new List<Candidature>();

            hc = new HttpClient();
        }

        public async Task GetLesUtilisateurs()
        {
            // récupération des utilisateurs
            var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/getAllUtilisateurs.php");
            var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
            var list = donnees["results"]["utilisateurs"];
            foreach (var item in list)
            {
                Utilisateur unUser = new Utilisateur()
                {
                    IdUtilisateur = Convert.ToInt32(item["idU"].Value.ToString()),
                    Login = item["login"].Value.ToString(),
                    MotDePasse = item["motDePasse"].Value.ToString(),
                    NomUtilisateur = item["nom"].Value.ToString(),
                    PrenomUtilisateur = item["prenom"].Value.ToString(),
                    RoleUtilisateur = item["role"].Value.ToString(),
                    VilleCandidat = item["ville"].Value.ToString(),
                    DescriptionCandidat = item["descriptionCandidat"].Value.ToString()
                };
                lesUtilisateurs.Add(unUser);
            }
        }

        //public async Task SetLesCandidatures()
        //{
        //    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/getAllCandidatures.php");
        //    var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
        //    var list = donnees["results"]["candidatures"];
        //    foreach (var item in list)
        //    {
        //        Candidature uneCandidature = new Candidature()
        //        {
        //            IdCandidature = Convert.ToInt32(item["idU"].Value.ToString()),
        //            MessageMotivations = item["libelle"].Value.ToString(),
        //            StatutMessage = item["motDePasse"].Value.ToString(),
        //            MessageReponse = item["nom"].Value.ToString(),
        //            LeCandidat = ,
        //            LePosteVoulu = ,
        //            LeResto =                     
        //        };

        //        lesCandidatures.Add(uneCandidature);
        //    }
        //}
    }
}
