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

        private bool hasSuceeded = false;
        private bool noConnection = false;
        private User user;

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
            IndicatorOn();
            string loginText = LoginEntry.Text;
            string passwordText = PasswordEntry.Text;

            await Task.Run(async () =>
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    //ONLINE LOGIN
                    user = null;
                    LoginManager loginManager = new LoginManager();
                    if (loginText != null && passwordText != null)
                    {

                        //Send login request
                        loginManager.Login = loginText;
                        loginManager.Password = passwordText;
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
                            hasSuceeded = true;
                        }
                    }
                }
                else
                {
                    //OFFLINE LOGIN
                    List<User> logins = JsonParser.LoadLoginJson();
                    if (logins == null)
                    {
                        noConnection = true;
                    }
                    SHA1Managed sha1 = new SHA1Managed();
                    var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(PasswordEntry.Text));
                    String hashedPassword = String.Join("", hash.Select(b => b.ToString("x2")).ToArray());
                    foreach (User u in logins)
                    {
                        if (LoginEntry.Text != null && PasswordEntry.Text != null
                        && LoginEntry.Text.Equals(u.Login)
                        && hashedPassword.Equals(u.Password))
                        {
                            user = u;
                            hasSuceeded = true;
                        }
                    }
                }
            });
            

            if (hasSuceeded)
            {
                await Navigation.PushAsync(new EventsPage(user));
                hasSuceeded = false;
                IndicatorOff();
                return;
            }
            if (noConnection)
            {
                await DisplayAlert("Erreur", "Vous devez vous connecter à la première utilisation.", "OK");
                noConnection = false;
                IndicatorOff();
                return;
            }

            await DisplayAlert("Erreur", "Le nom d'utilisateur ou le mot de passe est erroné", "OK");
            IndicatorOff();
        }

        void IndicatorOn()
        {
            waitingLayout.IsVisible = true;
            activityIndicator.IsRunning = true;
            MainLayout.IsEnabled = false;
            ButtonLogin.IsEnabled = false;
            LoginEntry.IsEnabled = false;
            PasswordEntry.IsEnabled = false;
            ButtonForget.IsEnabled = false;
            ButtonInscription.IsEnabled = false;
            ButtonParameter.IsEnabled = false;
        }

        void IndicatorOff()
        {
            waitingLayout.IsVisible = false;
            activityIndicator.IsRunning = false;
            MainLayout.IsEnabled = true;
            ButtonLogin.IsEnabled = true;
            LoginEntry.IsEnabled = true;
            PasswordEntry.IsEnabled = true;
            ButtonForget.IsEnabled = true;
            ButtonInscription.IsEnabled = true;
            ButtonParameter.IsEnabled = true;
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
