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
    public sealed partial class Page_NouvelleCandidature : Page
    {
        public Page_NouvelleCandidature()
        {
            this.InitializeComponent();
        }
        Utilisateur lutilisateurActuellement;
        Data_EatGood lesDonnees;
        HttpClient hc;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                lesDonnees = e.Parameter as Data_EatGood;
                lutilisateurActuellement = lesDonnees.LUtilisateurDeMaintenant;
                txtBienvenue.Text = $"Rejoins-nous {lutilisateurActuellement.PrenomUtilisateur} {lutilisateurActuellement.NomUtilisateur}";
            }
            else
            {
                txtBienvenue.Text = $"Nouvelle candidature ! ";
            }
            base.OnNavigatedTo(e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            hc = new HttpClient();
            cboResto.ItemsSource = lesDonnees.lesResto;
            cboPostes.ItemsSource = lesDonnees.lesPostes;
        }
        private async void BtnEnvoyerMaCandidature_Click(object sender, RoutedEventArgs e)
        {
            if(cboResto.SelectedItem == null)
            {
                var message = new MessageDialog("Veuillez choisir un restaurant.");
                await message.ShowAsync(); ;
            } 
            else if (cboPostes.SelectedItem == null)
            {
                var message = new MessageDialog("Veuillez choisir un poste.");
                await message.ShowAsync(); ;
            }
            else if (txtMotivations.Text == "")
            {
                var message = new MessageDialog("Veuillez entrer vos motivations.");
                await message.ShowAsync(); ;
            }
            else
            {
                // action=newCandidature&idCandidat={idCandidat}&idResto={idResto}&idPoste={idPoste}&motivations={motivations}
                // récupération des données candidature
                string idCandidat = lutilisateurActuellement.IdUtilisateur.ToString();
                Poste lePosteChoisi = (cboPostes.SelectedItem as Poste);
                string idPoste = lePosteChoisi.IdPoste.ToString();
                Restaurant leRestoChoisi = (cboResto.SelectedItem as Restaurant);
                string idResto = leRestoChoisi.IdResto.ToString();
                string motivations = txtMotivations.Text;
                ContentDialog newCandidatureDialog = new ContentDialog
                {
                    Title = "Attention !",
                    Content = "Les motivations que vous avez rentrées ne sont pas modifiables par la suite, relisez afin de ne pas faire d'erreur.\n" +
                              "Êtes-vous sûr(e) de vouloir postuler au restaurant " + leRestoChoisi.LibelleResto + 
                              " au poste de " + lePosteChoisi.LibellePoste + " ?",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await newCandidatureDialog.ShowAsync();
                // Créer la candidature si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    // Créer la candidature
                    // L'api permet de vérifier si la candidature existe déjà à l'aide de l'utilisateur et l'id du resto
                    // Si elle n'existe pas déjà, on la créée.
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index.php?" +
                                                          "action=newCandidature" +
                                                          "&idCandidat=" + idCandidat +
                                                          "&idResto=" + idResto +
                                                          "&idPoste=" + idPoste +
                                                          "&motivations=" + motivations);
                    var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donnees["Success"];
                    if (resultat == "false")
                    {
                        var message = new MessageDialog("Vous avez déjà candidaté pour le restaurant " + leRestoChoisi.LibelleResto);
                        await message.ShowAsync();
                        cboResto.SelectedItem = null; ;
                    }
                    else
                    {
                        var message = new MessageDialog("Vous avez bien candidaté pour le restaurant " + 
                                                        leRestoChoisi.LibelleResto + " au poste de " + lePosteChoisi.LibellePoste);
                        await message.ShowAsync();
                        cboResto.SelectedItem = null;
                        cboPostes.SelectedItem = null;
                        txtMotivations.Text = "";
                        await lesDonnees.ChargerLesDonnees();
                        this.Frame.Navigate(typeof(Page_Accueil), lesDonnees);
                    }
                }
                else
                {
                    // Ne pas créer la candidature

                }
            }
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_Accueil), lesDonnees);

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
            this.Frame.Navigate(typeof(Page_NouvelleCandidature), lesDonnees);

        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
