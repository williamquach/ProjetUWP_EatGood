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
using Projet_EatGood_Recrutement.Classes;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Projet_EatGood_Recrutement.App.API;
using Projet_EatGood_Recrutement.App.Pages;
// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Projet_EatGood_Recrutement.App
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

        }
        Data_EatGood lesDonnees;
        Utilisateur lutilisateurActuel;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetAllUsers();
        }
        public async void GetAllUsers()
        {
            lesDonnees = new Data_EatGood();

            await lesDonnees.ChargerLesDonnees();
        }

        private async void btnConnexion_Click(object sender, RoutedEventArgs e)
        {
            if (txtLogin.Text == "" || txtMdp.Password == "")
            {
                var error_Entry = new MessageDialog("Veuillez entrer un identifiant et un mot de passe.");
                await error_Entry.ShowAsync();
            }
            else
            {
                bool login = false;
                bool mdp = false;
                foreach (Utilisateur u in lesDonnees.lesUtilisateurs)
                {
                    if (u.Login == txtLogin.Text && u.MotDePasse == txtMdp.Password)
                    {
                        login = true;
                        mdp = true;
                        lutilisateurActuel = u;
                        break;
                    }
                    else if (u.Login == txtLogin.Text && u.MotDePasse != txtMdp.Password)
                    {
                        login = true;
                        break;
                    }
                }
                if (!login && !mdp)
                {
                    var error_Ids = new MessageDialog("Identifiant ou mot de passe incorrect.");
                    await error_Ids.ShowAsync();
                }
                else if (login && !mdp)
                {
                    var error_Mdp = new MessageDialog("Mot de passe incorrect.");
                    await error_Mdp.ShowAsync();
                }
                else if (login && mdp)
                {
                    object[] donneesAEnvoyer = new object[2];
                    donneesAEnvoyer[0] = lutilisateurActuel;
                    donneesAEnvoyer[1] = lesDonnees;
                    this.Frame.Navigate(typeof(Page_Accueil), donneesAEnvoyer);

                }
            }
        }

        private void btnInscription_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(_2_Page_Inscription));
        }
    }
}
