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


	}
}
