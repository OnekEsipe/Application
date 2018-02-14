using Onek.data;
using Onek.utils;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Onek
{
	public partial class MainPage : ContentPage
	{
       public MainPage()
		{
			InitializeComponent();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoginEntry.Text = "";
            PasswordEntry.Text = "";
        }

        async void OnButtonLoginClicked(object sender, EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                //ONLINE LOGIN
                User user = null;
                LoginManager loginManager = new LoginManager();
                if (LoginEntry.Text != null && PasswordEntry != null)
                {
                    //Send login request
                    loginManager.Login = LoginEntry.Text;
                    loginManager.Password = PasswordEntry.Text;
                    String loginJson = loginManager.GenerateLoginJson();
                    HttpWebResponse httpWebResponse = loginManager.SendAuthenticationRequest(loginJson);
                    //Check login response
                    if (httpWebResponse != null && httpWebResponse.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        //Get user json account
                        Stream responseStream = httpWebResponse.GetResponseStream();
                        StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                        String jsonAccount = streamReader.ReadToEnd();
                        List<User> users = JsonParser.DeserializeJsonAccount(jsonAccount);
                        user = users.First();
                        //Save user in local jsonAccount
                        JsonParser.SaveJsonAccountInMemory(users);
                        //Display event page
                        await Navigation.PushAsync(new EventsPage(user));
                        return;
                    }
                }
            }
            else
            {
                //OFFLINE LOGIN
                List<User> logins = JsonParser.LoadLoginJson();
                SHA1Managed sha1 = new SHA1Managed();
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(PasswordEntry.Text));
                String hashedPassword = String.Join("", hash.Select(b => b.ToString("x2")).ToArray());
                foreach (User u in logins)
                {
                    if (LoginEntry.Text != null && PasswordEntry.Text != null
                    && LoginEntry.Text.Equals(u.Login) 
                    && hashedPassword.Equals(u.Password))
                    {

                        await Navigation.PushAsync(new EventsPage(u));
                        return;
                    }
                }
            }

            await DisplayAlert("Erreur", "Le nom d'utilisateur ou le mot de passe est erroné", "OK");
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
