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
using Newtonsoft.Json;
using Projet_EatGood_Recrutement.App.API;
using Projet_EatGood_Recrutement.App.Pages.Candidat;
using Projet_EatGood_Recrutement.Classes;
using System.Net.Http;
using Windows.UI.Popups;
using System.Threading.Tasks;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projet_EatGood_Recrutement.App.Pages.Recruteur
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Page_R_Messages : Page
    {
        Utilisateur lutilisateurActuellement;
        Data_EatGood lesDonnees;
        HttpClient hc;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                lesDonnees = e.Parameter as Data_EatGood;
                lutilisateurActuellement = lesDonnees.LUtilisateurDeMaintenant;
                txtBienvenue.Text = $"Voici tes messages {lutilisateurActuellement.PrenomUtilisateur} {lutilisateurActuellement.NomUtilisateur}";
            }
            else
            {
                txtBienvenue.Text = $"Voici tes messages ! ";
            }
            base.OnNavigatedTo(e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            hc = new HttpClient();
            ChargerLesMessages();
        }
        public Page_R_Messages()
        {
            this.InitializeComponent();
        }
        public async void ChargerLesMessages()
        {
            List<Message> lesMessageDuUser = await lesDonnees.GetLesMessagesDuCandidat(lutilisateurActuellement.IdUtilisateur);
            if (lesMessageDuUser.Count == 0)
            {
                txtYatilDesMessages.Visibility = Visibility.Visible;
                lvMessagesDuCandidat.Visibility = Visibility.Collapsed;
            }
            else
            {
                lvMessagesDuCandidat.ItemsSource = lesMessageDuUser;
            }
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            // action=deletemessage&idMessage={ID MESSAGE}&idCandidat={ID CANDIDAT}
            if (lvMessagesDuCandidat.SelectedItem != null)
            {
                Message leMessageChoisi = lvMessagesDuCandidat.SelectedItem as Message;
                Restaurant leRestoChoisi = leMessageChoisi.LExpediteur;
                ContentDialog DeleteMessageDialog = new ContentDialog
                {
                    Title = "Attention !",
                    Content = "Vous vous apprêtez à supprimer un message.\n" +
                              "Êtes-vous sûr(e) de vouloir supprimer ce message du restaurant " + leRestoChoisi.LibelleResto +
                              " ?",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await DeleteMessageDialog.ShowAsync();
                // Supprime le message si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    // action=deleteCandidature&idCandidature={idCandidature}
                    string idMessage = leMessageChoisi.IdMessage.ToString();
                    string idCandidat = lutilisateurActuellement.IdUtilisateur.ToString();
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_candidat.php?" +
                                                          "action=deleteMessage" +
                                                          "&idMessage=" + idMessage +
                                                          "&idCandidat=" + idCandidat);
                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donneesJson["Success"];
                    if (resultat == "true")
                    {
                        var message = new MessageDialog("Vous avez supprimé un message du restaurant " +
                                                        leRestoChoisi.LibelleResto);
                        await message.ShowAsync();
                        await lesDonnees.ChargerLesDonnees();
                        this.Frame.Navigate(typeof(Page_Accueil), lesDonnees);
                    }
                    else
                    {
                        var message = new MessageDialog("Ce message n'existe pas. Ou alors il y a une erreur dans le code");
                        await message.ShowAsync();
                    }
                }
            }
            else
            {
                var message = new MessageDialog("! Avant de supprimer un message, il faut d'abord en selectionner un !");
                await message.ShowAsync();
            }
        }
        private async void btnChangerEtat_Click(object sender, RoutedEventArgs e)
        {
            if (lvMessagesDuCandidat.SelectedItem != null)
            {
                Message leMessageChoisi = lvMessagesDuCandidat.SelectedItem as Message;
                Restaurant leRestoChoisi = leMessageChoisi.LExpediteur;
                if (leMessageChoisi.StatutMessage == "Non lu")
                {

                    ContentDialog ChangeMessageDialog = new ContentDialog
                    {
                        Title = "Attention !",
                        Content = "Avez vous réellement lu ce message du restaurant " + leRestoChoisi.LibelleResto +
                                  " ?",
                        PrimaryButtonText = "Oui",
                        CloseButtonText = "Non"
                    };

                    ContentDialogResult result = await ChangeMessageDialog.ShowAsync();
                    // Change letat du message si l'utilisateur a cliqué sur le bouton principal ("oui")
                    // Sinon, rien faire.
                    if (result == ContentDialogResult.Primary)
                    {
                        ChangerEtatMessage();
                    }
                }
                else
                {
                    ContentDialog ChangeMessageDialog = new ContentDialog
                    {
                        Title = "Attention !",
                        Content = "Voulez-vous vraiment changer le statut du message du restaurant " + leRestoChoisi.LibelleResto +
                                  " en 'Non lu' ?",
                        PrimaryButtonText = "Oui",
                        CloseButtonText = "Non"
                    };

                    ContentDialogResult result = await ChangeMessageDialog.ShowAsync();
                    // Change letat du message si l'utilisateur a cliqué sur le bouton principal ("oui")
                    // Sinon, rien faire.
                    if (result == ContentDialogResult.Primary)
                    {
                        ChangerEtatMessage();
                    }
                }
            }
            else
            {
                var message = new MessageDialog("Veuillez sélectionner un message avant de vouloir changer son statut.");
                await message.ShowAsync();
            }
        }
        public async void ChangerEtatMessage()
        {
            Message leMessageSelectionne = lvMessagesDuCandidat.SelectedItem as Message;
            int idMsg = leMessageSelectionne.IdMessage;
            int idCandidat = leMessageSelectionne.LeDestinataire.IdUtilisateur;
            int idResto = leMessageSelectionne.LExpediteur.IdResto;
            var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/updateStatutMessage.php?" +
                                                  "action=updateEtatMessage" +
                                                  "&idMessage=" + idMsg +
                                                  "&idCandidat=" + idCandidat +
                                                  "&idResto=" + idResto);
            var donnees = JsonConvert.DeserializeObject<dynamic>(reponse);
            var list = donnees["Success"];
            if (list == "false")
            {
                var message = new MessageDialog("Erreur le message n'existe pas");
                await message.ShowAsync(); ;
            }
            else if (list == "true")
            {
                ChargerLesMessages();
            }
        }

        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
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

        }
    }
}
