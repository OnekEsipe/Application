using Onek.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Onek
{
	public partial class MainPage : ContentPage
	{
        private String loginTest = "a";
        private String passTest = "a";

        public MainPage()
		{
			InitializeComponent();
		}

        async void OnButtonLoginClicked(object sender, EventArgs e)
        {
            if (LoginEntry.Text != null && PasswordEntry.Text != null 
                && LoginEntry.Text.Equals(loginTest) && PasswordEntry.Text.Equals(passTest))
            {
                await Navigation.PushAsync(new EventsPage());
            }
            else
            {
                await DisplayAlert("Erreur", "Le nom d'utilisateur ou le mot de passe est erroné", "OK");
            }
        }

        async void OnButtonParameterClicked(object sender, EventArgs e)
        {
            string title = "Changement de serveur";
            string text = "Entrez une adresse : ";
            String ServerAdress = await InputDialog.InputBox(this.Navigation, title, text, "");

            // Check if Server Adress is OK and Change it in Settings App
        }

        async void OnButtonInscriptionClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InscriptionPage());
        }

        async void onButtonOublieClicked(object sender, EventArgs e)
        {
            string title = "Récupétation de mot de passe";
            string text = "Entrez votre adresse mail :";
            String Mail = await InputDialog.InputBox(this.Navigation, title, text, "");

            //Faire demande envoi de mail
        }
    }
}
