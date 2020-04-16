using Projet_EatGood_Recrutement.Classes;
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Utilisateur && e.Parameter != null)
            {
                Utilisateur lutilisateurActuellement = e.Parameter as Utilisateur;
                txtBienvenue.Text = $"Bienvenue, {lutilisateurActuellement.PrenomUtilisateur}";
            }
            else
            {
                txtBienvenue.Text = $"Bienvenue !";
            }
            base.OnNavigatedTo(e);
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void lvCandidaturesCandidat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnProfil_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
