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
    public sealed partial class Page_R_Accueil : Page
    {
        public Page_R_Accueil()
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
                txtBienvenue.Text = $"Bienvenue {lutilisateurActuellement.PrenomUtilisateur} {lutilisateurActuellement.NomUtilisateur}";
            }
            else
            {
                txtBienvenue.Text = $"Bienvenue ! ";
            }
            base.OnNavigatedTo(e);

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            hc = new HttpClient();
            List<Candidature> lesCandidatures = lesDonnees.AllCandidatures;
            lvCandidatures.ItemsSource = lesCandidatures;
            if (lesCandidatures.Count == 0)
            {
                txtYaTilDesCandidatures.Visibility = Visibility.Visible;
                lvCandidatures.Visibility = Visibility.Collapsed;
            }
            else
            {
                lvCandidatures.ItemsSource = lesCandidatures;
            }
        }
        public async void SetNouveauStatutCandidature(string statut)
        {
            if (lvCandidatures.SelectedItem != null)
            {
                Candidature laCandidChoisie = lvCandidatures.SelectedItem as Candidature;
                Restaurant leRestoChoisi = laCandidChoisie.LeResto;
                Poste lePosteChoisi = laCandidChoisie.LePosteVoulu;
                Utilisateur lutilisateur = laCandidChoisie.LeCandidat;
                ContentDialog ChangeStatutCandidatureDialog = new ContentDialog
                {
                    Title = "Bonjour !",
                    Content = "Voulez-vous vraiment modifier le statut de la candidature de " + lutilisateur.FullName +
                              "au restaurant " + leRestoChoisi.LibelleResto + " au poste de " + lePosteChoisi.LibellePoste + " ?",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await ChangeStatutCandidatureDialog.ShowAsync();
                // Modifie la candidature si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    //action=updateCandidatureStatut&idCandidature={idCandidature}&new_statut={new_statut}
                    string idCandidature = laCandidChoisie.IdCandidature.ToString();
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                          "action=updateCandidatureStatut" +
                                                          "&idCandidature=" + idCandidature +
                                                          "&new_statut=" + statut);
                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donneesJson["Success"];
                    if (resultat == "true")
                    {
                        await lesDonnees.SetAllCandidature();
                        this.Frame.Navigate(typeof(Page_R_Accueil), lesDonnees);
                    }
                    else if (resultat == "false")
                    {
                        var message = new MessageDialog("Cette candidature n'existe pas. Ou alors il y a une erreur dans le code");
                        await message.ShowAsync();
                    }
                }
            }
            else
            {
                var message = new MessageDialog("! Avant de modifier le statut d'une candidature, il faut d'abord en selectionner une !");
                await message.ShowAsync();
            }
        }

        private async void btnStatutEncours_Click(object sender, RoutedEventArgs e)
        {
            if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "En cours")
            {
                var message = new MessageDialog("Cette candidature est déjà en statut 'En cours'");
                await message.ShowAsync();
            }
            else if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "Acceptée")
            {
                var message = new MessageDialog("Cette candidature a déjà été acceptée, vous ne pouvez pas revenir en arrière.");
                await message.ShowAsync();
            }
            else if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "Refusée")
            {
                var message = new MessageDialog("Cette candidature a déjà été refusée, vous ne pouvez pas revenir en arrière.");
                await message.ShowAsync();
            }
            else if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "En attente")
            {
                SetNouveauStatutCandidature("En cours");
            }

        }

        private async void btnStatutRefuser_Click(object sender, RoutedEventArgs e)
        {
            if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "En attente")
            {
                var message = new MessageDialog("Cette candidature n'a pas encore été traitée, veuillez d'abord passer son statut" +
                                                " à 'En cours'");
                await message.ShowAsync();
            }
            else if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "Acceptée")
            {
                var message = new MessageDialog("Cette candidature a déjà été acceptée, vous ne pouvez pas revenir en arrière.");
                await message.ShowAsync();
            }
            else if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "Refusée")
            {
                var message = new MessageDialog("Cette candidature a déjà été refusée.");
                await message.ShowAsync();
            }
            else if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "En cours")
            {
                SetNouveauStatutCandidature("Refusée");
            }
        }

        private async void btnStatutAccepter_Click(object sender, RoutedEventArgs e)
        {
            if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "En attente")
            {
                var message = new MessageDialog("Cette candidature n'a pas encore été traitée, veuillez d'abord passer son statut" +
                                                " à 'En cours'");
                await message.ShowAsync();
            }
            else if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "Acceptée")
            {
                var message = new MessageDialog("Cette candidature a déjà été acceptée.");
                await message.ShowAsync();
            }
            else if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "Refusée")
            {
                var message = new MessageDialog("Cette candidature a déjà été refusée, vous ne pouvez pas revenir en arrière.");
                await message.ShowAsync();
            }
            else if ((lvCandidatures.SelectedItem as Candidature).StatutMessage == "En cours")
            {
                SetNouveauStatutCandidature("Acceptée");
            }
        }
        private async Task<string> InputTextDialogAsync(string title, string ancienMessage)
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 256;
            inputTextBox.Width = 480;
            inputTextBox.TextWrapping = TextWrapping.Wrap;
            inputTextBox.Text = ancienMessage;
            ContentDialog dialog = new ContentDialog();
            dialog.Width = 600;
            dialog.Content = inputTextBox;
            dialog.Title = title;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Valider";
            dialog.SecondaryButtonText = "Annuler";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else
                return "";
        }
        // REFUS:
        //Bonjour, 

        //Nous vous remercions d'avoir postulé au poste d'équipière polyvalente. 
        //Après un examen attentif de votre candidature, nous regrettons de vous informer qu'elle n'a pas été sélectionnée pour ce poste.

        //Nous pourrons vous contacter pour de futures opportunités et vous encourageons à nous rejoindre sur les réseaux sociaux.
        //Nous vous remercions de votre intérêt pour EatGood et vous souhaitons le meilleur dans votre carrière.

        //Meilleures salutations,
        private async void btnMessageReponse_Click(object sender, RoutedEventArgs e)
        {
            if (lvCandidatures.SelectedItem != null)
            {
                Candidature laCandidChoisie = lvCandidatures.SelectedItem as Candidature;
                Restaurant leRestoChoisi = laCandidChoisie.LeResto;
                Poste lePosteChoisi = laCandidChoisie.LePosteVoulu;
                Utilisateur lutilisateur = laCandidChoisie.LeCandidat;
                ContentDialog ChangeStatutCandidatureDialog = new ContentDialog
                {
                    Title = "Bonjour !",
                    Content = "Voulez-vous vraiment modifier le message de la candidature de " + lutilisateur.FullName +
                              "au restaurant " + leRestoChoisi.LibelleResto + " au poste de " + lePosteChoisi.LibellePoste + " ?",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await ChangeStatutCandidatureDialog.ShowAsync();
                // Modifie la candidature si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.

                if (result == ContentDialogResult.Primary)
                {
                    string text = await InputTextDialogAsync("Message de réponse au candidat", laCandidChoisie.MessageReponse);
                    if (text != "")
                    {
                        string messageRep = text;
                        //action=updateCandidatureStatut&idCandidature={idCandidature}&new_message={new_statut}
                        string idCandidature = laCandidChoisie.IdCandidature.ToString();
                        var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                              "action=updateCandidatureMessage" +
                                                              "&idCandidature=" + idCandidature +
                                                              "&new_message=" + messageRep);
                        var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                        var resultat = donneesJson["Success"];
                        if (resultat == "true")
                        {
                            await lesDonnees.SetAllCandidature();
                            this.Frame.Navigate(typeof(Page_R_Accueil), lesDonnees);
                        }
                        else if (resultat == "false")
                        {
                            var message = new MessageDialog("Cette candidature n'existe pas. Ou alors il y a une erreur dans le code");
                            await message.ShowAsync();
                        }
                    }
                    else
                    {
                        var message = new MessageDialog("Vous devez impérativement entrer un message.");
                        await message.ShowAsync();
                    }
                }
            }
            else
            {
                var message = new MessageDialog("! Avant de modifier le message d'une candidature, il faut d'abord en selectionner une !");
                await message.ShowAsync();
            }
        }

        private void lvCandidatures_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));

        }
    }
}
