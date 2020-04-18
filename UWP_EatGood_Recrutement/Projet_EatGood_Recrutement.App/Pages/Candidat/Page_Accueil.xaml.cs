﻿using Newtonsoft.Json;
using Projet_EatGood_Recrutement.App.API;
using Projet_EatGood_Recrutement.App.Pages.Candidat;
using Projet_EatGood_Recrutement.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            hc = new HttpClient();
            List<Candidature> lesCandidaturesDuUser = await lesDonnees.GetLesCandidaturesByCandidat(lutilisateurActuellement.IdUtilisateur);
            if (lesCandidaturesDuUser.Count == 0)
            {
                txtYaTilDesCandidatures.Visibility = Visibility.Visible;
                lvCandidaturesCandidat.Visibility = Visibility.Collapsed;
            } 
            else
            {
                lvCandidaturesCandidat.ItemsSource = lesCandidaturesDuUser;
            }
        }

        private void lvCandidaturesCandidat_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ShowEtatDeLaCandidature();
        }

        public async void ShowEtatDeLaCandidature()
        {
            if (lvCandidaturesCandidat.SelectedItem != null)
            {
                Candidature laCandidatureChoisie = lvCandidaturesCandidat.SelectedItem as Candidature;
                if (laCandidatureChoisie.StatutMessage == "En attente")
                {
                    var message = new MessageDialog("Cette candidature est encore en attente, le recruteur du restaurant "
                                                    + laCandidatureChoisie.LeResto.LibelleResto + " n'a pas encore examiné votre candidature.");
                    await message.ShowAsync();
                }
                else if (laCandidatureChoisie.StatutMessage == "En cours")
                {
                    var message = new MessageDialog("Cette candidature est encore en cours d'examen, le recruteur du restaurant "
                                                    + laCandidatureChoisie.LeResto.LibelleResto + " est entrain de l'étudier.");
                    await message.ShowAsync();
                }
                else if (laCandidatureChoisie.StatutMessage == "Refusée")
                {
                    var message = new MessageDialog(laCandidatureChoisie.MessageReponse);
                    await message.ShowAsync();
                    //Bonjour,
                    //Nous vous remercions d'avoir postulé au poste STAGE - Développement Logiciel Applicatif de Monitoring Hardware F/H. Après un examen attentif de votre candidature, nous regrettons de vous informer qu'elle n'a pas été sélectionnée pour ce poste.
                    //Nous pourrons vous contacter pour de futures opportunités et vous encourageons à nous rejoindre sur les réseaux sociaux.
                    //Nous vous remercions de votre intérêt pour Thales et vous souhaitons le meilleur dans votre carrière.
                    //Meilleures salutations,
                    //L'équipe Ressources Humaines Thales
                }
                else if (laCandidatureChoisie.StatutMessage == "En attente d'entretien")
                {
                    var message = new MessageDialog(laCandidatureChoisie.MessageReponse);
                    await message.ShowAsync();
                }
            }
            else
            {
                var message = new MessageDialog("Veuillez sélectionner une candidature");
                await message.ShowAsync();
            }
        }
        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (lvCandidaturesCandidat.SelectedItem != null)
            {
                Candidature laCandidChoisie = lvCandidaturesCandidat.SelectedItem as Candidature;
                Restaurant leRestoChoisi = laCandidChoisie.LeResto;
                Poste lePosteChoisi = laCandidChoisie.LePosteVoulu;
                ContentDialog DeleteCandidatureDialog = new ContentDialog
                {
                    Title = "Attention !",
                    Content = "Vous vous apprêtez à supprimer votre candidature.\n" +
                              "Êtes-vous sûr(e) de vouloir supprimer votre candidature au restaurant " + leRestoChoisi.LibelleResto +
                              " au poste de " + lePosteChoisi.LibellePoste + " ?",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await DeleteCandidatureDialog.ShowAsync();
                // Supprime la candidature si l'utilisateur a cliqué sur le bouton principal ("oui")
                // Sinon, rien faire.
                if (result == ContentDialogResult.Primary)
                {
                    // action=deleteCandidature&idCandidature={idCandidature}
                    string idCandidature = laCandidChoisie.IdCandidature.ToString();
                    var reponse = await hc.GetStringAsync("http://localhost/recru_eatgood_api/index_candidat.php?" +
                                                          "action=deleteCandidature" +
                                                          "&idCandidature=" + idCandidature);
                    var donneesJson = JsonConvert.DeserializeObject<dynamic>(reponse);
                    var resultat = donneesJson["Success"];
                    if (resultat == "true")
                    {
                        var message = new MessageDialog("Vous avez supprimé votre candidature chez le restaurant " +
                                                        leRestoChoisi.LibelleResto + " au poste de " + lePosteChoisi.LibellePoste);
                        await message.ShowAsync();
                        await lesDonnees.ChargerLesDonnees();
                        this.Frame.Navigate(typeof(Page_Accueil), lesDonnees);
                    }
                    else
                    {
                        var message = new MessageDialog("Cette candidature n'existe pas. Ou alors il y a une erreur dans le code");
                        await message.ShowAsync();
                    }
                }                
            }
            else
            {
                var message = new MessageDialog("! Avant de supprimer une candidature, il faut d'abord en selectionner une !");
                await message.ShowAsync();
            }
        }

        private void BtnProfil_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_Profil), lesDonnees);

        }

        private void BtnMesMessages_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_Messages), lesDonnees);

        }

        private void BtnNvelleCandidature_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_NouvelleCandidature), lesDonnees);
        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {

            this.Frame.Navigate(typeof(MainPage));
        }

        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page_Accueil), lesDonnees);
        }        
    }
}
