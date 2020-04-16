using Projet_EatGood_Recrutement.App.API;
using Projet_EatGood_Recrutement.App.Pages.Candidat;
using Projet_EatGood_Recrutement.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projet_EatGood_Recrutement.App.Pages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Page_Accueil : Page
    {
        public Page_Accueil()
        {
            this.InitializeComponent();
        }
        Utilisateur lutilisateurActuellement;
        object[] donnesRecues;
        Data_EatGood dataEatGood;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                donnesRecues = (Object[])e.Parameter;
                dataEatGood = donnesRecues[1] as Data_EatGood;
                lutilisateurActuellement = donnesRecues[0] as Utilisateur;
                txtBienvenue.Text = $"Bienvenue {lutilisateurActuellement.PrenomUtilisateur} {lutilisateurActuellement.NomUtilisateur}";
            }
            else
            {
                txtBienvenue.Text = $"Bienvenue ! ";
            }
            base.OnNavigatedTo(e);
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            List<Candidature> lesCandidaturesDuUser = await dataEatGood.GetLesCandidaturesByCandidat(lutilisateurActuellement.IdUtilisateur);
            if (lesCandidaturesDuUser.Count == 0)
            {
                var message = new MessageDialog("Vous n'avez posté aucune candidature. Qu'attendez vous ?! ;)");
                await message.ShowAsync();
            } 
            else
            {
                lvCandidaturesCandidat.ItemsSource = lesCandidaturesDuUser;
            }
        }

        private void lvCandidaturesCandidat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnProfil_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_Profil), lutilisateurActuellement);

        }

        private void BtnMesMessages_Click(object sender, RoutedEventArgs e)
        {

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
            this.Frame.Navigate(typeof(Page_Accueil), lutilisateurActuellement);
        }
    }
}
