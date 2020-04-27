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
    public sealed partial class Page_R_Backoffice : Page
    {
        public Page_R_Backoffice()
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
                txtBienvenue.Text = "Back Office";
            }
            else
            {
                txtBienvenue.Text = "Back Office";
            }
            base.OnNavigatedTo(e);
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            hc = new HttpClient();
            lvUtilisateurs.ItemsSource = lesDonnees.lesUtilisateurs;
            lvPostes.ItemsSource = lesDonnees.lesPostes;
            lvChoixPosteChangeQte.ItemsSource = lesDonnees.lesPostes;
            cboChoixResto.ItemsSource = lesDonnees.lesResto;
            recuperationDesNecessites();

        }
        public async void recuperationDesNecessites()
        {
            await lesDonnees.GetAllNecessites();
        }

        private async Task<string> InputTextDialogAsync(string title)
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 200;
            inputTextBox.Width = 400;
            inputTextBox.TextWrapping = TextWrapping.Wrap;
            inputTextBox.Text = "";
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
                return "annuler";
        }
        public async Task<string> CboBoxDialogAsync(string title)
        {
            ComboBox inputCombobox = new ComboBox();
            inputCombobox.Items.Add("candidat");
            inputCombobox.Items.Add("recruteur");
            inputCombobox.PlaceholderText = "Choisissez un role";
            inputCombobox.Width = 300;

            ContentDialog dialog = new ContentDialog();
            dialog.Width = 600;
            dialog.Content = inputCombobox;
            dialog.Title = title;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Valider";
            dialog.SecondaryButtonText = "Annuler";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (inputCombobox.SelectedItem as string == null)
                {
                    var message = new MessageDialog("Veuillez choisir un rôle.");
                    await message.ShowAsync();
                    return await CboBoxDialogAsync(title);
                }
                else
                {
                    return (inputCombobox.SelectedItem as string);
                }
            }
            else
            {
                return "annuler";
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
        private void BtnBackOffice_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_R_Backoffice), lesDonnees);
        }
        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void BtnSupprimerUtilisateur_Click(object sender, RoutedEventArgs e)
        {
            if(lvUtilisateurs.SelectedItem == null)
            {
                var message = new MessageDialog("Veuillez d'abord choisir un utilisateur.");
                await message.ShowAsync();
            }
            else
            {
                ContentDialog deleteUserDialog = new ContentDialog
                {
                    Title = "Attention !",
                    Content = "Vous vous apprêtez à utilisateur de cette application ? Êtes-vous sûr d'effectuer cette action ? ",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await deleteUserDialog.ShowAsync();
                // Supprime l'utilisateur si il a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    string idUser = (lvUtilisateurs.SelectedItem as Utilisateur).IdUtilisateur.ToString();
                    // http://localhost/recru_eatgood_api/index_recruteur.php?action=deleteUser&idUser=10
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                          "action=deleteUser" +
                                                          "&idUser=" + idUser);
                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donneesJson["Success"];
                    if (resultat == "true")
                    {
                        var message = new MessageDialog("Utilisateur supprimé.");
                        await message.ShowAsync();
                        await lesDonnees.ChargerLesDonnees();
                        this.Frame.Navigate(typeof(Page_R_Backoffice), lesDonnees);
                    }
                    else if (resultat == "false")
                    {
                        var message = new MessageDialog("Impossible de supprimer cet utilisateur. Bug");
                        await message.ShowAsync();
                    }
                }
            }
        }

        private async void BtnModifierStatut_Click(object sender, RoutedEventArgs e)
        {
            if (lvUtilisateurs.SelectedItem == null)
            {
                var message = new MessageDialog("Veuillez d'abord choisir un utilisateur.");
                await message.ShowAsync();
            }
            else
            {
                string textRole = await CboBoxDialogAsync("Nouveau rôle");
                if (textRole == "")
                {
                    var message = new MessageDialog("Veuillez choisir un rôle");
                    await message.ShowAsync();
                }
                else if(textRole != "annuler")
                {
                    Utilisateur lutilisateurChoisi = lvUtilisateurs.SelectedItem as Utilisateur;
                    if(lutilisateurChoisi.RoleUtilisateur == textRole)
                    {
                        var message = new MessageDialog("L'utilisateur possède déjà ce rôle");
                        await message.ShowAsync();
                    }
                    else
                    {
                        ContentDialog deleteUserDialog = new ContentDialog
                        {
                            Title = "Attention !",
                            Content = "Vous vous apprêtez à modifier le rôle de cet utilisateur ? Êtes-vous sûr d'effectuer cette action ? ",
                            PrimaryButtonText = "Oui",
                            CloseButtonText = "Non"
                        };

                        ContentDialogResult result = await deleteUserDialog.ShowAsync();
                        // Modifie le role de l'utilisateur si il a cliqué sur le bouton principal ("oui")
                        // Sinon, rien faire.
                        if (result == ContentDialogResult.Primary)
                        {
                            string idUser = lutilisateurChoisi.IdUtilisateur.ToString();
                            string newRole = textRole;
                            // http://localhost/recru_eatgood_api/index_recruteur.php?action=updateRoleUser&idUser=10&new_role=recruteur
                            var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                                  "action=updateRoleUser" +
                                                                  "&idUser=" + idUser +
                                                                  "&new_role=" + newRole);
                            var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                            var resultat = donneesJson["Success"];
                            if (resultat == "true")
                            {
                                var message = new MessageDialog("Rôle modifié.");
                                await message.ShowAsync();
                                await lesDonnees.ChargerLesDonnees();
                                this.Frame.Navigate(typeof(Page_R_Backoffice), lesDonnees);
                            }
                            else if (resultat == "false")
                            {
                                var message = new MessageDialog("Impossible de modifier cet utilisateur. Bug");
                                await message.ShowAsync();
                            }
                        }
                    }
                }

            }
        }

        private async void BtnAjouterPoste_Click(object sender, RoutedEventArgs e)
        {
            string nouveauPoste = await InputTextDialogAsync("Nouveau poste");
            if(nouveauPoste == "")
            {
                var message = new MessageDialog("Vous devez rentrer un nom de poste.");
                await message.ShowAsync();
                BtnAjouterPoste_Click(sender, e);
            }
            else if (nouveauPoste != "annuler")
            {
                ContentDialog newPosteDialog = new ContentDialog
                {
                    Title = "Attention !",
                    Content = "Vous vous apprêtez à ajouter un nouveau poste ? Êtes-vous sûr d'effectuer cette action ? ",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await newPosteDialog.ShowAsync();
                // Ajoute un poste si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                          "action=createPoste" +
                                                          "&libellePoste=" + nouveauPoste);
                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donneesJson["Success"];
                    if (resultat == "true")
                    {
                        var message = new MessageDialog("Poste ajouté.");
                        await message.ShowAsync();
                        await lesDonnees.ChargerLesDonnees();
                        this.Frame.Navigate(typeof(Page_R_Backoffice), lesDonnees);
                    }
                    else if (resultat == "false")
                    {
                        var message = new MessageDialog("Impossible d'ajouter un poste. Bug");
                        await message.ShowAsync();
                    }
                }
            }
        }

        private async void BtnSupprimerPoste_Click(object sender, RoutedEventArgs e)
        {
            if(lvPostes.SelectedItem == null)
            {
                var message = new MessageDialog("Il faut choisir un poste pour pouvoir en supprimer un !");
                await message.ShowAsync();
            }
            else
            {
                ContentDialog DeletePosteDialog = new ContentDialog
                {
                    Title = "Attention !",
                    Content = "Vous vous apprêtez à supprimer un poste ? Êtes-vous sûr d'effectuer cette action ? ",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await DeletePosteDialog.ShowAsync();
                // Ajoute un poste si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    Poste lePoste = (lvPostes.SelectedItem as Poste);
                    string idPoste = lePoste.IdPoste.ToString();
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                          "action=deletePoste" +
                                                          "&idPoste=" + idPoste);
                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donneesJson["Success"];
                    if (resultat == "true")
                    {
                        var message = new MessageDialog("Poste supprimé : '"+ lePoste.LibellePoste +"'.");
                        await message.ShowAsync();
                        await lesDonnees.ChargerLesDonnees();
                        this.Frame.Navigate(typeof(Page_R_Backoffice), lesDonnees);
                    }
                    else if (resultat == "false")
                    {
                        var message = new MessageDialog("Impossible de supprimer ce poste. Bug");
                        await message.ShowAsync();
                    }
                }
            }
        }

        private async void BtnModifierPoste_Click(object sender, RoutedEventArgs e)
        {
            if (lvPostes.SelectedItem == null)
            {
                var message = new MessageDialog("Il faut choisir un poste pour pouvoir en modifier un !");
                await message.ShowAsync();
            }
            else
            {
                string nouveauLibellePoste = await InputTextDialogAsync("Nouveau poste");
                if(nouveauLibellePoste == "")
                {
                    var message = new MessageDialog("Veuillez remplir le champs !");
                    await message.ShowAsync();
                    BtnModifierPoste_Click(sender, e);
                }
                else if(nouveauLibellePoste != "annuler")
                {
                    ContentDialog DeletePosteDialog = new ContentDialog
                    {
                        Title = "Attention !",
                        Content = "Vous vous apprêtez à modifier le nom d'un poste ? Êtes-vous sûr d'effectuer cette action ? ",
                        PrimaryButtonText = "Oui",
                        CloseButtonText = "Non"
                    };

                    ContentDialogResult result = await DeletePosteDialog.ShowAsync();
                    // Modifie un poste si l'utilisateur a cliqué sur le bouton principal ("oui")
                    // Sinon, rien faire.
                    if (result == ContentDialogResult.Primary)
                    {
                        Poste lePoste = (lvPostes.SelectedItem as Poste);
                        string idPoste = lePoste.IdPoste.ToString();
                        var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                              "action=updatePoste" +
                                                              "&idPoste=" + idPoste +
                                                              "&new_libelle=" + nouveauLibellePoste);
                        var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                        var resultat = donneesJson["Success"];
                        if (resultat == "true")
                        {
                            var message = new MessageDialog("Poste modifié : '"+ lePoste.LibellePoste +"' à '" + nouveauLibellePoste + "'.");
                            await message.ShowAsync();
                            await lesDonnees.ChargerLesDonnees();
                            this.Frame.Navigate(typeof(Page_R_Backoffice), lesDonnees);
                        }
                        else if (resultat == "false")
                        {
                            var message = new MessageDialog("Impossible de modifier ce poste. Bug");
                            await message.ShowAsync();
                        }
                    }
                }
            }
        }

        private async void cboChoixResto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboChoixResto.SelectedItem == null)
            {
                var message = new MessageDialog("Vous devez choisir un restaurant afin d'afficher ses nécessités.");
                await message.ShowAsync();
            }
            else
            {
                lvNecessites.ItemsSource = lesDonnees.GetNecessitesByResto(cboChoixResto.SelectedItem as Restaurant);
            }
        }

        private async void BtnModifierQuantité_Click(object sender, RoutedEventArgs e)
        {
            if(lvNecessites.SelectedItem == null)
            {
                var message = new MessageDialog("Vous devez choisir une nécessité afin de modifier sa quantité.");
                await message.ShowAsync();
            }
            else
            {
                string nouvelleQuantite = await InputTextDialogAsync("Changement de quantité");
                if (nouvelleQuantite == "")
                {
                    var message = new MessageDialog("Veuillez remplir le champs !");
                    await message.ShowAsync();
                    BtnModifierQuantité_Click(sender, e);
                }
                else if(!IsNumeric(nouvelleQuantite) && nouvelleQuantite != "annuler")
                {
                    var message = new MessageDialog("Veuillez entrer un nombre !");
                    await message.ShowAsync();
                    BtnModifierQuantité_Click(sender, e);
                }
                else if (nouvelleQuantite != "annuler")
                {
                    ContentDialog ChangeQteNecessiteDialog = new ContentDialog
                    {
                        Title = "Attention !",
                        Content = "Vous vous apprêtez à modifier la quantité d'une nécessité ? Êtes-vous sûr d'effectuer cette action ? ",
                        PrimaryButtonText = "Oui",
                        CloseButtonText = "Non"
                    };

                    ContentDialogResult result = await ChangeQteNecessiteDialog.ShowAsync();
                    // Modifie une necessité si l'utilisateur a cliqué sur le bouton principal ("oui")
                    // Sinon, rien faire.
                    if (result == ContentDialogResult.Primary)
                    {
                        Necessites laNecessite = (lvNecessites.SelectedItem as Necessites);
                        string idPoste = laNecessite.LePoste.IdPoste.ToString();
                        string idResto = laNecessite.LeResto.IdResto.ToString();
                        var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                              "action=updateNecessite" +
                                                              "&idPoste=" + idPoste +
                                                              "&idResto=" + idResto +
                                                              "&new_quantite=" + nouvelleQuantite);
                        var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                        var resultat = donneesJson["Success"];
                        if (resultat == "true")
                        {
                            var message = new MessageDialog("Nécessité modifiée : " + laNecessite.LePoste.LibellePoste + " => de "+ laNecessite.LaQuantite +" à " + nouvelleQuantite + ".");
                            await message.ShowAsync();
                            //await lesDonnees.GetAllNecessites();
                            this.Frame.Navigate(typeof(Page_R_Backoffice), lesDonnees);
                        }
                        else if (resultat == "false")
                        {
                            var message = new MessageDialog("Impossible de modifier cet nécessité. Bug");
                            await message.ShowAsync();
                        }
                    }
                }
            }
        }
        public bool IsNumeric(string Nombre)
        {
            try
            {
                int.Parse(Nombre);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async void BtnAjoutNecessite_Click(object sender, RoutedEventArgs e)
        {
            if(cboChoixResto.SelectedItem == null)
            {
                var message = new MessageDialog("Veuillez choisir un restaurant.");
                await message.ShowAsync();
            }
            else if(lvChoixPosteChangeQte.SelectedItem == null)
            {
                var message = new MessageDialog("Veuillez choisir un poste.");
                await message.ShowAsync();
            }
            else if (txtQuantité.Text == "")
            {
                var message = new MessageDialog("Veuillez entrer une quantité nécessaire au poste de "+ (lvChoixPosteChangeQte.SelectedItem as Poste).LibellePoste +".");
                await message.ShowAsync();
            }
            else if(!IsNumeric(txtQuantité.Text)) 
            {
                var message = new MessageDialog("Veuillez entrer un nombre.");
                await message.ShowAsync();
            }
            else
            {
                string quantité = txtQuantité.Text;
                Poste lePoste = lvChoixPosteChangeQte.SelectedItem as Poste;
                Restaurant leResto = cboChoixResto.SelectedItem as Restaurant;
                ContentDialog CreateNecessiteDialog = new ContentDialog
                {
                    Title = "Attention !",
                    Content = "Vous vous apprêtez à ajouter nécessité de "+ quantité +" pour le poste de "+ lePoste.LibellePoste +" " +
                    " au restaurant "+ leResto.LibelleResto +" ? Êtes-vous sûr d'effectuer cette action ? ",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await CreateNecessiteDialog.ShowAsync();
                // Ajoute une necessité si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    string idPoste = lePoste.IdPoste.ToString();
                    string idResto = leResto.IdResto.ToString();
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_recruteur.php?" +
                                                          "action=createNecessite" +
                                                          "&idPoste=" + idPoste +
                                                          "&idResto=" + idResto +
                                                          "&quantite=" + quantité);
                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donneesJson["Success"];
                    if (resultat == "true")
                    {
                        var message = new MessageDialog("Nécessité ajouté pour le restaurant "+ leResto.LibelleResto +" : " 
                                                    + lePoste.LibellePoste + " => " + quantité + " unités.");
                        await message.ShowAsync();
                        //await lesDonnees.GetAllNecessites();
                        this.Frame.Navigate(typeof(Page_R_Backoffice), lesDonnees);
                    }
                    else if (resultat == "false")
                    {
                        var message = new MessageDialog("Impossible d'ajouter une nécessité. Bug");
                        await message.ShowAsync();
                    }
                }
            }
        }
    }
}
