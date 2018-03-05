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
    /// <summary>
    /// Login page
    /// </summary>
	public partial class MainPage : ContentPage
	{
        //Variables
        private bool hasSuceeded = false;
        private bool noConnection = false;
        private bool isError = false;
        private bool noServer = false;
        private User user;

        /// <summary>
        /// MainPage constructor
        /// </summary>
        public MainPage()
	    {
			InitializeComponent();
        }

        /// <summary>
        /// Executed when page is displayed
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoginEntry.Text = "";
            PasswordEntry.Text = "";
            IndicatorOff();
        }

        /// <summary>
        /// Executed when user clicks on button login, check login information and redirect user to events page if
        /// login is OK
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        async void OnButtonLoginClicked(object sender, EventArgs e)
        {
            if (ApplicationConstants.URL == null || ApplicationConstants.URL.Equals(""))
            {
                await DisplayAlert("Erreur", "Vous devez renseigner l'url du serveur en cliquant sur le bouton paramètres serveur", "OK");
                return;
            }
            IndicatorOn();
            string loginText = LoginEntry.Text;
            string passwordText = PasswordEntry.Text;
            await Task.Run(() =>
            {
                //Hash password
                SHA1Managed sha1 = new SHA1Managed();
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(passwordText));
                String hashedPassword = String.Join("", hash.Select(b => b.ToString("x2")).ToArray());
                //ONLINE LOGIN
                user = null;
                LoginManager loginManager = new LoginManager();
                if (loginText != null && hashedPassword != null)
                {
                    //Send login request
                    loginManager.Login = loginText;
                    loginManager.Password = hashedPassword;
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
                    //Wrong credentials
                    else if (httpWebResponse != null && httpWebResponse.StatusCode.Equals(HttpStatusCode.Forbidden))
                    {
                        isError = true;
                    }
                    //No server connection
                    else
                    {
                        //OFFLINE LOGIN
                        List<User> logins = JsonParser.LoadLoginJson();
                        if (logins == null)
                        {
                            noConnection = true;
                        }
                        else
                        {
                            foreach (User u in logins)
                            {
                                if (loginText != null && passwordText != null
                                && loginText.Equals(u.Login)
                                && hashedPassword.Equals(u.Password))
                                {
                                    user = u;
                                    hasSuceeded = true;
                                }
                            }
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
            if (isError)
            {
                await DisplayAlert("Erreur", "Le nom d'utilisateur ou le mot de passe est erroné", "OK");
                isError = false;
                IndicatorOff();
                return;
            }

            if (noServer)
            {
                await DisplayAlert("Erreur", "Impossible de contacter le serveur, vérifiez votre URL dans les paramètres", "OK");
                noServer = false;
                IndicatorOff();
                return;
            }

            await DisplayAlert("Erreur", "Le nom d'utilisateur ou le mot de passe est erroné", "OK");
            IndicatorOff();
        }

        /// <summary>
        /// Display the loading indicator when the login request is processing
        /// </summary>
        void IndicatorOn()
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                waitingLayoutPhone.IsVisible = true;
                activityIndicatorPhone.IsRunning = true;
            }
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                waitingLayoutTablet.IsVisible = true;
                activityIndicatorTablet.IsRunning = true;
            }
            MainLayout.IsEnabled = false;
            ButtonLogin.IsEnabled = false;
            LoginEntry.IsEnabled = false;
            PasswordEntry.IsEnabled = false;
            ButtonForget.IsEnabled = false;
            ButtonInscription.IsEnabled = false;
            ButtonParameter.IsEnabled = false;
        }

        /// <summary>
        /// Remove the login indicator
        /// </summary>
        void IndicatorOff()
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                waitingLayoutPhone.IsVisible = false;
                activityIndicatorPhone.IsRunning = false;
            }
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                waitingLayoutTablet.IsVisible = false;
                activityIndicatorTablet.IsRunning = false;
            }
            MainLayout.IsEnabled = true;
            ButtonLogin.IsEnabled = true;
            LoginEntry.IsEnabled = true;
            PasswordEntry.IsEnabled = true;
            ButtonForget.IsEnabled = true;
            ButtonInscription.IsEnabled = true;
            ButtonParameter.IsEnabled = true;
        }
        
        /// <summary>
        /// Display a pop-up to change the server url
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        async void OnButtonParameterClicked(object sender, EventArgs e)
        {
            string title = "Changement de serveur";
            string text = "Entrez un URL : ";
            String ServerAdress = await InputDialog.InputBox(this.Navigation, title, text, ApplicationConstants.URL);
            if (ServerAdress != null && !ServerAdress.Equals(""))
            {
                //Change server URL in applicationConstants
                ApplicationConstants.URL = ServerAdress;
                //Remove events and evaluation files saved localy
                File.Delete(Path.Combine(ApplicationConstants.jsonDataDirectory, "*"));
                if(File.Exists(ApplicationConstants.pathToJsonAccountFile))
                    File.Delete(ApplicationConstants.pathToJsonAccountFile);
                if(Directory.Exists(ApplicationConstants.pathToJsonToSend))
                    File.Delete(Path.Combine(ApplicationConstants.pathToJsonToSend, "*"));
            }

        }

        /// <summary>
        /// Open page to register as a jury
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        async void OnButtonInscriptionClicked(object sender, EventArgs e)
        {
            if (ApplicationConstants.URL == null || ApplicationConstants.URL.Equals(""))
            {
                await DisplayAlert("Erreur", "Vous devez renseigner l'url du serveur en cliquant sur le bouton paramètres serveur", "OK");
                return;
            }
            if (CrossConnectivity.Current.IsConnected)
            {
                await Navigation.PushAsync(new InscriptionPage());
            }
            else
            {
                await DisplayAlert("Erreur", "Vous devez être en ligne pour utiliser cette fonctionnalité", "OK");
            }
        }

        /// <summary>
        /// Reset password, Display a pop-up where the user fill his mail address and
        /// send a request to the server to receive an e-mail to reset password
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        async void onButtonOublieClicked(object sender, EventArgs e)
        {
            if (ApplicationConstants.URL == null || ApplicationConstants.URL.Equals(""))
            {
                await DisplayAlert("Erreur", "Vous devez renseigner l'url du serveur en cliquant sur le bouton paramètres serveur", "OK");
                return;
            }
            IndicatorOn();

            string title = "Récupération de mot de passe";
            string text = "Entrez votre adresse mail :";
            if (CrossConnectivity.Current.IsConnected)
            {
                //Open pop-up
                String Mail = await InputDialog.InputBox(this.Navigation, title, text, "");
                //if mail is not valid
                if (Mail != null && !Mail.Equals("") && CreateAccountManager.CheckMail(Mail) == false)
                {
                    await DisplayAlert("Erreur", "Adresse mail invalide", "OK");
                }
                else if (Mail != null && !Mail.Equals(""))
                {
                    String jsonResetPassword = "{ \"Mail\" : \"" + Mail + "\" }";
                    //Send reset password request
                    try
                    {
                        HttpWebRequest webRequest = WebRequest.Create(ApplicationConstants.serverResetPasswordURL) as HttpWebRequest;
                        webRequest.ContentType = "application/json";
                        webRequest.Method = "POST";
                        JsonParser.SendToServer(webRequest, jsonResetPassword);
                        HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse;
                        //If response OK, quit
                        if (webResponse.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            await DisplayAlert("Mot de passe oublié", "Vous allez recevoir un e-mail pour réinitialiser votre mot de passe", "OK");
                            await Navigation.PopAsync();
                        }
                    }
                    catch (Exception exception)
                    {
                        WebException webException = exception as WebException;
                        HttpWebResponse response = webException.Response as HttpWebResponse;
                        //If conflict, warn user
                        if (response.StatusCode.Equals(HttpStatusCode.Conflict))
                        {
                            await DisplayAlert("Erreur", "Aucun compte n'est rattaché à cette adresse email", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Erreur", "Erreur lors de l'envoi ou du traitement de la requête", "OK");
                        }
                    }
                }
            }
            else
            {
                await DisplayAlert("Erreur", "Vous devez être en ligne pour utiliser cette fonctionnalité", "OK");
            }

            IndicatorOff();
        }
    }
}
