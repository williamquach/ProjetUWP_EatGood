using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projet_EatGood_Recrutement.Classes;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.VisualBasic.CompilerServices;

namespace Projet_EatGood_Recrutement.App.API
{
    public class Data_EatGood
    {
        public Utilisateur LUtilisateurDeMaintenant { get; set; }
        public List<Utilisateur> lesUtilisateurs { get; set; }
        public List<Candidature> lesCandidaturesDuCandidatActuel { get; set; }
        public List<Restaurant> lesResto { get; set; }
        public List<Poste> lesPostes { get; set; }
        public List<Candidature> AllCandidatures { get; set; }

        HttpClient hc;
        HttpClient hc2;

        public Data_EatGood()
        {
            hc = new HttpClient();
            hc2 = new HttpClient();
            appelFonctionCharger();
        }
        public void SetLutilisateurDeMaintenant(Utilisateur util)
        {
            LUtilisateurDeMaintenant = util;
        }
        public async void appelFonctionCharger()
        {
            await ChargerLesDonnees();
        }

        public async Task ChargerLesDonnees()
        {
            await GetLesResto();
            await Task.WhenAny(GetLesUtilisateurs());
            await Task.WhenAll(ChargerLesPostes());
        }
        public async Task ChargerLesPostes()
        {
            await GetLesPostes();
            await SetAllCandidature();
        }
        public async Task SetAllCandidature()
        {
            await GetAllCandidature();
        }
        public async Task GetLesPostes()
        {
            lesPostes = new List<Poste>();

            var reponse = await hc2.GetStringAsync("http://localhost/recru_eatgood_api/getAllPostes.php");
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
            lesResto = new List<Restaurant>();

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
            lesUtilisateurs = new List<Utilisateur>();
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
                    FullName = item["nom"].Value.ToString() + " " + item["prenom"].Value.ToString()
                };
                if (item["descriptionCandidat"].Value != null)
                {
                    unUser.DescriptionCandidat = item["descriptionCandidat"].Value.ToString();
                }
                lesUtilisateurs.Add(unUser);
            }
        }

        public async Task<List<Candidature>> GetLesCandidaturesByCandidat(int idCandidat)
        {
            List<Candidature> lesCandidatures = new List<Candidature>();
            var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/getCandidaturesByCandidat.php?action=getCandidatures&idCandidat=" + idCandidat);
            var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
            var list = donnees["results"]["candidatures"];
            if (list != null)
            {
                foreach (var item in list)
                {
                    Restaurant leResto = GetUnRestoById(Convert.ToInt32(item["codeRestaurant"].Value.ToString()));
                    Poste lePoste = GetUnPosteById(Convert.ToInt32(item["codePoste"].Value.ToString()));
                    Utilisateur lUtilisateur = GetUnUtilisateurById(Convert.ToInt32(item["codeCandidat"].Value.ToString()));
                    Candidature uneCandidature = new Candidature()
                    {
                        IdCandidature = Convert.ToInt32(item["idC"].Value.ToString()),
                        MessageMotivations = item["messageMotivations"].Value.ToString(),
                        StatutMessage = item["statut"].Value.ToString(),
                        LeCandidat = lUtilisateur,
                        LePosteVoulu = lePoste,
                        LeResto = leResto
                    };
                    if (item["messageReponse"].Value != null)
                    {
                        uneCandidature.MessageReponse = "";
                    }

                    lesCandidatures.Add(uneCandidature);
                }
            }
            return lesCandidatures;
        }

        public async Task<List<Message>> GetLesMessagesDuCandidat(int idCandidat)
        {
            List<Message> lesMessages = new List<Message>();
            var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/getMessagesByUser.php?" +
                                                  "action=getMessagesByCandidat&" +
                                                  "idCandidat=" + idCandidat);
            var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
            var list = donnees["results"]["messages"];
            if (list != null)
            {
                foreach (var item in list)
                {
                    Utilisateur lUtilisateur = GetUnUtilisateurById(Convert.ToInt32(item["codeCandidat"].Value.ToString()));
                    Restaurant leResto = GetUnRestoById(Convert.ToInt32(item["codeRestaurant"].Value.ToString()));
                    Message unMessage = new Message()
                    {
                        IdMessage = Convert.ToInt32(item["idM"].Value.ToString()),
                        ContenuMessage = item["message"].Value.ToString(),
                        LExpediteur = leResto,
                        StatutMessage = item["statutMsg"].Value.ToString(),
                        LeDestinataire = lUtilisateur
                    };

                    lesMessages.Add(unMessage);
                }
            }
            return lesMessages;
        }
        // ===================================================================================================================================================
        //  FONCTION SYNCHRONE ou non async en tout cas (fonctions simples)
        // ===================================================================================================================================================
        public Restaurant GetUnRestoById(int idResto)
        {
            Restaurant leResto = new Restaurant();
            foreach(Restaurant r in lesResto)
            {
                if(r.IdResto == idResto)
                {
                    leResto = r;
                    break;
                }
            }
            return leResto;
        }
        public Poste GetUnPosteById(int idPoste)
        {
            Poste lePoste = new Poste();
            foreach (Poste p in lesPostes)
            {
                if (p.IdPoste == idPoste)
                {
                    lePoste = p;
                    break;
                }
            }
            return lePoste;
        }
        public Utilisateur GetUnUtilisateurById(int idUtilisateur)
        {
            Utilisateur lUtilisateur = new Utilisateur();
            foreach (Utilisateur u in lesUtilisateurs)
            {
                if (u.IdUtilisateur == idUtilisateur)
                {
                    lUtilisateur = u;
                    break;
                }
            }
            return lUtilisateur;
        }
        public List<string> GetLesNomResto()
        {
            List<string> lesRestoLibelles = new List<string>();
            foreach (Restaurant r in lesResto)
            {
                lesRestoLibelles.Add(r.LibelleResto);
            }
            return lesRestoLibelles;
        }
        // =============================================================================================================


        // RECRUTEUR
        public async Task GetAllCandidature()
        {
            AllCandidatures = new List<Candidature>();
            var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/getAllCandidatures.php");
            var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
            var list = donnees["results"]["candidatures"];
            if (list != null)
            {
                foreach (var item in list)
                {
                    Restaurant leResto = GetUnRestoById(Convert.ToInt32(item["codeRestaurant"].Value.ToString()));
                    Poste lePoste = GetUnPosteById(Convert.ToInt32(item["codePoste"].Value.ToString()));
                    Utilisateur lUtilisateur = GetUnUtilisateurById(Convert.ToInt32(item["codeCandidat"].Value.ToString()));
                    Candidature uneCandidature = new Candidature()
                    {
                        IdCandidature = Convert.ToInt32(item["idC"].Value.ToString()),
                        MessageMotivations = item["messageMotivations"].Value.ToString(),
                        StatutMessage = item["statut"].Value.ToString(),
                        LeCandidat = lUtilisateur,
                        LePosteVoulu = lePoste,
                        LeResto = leResto
                    };
                    if (item["messageReponse"].Value != null)
                    {
                        uneCandidature.MessageReponse = item["messageReponse"].Value.ToString();
                    }
                    else
                    {
                        uneCandidature.MessageReponse = "";
                    }
                    AllCandidatures.Add(uneCandidature);
                    //lesCandidatures.Add(uneCandidature);
                }
            }
            //return lesCandidatures;
        }
    }
}
