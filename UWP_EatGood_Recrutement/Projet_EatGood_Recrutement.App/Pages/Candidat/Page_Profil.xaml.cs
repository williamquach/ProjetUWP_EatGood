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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                lutilisateurActuellement = e.Parameter as Utilisateur;
                txtBienvenue.Text += " " + lutilisateurActuellement.PrenomUtilisateur + " " + lutilisateurActuellement.NomUtilisateur;
            }
            base.OnNavigatedTo(e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (lutilisateurActuellement != null)
            {
                txtMdp.Password = lutilisateurActuellement.MotDePasse;
                txtNom.Text = lutilisateurActuellement.NomUtilisateur;
                txtPrenom.Text = lutilisateurActuellement.PrenomUtilisateur;
                txtVille.Text = lutilisateurActuellement.VilleCandidat;
                txtDescription.Text = lutilisateurActuellement.DescriptionCandidat;
            }
        }
        private void BtnModifierProfil_Click(object sender, RoutedEventArgs e)
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
    }
}
