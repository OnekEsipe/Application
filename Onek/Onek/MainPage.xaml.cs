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
                //Check server communication
                if(LoginManager.checkServerCommunication(ApplicationConstants.PingableURL))
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
                        //Wrong credentials
                        else if (httpWebResponse.StatusCode.Equals(HttpStatusCode.Forbidden))
                        {
                            await DisplayAlert("Erreur", "Le nom d'utilisateur ou le mot de passe est erroné", "OK");
                        }
                    }
                    
                }
                //No server connection
                else
                {
                    await DisplayAlert("Erreur", "Impossible de contacter le serveur, vérifiez votre URL dans les paramètres", "OK");
                    return;
                }
            }
            else
            {
                //OFFLINE LOGIN
                List<User> logins = JsonParser.LoadLoginJson();
                if(logins == null)
                {
                    await DisplayAlert("Erreur", "Vous devez vous connecter à la première utilisation.", "OK");
                    return;
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
            String ServerAdress = await InputDialog.InputBox(this.Navigation, title, text, ApplicationConstants.URL);
            if (ServerAdress != null || !ServerAdress.Equals(""))
            {
                //Change server URL in applicationConstants
                ApplicationConstants.URL = ServerAdress;
                //Remove events and evaluation files saved localy
                File.Delete(Path.Combine(ApplicationConstants.jsonDataDirectory, "*"));
                File.Delete(ApplicationConstants.pathToJsonAccountFile);
                File.Delete(Path.Combine(ApplicationConstants.pathToJsonToSend, "*"));
            }

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
