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
            ChargerLesDonneesEnvoiMsg();
        }
        public Page_R_Messages()
        {
            this.InitializeComponent();
        }
        public async void ChargerLesMessages()
        {
            List<Message> tousLesMessages = await lesDonnees.GetAllMessagesEnvoyes();
            if (tousLesMessages.Count == 0)
            {
                txtYatilDesMessages.Visibility = Visibility.Visible;
                lvMessagesDuCandidat.Visibility = Visibility.Collapsed;
            }
            else
            {
                lvMessagesDuCandidat.ItemsSource = tousLesMessages;
            }
        }
        public async void ChargerLesDonneesEnvoiMsg()
        {
            cboMessages.ItemsSource = await lesDonnees.GetAllMessages();
            cboResto.ItemsSource = lesDonnees.lesResto;
            lvMsgAuxCandidats.ItemsSource = lesDonnees.GetAllCandidats();
        }
        private async Task<string> InputTextDialogAsync(string title)
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 256;
            inputTextBox.Width = 480;
            inputTextBox.TextWrapping = TextWrapping.Wrap;
            inputTextBox.Text = "";
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
        private async void btnSupprimerMessageEnvoye_Click(object sender, RoutedEventArgs e)
        {
            // action=deletemessage&idMessage={ID MESSAGE}&idCandidat={ID CANDIDAT}
            if (lvMessagesDuCandidat.SelectedItem != null)
            {
                Message leMessageChoisi = lvMessagesDuCandidat.SelectedItem as Message;
                Restaurant leRestoChoisi = leMessageChoisi.LExpediteur;
                Utilisateur leDestinataire = leMessageChoisi.LeDestinataire;
                ContentDialog DeleteMessageDialog = new ContentDialog
                {
                    Title = "Attention !",
                    Content = "Vous vous apprêtez à supprimer un message envoyé.\n" +
                              "Êtes-vous sûr(e) de vouloir supprimer ce message du restaurant " + leRestoChoisi.LibelleResto +
                              " destiné à " + leDestinataire.FullName + " ?",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await DeleteMessageDialog.ShowAsync();
                // Supprime le message envoyé si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    // action=deleteCandidature&idCandidature={idCandidature}
                    string idMessage = leMessageChoisi.IdMessage.ToString();
                    string idCandidat = leDestinataire.IdUtilisateur.ToString();
                    string idResto = leRestoChoisi.IdResto.ToString();
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                          "action=deleteMessageEnvoye" +
                                                          "&idMessage=" + idMessage +
                                                          "&idCandidat=" + idCandidat +
                                                          "&idResto=" + idResto);
                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donneesJson["Success"];
                    if (resultat == "true")
                    {
                        var message = new MessageDialog("Vous avez supprimé un message du restaurant " +
                                                        leRestoChoisi.LibelleResto + " envoyé à " + leDestinataire.FullName);
                        await message.ShowAsync();
                        await lesDonnees.ChargerLesDonnees();
                        this.Frame.Navigate(typeof(Page_R_Messages), lesDonnees);
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
                var message = new MessageDialog("Avant de supprimer un message envoyé, il faut d'abord en selectionner un !");
                await message.ShowAsync();
            }
        }
        private async void btnCreerUnMessage_Click(object sender, RoutedEventArgs e)
        {
            string text = await InputTextDialogAsync("Nouveau message");
            if(text == "")
            {
                var message = new MessageDialog("Vous devez entrer un message.");
                await message.ShowAsync();
            }
            else if (text != "annuler") 
            {
                // "action=createMessage&contenuMsg={message}"

                string contenumsg = text;
                var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                      "action=createMessage" +
                                                      "&contenuMsg=" + contenumsg);
                var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                var resultat = donneesJson["Success"];
                if (resultat == "true")
                {
                    var message = new MessageDialog("Message créé.");
                    await message.ShowAsync();
                    this.Frame.Navigate(typeof(Page_R_Messages), lesDonnees);
                }
                else if (resultat == "false")
                {
                    var message = new MessageDialog("Impossible de créer un message.");
                    await message.ShowAsync();
                }
            }
        }
        private async void btnSupprimerUnMessage_Click(object sender, RoutedEventArgs e)
        {
            if(cboMessages.SelectedItem == null)
            {
                var message = new MessageDialog("Veuillez d'abord sélectionner un message dans la liste ci-dessus.");
                await message.ShowAsync();
            }
            else
            {
                ContentDialog deleteMessageDialog = new ContentDialog
                {
                    Title = "Attention !",
                    Content = "Vous vous apprêtez à supprimer un message ? Êtes-vous sûr d'effectuer cette action ? ",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await deleteMessageDialog.ShowAsync();
                // Créer la candidature si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    string idMessage = (cboMessages.SelectedItem as Message).IdMessage.ToString();
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                          "action=deleteMessage" +
                                                          "&idMessage=" + idMessage);
                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donneesJson["Success"];
                    if (resultat == "true")
                    {
                        var message = new MessageDialog("Message supprimé.");
                        await message.ShowAsync();
                        this.Frame.Navigate(typeof(Page_R_Messages), lesDonnees);
                    }
                    else if (resultat == "false")
                    {
                        var message = new MessageDialog("Impossible de supprimer ce message. Bug");
                        await message.ShowAsync();
                    }
                }
            }
        }
        private async void btnEnvoyerMsg_Click(object sender, RoutedEventArgs e)
        {
            if(cboMessages.SelectedItem == null)
            {
                var message = new MessageDialog("Veuillez d'abord choisir un message à envoyer.");
                await message.ShowAsync();
            }
            else if (cboResto.SelectedItem == null)
            {
                var message = new MessageDialog("Veuillez d'abord choisir un restaurant en tant qu'expéditeur.");
                await message.ShowAsync();
            }
            else if (lvMsgAuxCandidats.SelectedItems == null)
            {
                var message = new MessageDialog("Veuillez d'abord choisir un ou plusieurs candidats.");
                await message.ShowAsync();
            }
            else
            {
                ContentDialog newEnvoieMsgDialog = new ContentDialog
                {
                    Title = "Attention !",
                    Content = "Vous vous apprêtez à envoyer un message à un ou plusieurs candidats ? Êtes-vous sûr d'effectuer cette action ? ",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await newEnvoieMsgDialog.ShowAsync();
                // Créer lenvoie si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    // action=createEnvoiMsg&idMessage={id message}&idCandidat={idCandidat}&idResto={idresto}
                    // &nbCandidatEnvoi={nombre de personne a qui envoyer le msg}
                    Message leMessage = cboMessages.SelectedItem as Message;
                    Restaurant leResto = cboResto.SelectedItem as Restaurant;
                    string idResto = leResto.IdResto.ToString();
                    string idMessage = leMessage.IdMessage.ToString();
                    string lesIdDesDestinataires = "";
                    foreach (Object d in lvMsgAuxCandidats.SelectedItems)
                    {
                        lesIdDesDestinataires += ((d as Utilisateur).IdUtilisateur.ToString());
                    }
                    string nbCandidatsEnvoi = lesIdDesDestinataires.Length.ToString();

                    // LA REQUETE
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                          "action=createEnvoiMsg" +
                                                          "&idMessage=" + idMessage +
                                                          "&idsCandidat=" + lesIdDesDestinataires +
                                                          "&idResto=" + idResto +
                                                          "&nbCandidatEnvoi=" + nbCandidatsEnvoi);
                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donneesJson["Success"];
                    // Si retour de cette requete = true, ajout des envois effectué.
                    if (resultat == "true")
                    {
                        var message = new MessageDialog("Le message a été envoyé au(x) candidat(s).");
                        await message.ShowAsync();
                        this.Frame.Navigate(typeof(Page_R_Messages), lesDonnees);
                    }
                    // Sinon certain envois existent déjà ou manque champs
                    else if (resultat == "false")
                    {
                        var message = new MessageDialog("Impossible de supprimer ce restaurant. Bug");
                        await message.ShowAsync();
                    }
                }
            }
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_R_Accueil), lesDonnees);

        }

        private void BtnProfil_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_R_Profil), lesDonnees);

        }

        private void BtnMesMessages_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_R_Messages), lesDonnees);

        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void btnCreerUnResto_Click(object sender, RoutedEventArgs e)
        {
            string libelleResto = "";
            string numRue = "";
            string nomRue = "";
            string ville = "";
            string cp = "";
            string numTel = "";

            string test1 = await DemanderLibelleResto();
            if (test1 != "annuler")
            {
                libelleResto = test1;

                string test2 = await DemanderNumRue();
                if (test2 != "annuler")
                {
                    numRue = test2;

                    string test3 = await DemanderNomRue();
                    if (test3 != "annuler")
                    {
                        nomRue = test3;

                        string test4 = await DemanderVille();
                        if (test4 != "annuler")
                        {
                            ville = test4;

                            string test5 = await DemanderCP();
                            if (test5 != "annuler")
                            {
                                cp = test5;

                                string test6 = await DemanderNumTel();
                                if (test6 != "annuler")
                                {
                                    numTel = test6;

                                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                      "action=createResto" +
                                                      "&libelle=" + libelleResto +
                                                      "&numRue=" + numRue +
                                                      "&nomRue=" + nomRue +
                                                      "&ville=" + ville +
                                                      "&codePostal=" + cp +
                                                      "&numTel=" + numTel
                                                      );
                                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                                    var resultat = donneesJson["Success"];
                                    if (resultat == "true")
                                    {
                                        var message = new MessageDialog("Restaurant ajouté.");
                                        await message.ShowAsync();
                                        await lesDonnees.ChargerLesDonnees();
                                        this.Frame.Navigate(typeof(Page_R_Messages), lesDonnees);
                                    }
                                    else if (resultat == "false")
                                    {
                                        var message = new MessageDialog("Impossible de d'insérer un restaurant.");
                                        await message.ShowAsync();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
        }
        public async Task<string> DemanderLibelleResto()
        {
            string text = await InputTextDialogAsync("Nom du resto(EatGood [Ville])");
            if(text == "")
            {
                var message = new MessageDialog("Veuillez ne pas laisser le champs vide.");
                await message.ShowAsync();
                return await DemanderLibelleResto();
            }
            else if (text == "annuler")
            {
                return "annuler";
            }
            else
            {
                return text;
            }
        }
        public async Task<string> DemanderNumRue()
        {
            string text = await InputTextDialogAsync("Numéro de rue du restaurant");
            if (text == "")
            {
                var message = new MessageDialog("Veuillez ne pas laisser le champs vide.");
                await message.ShowAsync();
                return await DemanderNumRue();
            }
            else if (text == "annuler")
            {
                return "annuler";
            }
            else
            {
                return text;
            }
        }
        public async Task<string> DemanderNomRue()
        {
            string text = await InputTextDialogAsync("Nom de la rue du restaurant");
            if (text == "")
            {
                var message = new MessageDialog("Veuillez ne pas laisser le champs vide.");
                await message.ShowAsync();
                return await DemanderNomRue();
            }
            else if (text == "annuler")
            {
                return "annuler";
            }
            else
            {
                return text;
            }
        }
        public async Task<string> DemanderVille()
        {
            string text = await InputTextDialogAsync("Ville du restaurant");
            if (text == "")
            {
                var message = new MessageDialog("Veuillez ne pas laisser le champs vide.");
                await message.ShowAsync();
                return await DemanderVille();
            }
            else if (text == "annuler")
            {
                return "annuler";
            }
            else
            {
                return text;
            }
        }
        public async Task<string> DemanderCP()
        {
            string text = await InputTextDialogAsync("Code postal du restaurant");
            if (text == "")
            {
                var message = new MessageDialog("Veuillez ne pas laisser le champs vide.");
                await message.ShowAsync();
                return await DemanderCP();
            }
            else if (text == "annuler")
            {
                return "annuler";
            }
            else
            {
                return text;
            }
        }
        public async Task<string> DemanderNumTel()
        {
            string text = await InputTextDialogAsync("Téléphone du restaurant sans espaces ni points");
            if (text == "")
            {
                var message = new MessageDialog("Veuillez ne pas laisser le champs vide.");
                await message.ShowAsync();
                return await DemanderNumTel();
            }
            else if (text == "annuler")
            {
                return "annuler";
            }
            else
            {
                return text;
            }
        }

        private async void btnRetirerUnResto_Click(object sender, RoutedEventArgs e)
        {
            if(cboResto.SelectedItem == null)
            {
                var message = new MessageDialog("Avant de supprimer un restaurant, il faut d'abord en selectionner un !");
                await message.ShowAsync();
            }
            else
            {
                ContentDialog deleteMessageDialog = new ContentDialog
                {
                    Title = "Attention !",
                    Content = "Vous vous apprêtez à retirer un restaurant ? Êtes-vous sûr d'effectuer cette action ? ",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await deleteMessageDialog.ShowAsync();
                // Créer la candidature si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    string idResto = (cboResto.SelectedItem as Restaurant).IdResto.ToString();
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                          "action=deleteResto" +
                                                          "&idResto=" + idResto);
                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donneesJson["Success"];
                    if (resultat == "true")
                    {
                        var message = new MessageDialog("Restaurant retiré.");
                        await message.ShowAsync();
                        await lesDonnees.ChargerLesDonnees();
                        this.Frame.Navigate(typeof(Page_R_Messages), lesDonnees);
                    }
                    else if (resultat == "false")
                    {
                        var message = new MessageDialog("Impossible de supprimer ce restaurant. Bug");
                        await message.ShowAsync();
                    }
                }
            }
        }
    }
}
