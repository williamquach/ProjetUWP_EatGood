using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    public sealed partial class _2_Page_Inscription : Page
    {
        public _2_Page_Inscription()
        {
            this.InitializeComponent();
        }
        HttpClient hc;
        private async void BtnSinscrire_Click(object sender, RoutedEventArgs e)
        {
            hc = new HttpClient();
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
            else if (txtLogin.Text == "")
            {
                var message = new MessageDialog("Veuillez entrer un login.");
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
                // localhost/recru_eatgood_api/index_candidat.php?action=sinscrire&nom={NOM}&prenom={PRENOM}&login={LOGIN}&motdepasse={PASSWORD}&ville={ville}
                // récupération des données utilisateurs
                string nom = txtNom.Text;
                string prenom = txtPrenom.Text;
                string login = txtLogin.Text;
                string mdp = txtMdp.Password;
                string ville = txtVille.Text;

                // L'api permet de vérifier si l'utilisateur existe déjà à l'aide de son login
                // S'il n'existe pas déjà, on l'inscrit.
                var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_candidat.php?" +
                                                      "action=sinscrire&" +
                                                      "nom=" + nom + "&" +
                                                      "prenom=" + prenom + "&" +
                                                      "login=" + login + "" +
                                                      "&motdepasse=" + mdp + "&" +
                                                      "ville=" + ville + "");
                var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
                var resultat = donnees["Success"];
                if (resultat == "false")
                {
                    var message = new MessageDialog("L'utilisateur " + txtLogin.Text + " existe déjà.");
                    await message.ShowAsync();
                    txtLogin.Text = "";
                }
                else
                {
                    var message = new MessageDialog("Utilisateur "+ txtLogin.Text +" est inscrit.");
                    await message.ShowAsync();
                    txtNom.Text = "";
                    txtPrenom.Text = "";
                    txtLogin.Text = "";
                    txtMdp.Password = "";
                    txtVille.Text = "";

                    this.Frame.Navigate(typeof(MainPage));

                }
            }
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
