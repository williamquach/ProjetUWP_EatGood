using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Projet_EatGood_Recrutement.App.API;
using Projet_EatGood_Recrutement.App.Pages.Candidat;
using Projet_EatGood_Recrutement.Classes;
using System.Net.Http;
using Newtonsoft.Json;
using Windows.UI.Popups;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projet_EatGood_Recrutement.App.Pages.Candidat
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Page_Profil : Page
    {
        public Page_Profil()
        {
            this.InitializeComponent();
        }
        Utilisateur lutilisateurActuellement;
        HttpClient hc;
        Data_EatGood lesDonnees;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                lesDonnees = e.Parameter as Data_EatGood;
                lutilisateurActuellement = lesDonnees.LUtilisateurDeMaintenant;
                txtBienvenue.Text +=  lutilisateurActuellement.PrenomUtilisateur + " " + lutilisateurActuellement.NomUtilisateur;
            }
            base.OnNavigatedTo(e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            hc = new HttpClient();
            if (lutilisateurActuellement != null)
            {
                txtMdp.Password = lutilisateurActuellement.MotDePasse;
                txtNom.Text = lutilisateurActuellement.NomUtilisateur;
                txtPrenom.Text = lutilisateurActuellement.PrenomUtilisateur;
                txtVille.Text = lutilisateurActuellement.VilleCandidat;
                txtDescription.Text = lutilisateurActuellement.DescriptionCandidat;
            }
        }
        private async void BtnModifierProfil_Click(object sender, RoutedEventArgs e)
        {
            if (txtNom.Text == "")
            {
                var message = new MessageDialog("Veuillez entrer un nom.");
                await message.ShowAsync();
            }
            else if (txtPrenom.Text == "")
            {
                var message = new MessageDialog("Veuillez entrer un prénom.");
                await message.ShowAsync();
            }
            else if (txtMdp.Password == "")
            {
                var message = new MessageDialog("Veuillez entrer un mot de passe.");
                await message.ShowAsync();
            }
            else if (txtMdp.Password.Length < 6 || txtMdp.Password.StartsWith("123"))
            {
                var message = new MessageDialog("Veuillez entrer un mot de passe plus compliqué (6 caractères au moins).");
                await message.ShowAsync();
            }
            else if (txtVille.Text == "")
            {
                var message = new MessageDialog("Veuillez entrer une ville.");
                await message.ShowAsync();
            }
            else
            {
                string login = lutilisateurActuellement.Login;
                string motDePasse = lutilisateurActuellement.MotDePasse;
                string newNom = txtNom.Text;
                string newPrenom = txtPrenom.Text;
                string newMdp = txtMdp.Password;
                string newVille = txtVille.Text;

                var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index.php?" +
                                                      "action=updateUser" +
                                                      "&login=" + login +
                                                      "&motdepasse=" + motDePasse +
                                                      "&new_nom=" + newNom +
                                                      "&new_prenom=" + newPrenom +
                                                      "&new_mdp=" + newMdp +
                                                      "&new_ville=" + newVille);
                var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
                var resultat = donnees["Success"];
                if (resultat == "true")
                {
                    lutilisateurActuellement.NomUtilisateur = txtNom.Text;
                    lutilisateurActuellement.PrenomUtilisateur = txtPrenom.Text;
                    lutilisateurActuellement.MotDePasse = txtMdp.Password;
                    lutilisateurActuellement.VilleCandidat = txtMdp.Password;
                    lesDonnees.LUtilisateurDeMaintenant = lutilisateurActuellement;
                    var message = new MessageDialog("Votre profil a bien été mis à jour");
                    await message.ShowAsync();
                    this.Frame.Navigate(typeof(Page_Accueil), lesDonnees);
                }
                else
                {
                    var message = new MessageDialog("L'utilisateur n'existe pas ou les identifiants sont incorrects. Veuillez recharger l'application.");
                    await message.ShowAsync();
                    this.Frame.Navigate(typeof(Page_Profil), lesDonnees);
                }
            }
        }
        private async void BtnModifierDescription_Click(object sender, RoutedEventArgs e)
        {
            if (txtDescription.Text == "")
            {
                var message = new MessageDialog("Veuillez entrer une description.");
                await message.ShowAsync();
            }
            else
            {
                string login = lutilisateurActuellement.Login;
                string motDePasse = lutilisateurActuellement.MotDePasse;
                string description = txtDescription.Text;

                var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index.php?" +
                                                      "action=updateDesc" + 
                                                      "&login=" + login +
                                                      "&motdepasse=" + motDePasse +
                                                      "&new_description=" + description);
                var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
                var resultat = donnees["Success"];
                if (resultat == "true")
                {
                    lutilisateurActuellement.DescriptionCandidat = txtDescription.Text;
                    lesDonnees.LUtilisateurDeMaintenant = lutilisateurActuellement;
                    var message = new MessageDialog("Votre description a bien été mise à jour");
                    await message.ShowAsync();
                    this.Frame.Navigate(typeof(Page_Accueil), lesDonnees);
                }
                else
                {
                    var message = new MessageDialog("L'utilisateur n'existe pas ou les identifiants sont incorrects. Veuillez recharger l'application.");
                    await message.ShowAsync();
                    this.Frame.Navigate(typeof(Page_Profil), lesDonnees);
                }
            }
        }

        private void BtnProfil_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_Profil), lesDonnees);
        }

        private void BtnMesMessages_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_Messages), lesDonnees);

        }

        private void BtnNvelleCandidature_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_Accueil), lesDonnees);

        }
    }
}
