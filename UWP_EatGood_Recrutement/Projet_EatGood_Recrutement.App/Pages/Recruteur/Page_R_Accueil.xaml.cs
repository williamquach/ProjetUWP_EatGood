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

        private async void btnStatutEncours_Click(object sender, RoutedEventArgs e)
        {
            if(lvCandidatures.SelectedItem != null)
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
                    var ChangerLaCandid = await SetNouveauStatutCandidature("En cours");
                }
            }
            else
            {
                var message = new MessageDialog("Veuillez d'abord sélectionner une candidature.");
                await message.ShowAsync();
            }
        }
        private async void btnStatutRefuser_Click(object sender, RoutedEventArgs e)
        {
            if (lvCandidatures.SelectedItem != null)
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
                    Candidature laCandidChoisie = lvCandidatures.SelectedItem as Candidature;

                    var ChangerLaCandid = await SetNouveauStatutCandidature("Refusée");
                    if (ChangerLaCandid == true)
                    {
                        laCandidChoisie.StatutMessage = "Refusée";
                        await SetMessageCandidature(laCandidChoisie, "En cours");
                    }
                }
            }
            else
            {
                var message = new MessageDialog("Veuillez d'abord sélectionner une candidature.");
                await message.ShowAsync();
            }
        }
        private async void btnStatutAccepter_Click(object sender, RoutedEventArgs e)
        {
            if (lvCandidatures.SelectedItem != null)
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
                    Candidature laCandidChoisie = lvCandidatures.SelectedItem as Candidature;

                    bool ChangerLaCandid = await SetNouveauStatutCandidature("Acceptée");
                    if (ChangerLaCandid is true)
                    {
                        laCandidChoisie.StatutMessage = "Acceptée";
                        await SetMessageCandidature(laCandidChoisie, "En cours");
                    }
                }
            }
            else
            {
                var message = new MessageDialog("Veuillez d'abord sélectionner une candidature.");
                await message.ShowAsync();
            }
        }
        private async void btnMessageReponse_Click(object sender, RoutedEventArgs e)
        {
            if (lvCandidatures.SelectedItem != null)
            {
                Candidature laCandidChoisie = lvCandidatures.SelectedItem as Candidature;
                Restaurant leRestoChoisi = laCandidChoisie.LeResto;
                Poste lePosteChoisi = laCandidChoisie.LePosteVoulu;
                Utilisateur lutilisateur = laCandidChoisie.LeCandidat;
                if (laCandidChoisie.StatutMessage == "En attente")
                {
                    var message = new MessageDialog("Vous ne pouvez pas modifier le message d'une candidature qui n'est pas : en cours de traitement ou déjà traitée");
                    await message.ShowAsync();
                }
                else
                {
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
                        SetMessageCandidature(laCandidChoisie, "Traitée");
                    }
                }
            }
            else
            {
                var message = new MessageDialog("! Avant de modifier le message d'une candidature, il faut d'abord en selectionner une !");
                await message.ShowAsync();
            }
        }
        private async Task<string> InputTextDialogAsync(string title, string ancienMessage, Candidature laCandidChoisie)
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 256;
            inputTextBox.Width = 480;
            inputTextBox.TextWrapping = TextWrapping.Wrap;
            if (ancienMessage != "")
            {
                inputTextBox.Text = ancienMessage;
            }
            else
            {
                if (laCandidChoisie.StatutMessage == "Acceptée")
                {
                    inputTextBox.Text = $"TEMPLATE : Bonjour Mme./M. {laCandidChoisie.LeCandidat.FullName},\n " +
                        $"Nous vous remercions d'avoir postulé au restaurant {laCandidChoisie.LeResto.LibelleResto} " +
                        $"au poste d'/de {laCandidChoisie.LePosteVoulu.LibellePoste}\n " +
                        "Après un examen attentif de votre candidature, nous avons le plaisir de vous informer qu'elle " +
                        "a pas été sélectionnée pour ce poste.\n " +
                        "Cependant, votre chemin n'est pas tout tracé. Vous devez encore passer un entretien avec notre directeur " +
                        "des ressources humaines.\n " +
                        $"Ainsi, vous disposez de 5 jours (ouvrés) pour nous appeler au numéro suivant {laCandidChoisie.LeResto.NumTelResto}, afin " +
                        "de fixer un jour et un horaire d'entretien.\n " +
                        "Nous vous remercions de votre intérêt pour EatGood.\n " +
                        "Meilleures salutations, L'équipe Ressources Humaines EatGood.";
                }
                else if (laCandidChoisie.StatutMessage == "Refusée")
                {
                    inputTextBox.Text = $"TEMPLATE : Bonjour Mme./M. {laCandidChoisie.LeCandidat.FullName},\n " +
                        $"Nous vous remercions d'avoir postulé au restaurant {laCandidChoisie.LeResto.LibelleResto} " +
                        $"au poste d'/de {laCandidChoisie.LePosteVoulu.LibellePoste}\n " +
                        "Après un examen attentif de votre candidature, nous regrettons de vous informer qu'elle " +
                        "n'a pas été sélectionnée pour ce poste.\n " +
                        "Nous pourrons vous contacter pour de futures opportunités et vous encourageons " +
                        "à nous rejoindre sur les réseaux sociaux.\n " +
                        "Nous vous remercions de votre intérêt pour EatGood et vous souhaitons le meilleur " +
                        "dans votre carrière.\n " +
                        "Meilleures salutations, L'équipe Ressources Humaines EatGood.";
                }

            }
            ContentDialog dialog = new ContentDialog();
            dialog.Width = 600;
            dialog.Content = inputTextBox;
            dialog.Title = title;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Valider";
            dialog.SecondaryButtonText = "Annuler";
            // Ici nous allon gérer 4 cas : 
            // L'utilisateur a cliqué sur Valider => on renvoie la valeur à l'interieur du textbox"
            // L'utilisateur a cliqué sur Annuler, il faut renvoyer ce qu'il yavait avant
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else 
                return "annuler";
        }
        public async Task<bool> SetNouveauStatutCandidature(string statut)
        {
            bool ModifierLeStatut = false;
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
                        ModifierLeStatut = true;
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
            return ModifierLeStatut;
        }
        public async Task SetMessageCandidature(Candidature laCandidChoisie, string statutAvantChangementMessage)
        {
            string text = await InputTextDialogAsync("Message de réponse au candidat", laCandidChoisie.MessageReponse, laCandidChoisie);
            if (text != "" && text!="annuler")
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
            else if (text == "" || (text == "annuler" && statutAvantChangementMessage == "En cours"))
            {
                var message = new MessageDialog("Vous devez impérativement entrer un message.");
                await message.ShowAsync();
                await Task.WhenAll(SetMessageCandidature(laCandidChoisie, "En cours"));
            }
        }
        private void lvCandidatures_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));

        }
        private void BtnProfil_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_R_Profil), lesDonnees);
        }
        private void BtnMesMessages_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_R_Messages), lesDonnees);
        }
        private void BtnBackOffice_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_R_Backoffice), lesDonnees);
        }
    }
}
