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
        public List<Restaurant> lesResto { get; set; }
        public List<Poste> lesPostes { get; set; }

        HttpClient hc;

        public Data_EatGood()
        {
            lesUtilisateurs = new List<Utilisateur>();
            lesCandidatures = new List<Candidature>();
            lesPostes = new List<Poste>();
            lesResto = new List<Restaurant>();
            hc = new HttpClient();
            appelFonctionCharger();
        }
        public async void appelFonctionCharger()
        {
            await ChargerLesDonnees();
        }

        public async Task ChargerLesDonnees()
        {
            await GetLesPostes();
            await Task.WhenAny(GetLesResto());
            await Task.WhenAll(GetLesUtilisateurs());
        }
        public async Task GetLesPostes()
        {
            var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/getAllPostes.php");
            var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
            var list = donnees["results"]["postes"];
            foreach (var item in list)
            {
                Poste unPoste = new Poste()
                {
                    IdPoste = Convert.ToInt32(item["idP"].Value.ToString()),
                    LibellePoste = item["libelle"].Value.ToString()
                };
                lesPostes.Add(unPoste);
            }
        }
        public async Task GetLesResto()
        {
            var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/getAllRestaurants.php");
            var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
            var list = donnees["results"]["restaurants"];
            foreach (var item in list)
            {
                Restaurant unResto = new Restaurant()
                {
                    IdResto = Convert.ToInt32(item["idR"].Value.ToString()),
                    LibelleResto = item["libelle"].Value.ToString(),
                    NumeroRueResto = item["nRue"].Value.ToString(),
                    NomRueResto = item["nomRue"].Value.ToString(),
                    VilleResto = item["ville"].Value.ToString(),
                    CodePostalResto = item["codePostal"].Value.ToString(),
                    NumTelResto = item["numTel"].Value.ToString(),
                };
                lesResto.Add(unResto);
            }
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
                };
                if (item["descriptionCandidat"].Value != null)
                {
                    unUser.DescriptionCandidat = item["descriptionCandidat"].Value.ToString();
                }
                lesUtilisateurs.Add(unUser);
            }
        }

        public async Task GetLesCandidaturesByCandidat(int idCandidat)
        {
            var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/getAllCandidatures.php?action=getCandidatures&idCandidat=" + idCandidat);
            var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
            var list = donnees["results"]["candidatures"];
            foreach (var item in list)
            {
                Candidature uneCandidature = new Candidature()
                {
                    IdCandidature = Convert.ToInt32(item["idU"].Value.ToString()),
                    MessageMotivations = item["libelle"].Value.ToString(),
                    StatutMessage = item["motDePasse"].Value.ToString(),
                    MessageReponse = item["nom"].Value.ToString()
                    //LeCandidat = ,
                    //LePosteVoulu = ,
                    //LeResto =
                };

                lesCandidatures.Add(uneCandidature);
            }
        }


    }
}
