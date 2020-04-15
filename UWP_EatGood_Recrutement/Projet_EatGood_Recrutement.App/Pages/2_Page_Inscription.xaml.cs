using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private async void BtnSinscrire_Click(object sender, RoutedEventArgs e)
        {
            if(txtLogin.Text == "")
            {
                var message = new MessageDialog("Veuillez entrer un login.");
                await message.ShowAsync();
            } else if (txtMdp.Password == "")
            {
                var message = new MessageDialog("Veuillez entrer un mot de passe.");
                await message.ShowAsync();
            } else if (txtMdp.Password.Length < 6 || txtMdp.Password.StartsWith("123"))
            {
                var message = new MessageDialog("Veuillez entrer un mot de passe plus compliqué (6 caractères au moins).");
                await message.ShowAsync();
            }
        }
    }
}
