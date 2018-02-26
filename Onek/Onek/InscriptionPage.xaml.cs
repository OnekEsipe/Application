using Newtonsoft.Json;
using Onek.data;
using Onek.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Onek
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InscriptionPage : ContentPage
	{
        private bool wrongCompleted { get; set; }
        private bool wrongPassword { get; set; }
        private bool wrongMail { get; set; }
        private bool succeed { get; set; }
        private bool error { get; set; }
        private bool startAnonym { get; set; }

        public InscriptionPage ()
		{
			InitializeComponent ();
		}


        /// <summary>
        /// Create a jury account 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnButtonInscriptionClicked(object sender, EventArgs e)
        {

            IndicatorOn();
            string loginText = LoginEntry.Text;
            string passwordText = PasswordEntry.Text;
            string mailText = MailEntry.Text;
            string nomText = NomEntry.Text;
            string prenomText = PrenomEntry.Text;

            String errorMessage = "";

            error = false;
            succeed = false;
            wrongPassword = false;
            wrongMail = false;
            wrongCompleted = false;
            startAnonym = false;

            await Task.Run(async () =>
            {
                CreateAccountManager accountManager = new CreateAccountManager();
                if (loginText == null || nomText == null || prenomText == null ||
                    mailText == null || passwordText == null || loginText.Equals("") ||
                    nomText.Equals("") || prenomText.Equals("") || mailText.Equals("") ||
                    passwordText.Equals(""))
                {
                    wrongCompleted = true;
                    return;
                }
                if(loginText.ToLower().StartsWith("jury"))
                {
                    startAnonym = true;
                    return;
                }
                if (CreateAccountManager.CheckPassword(PasswordEntry.Text) == false)
                {
                    wrongPassword = true;
                    return;
                }
                if (CreateAccountManager.CheckMail(MailEntry.Text) == false)
                {
                    wrongMail = true;
                    return;
                }
                accountManager.Login = LoginEntry.Text;
                accountManager.Lastname = NomEntry.Text;
                accountManager.Firstname = PrenomEntry.Text;
                accountManager.Mail = MailEntry.Text;
                //Compute password hash (SHA1)
                SHA1Managed sha1 = new SHA1Managed();
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(PasswordEntry.Text));
                String hashedPassword = String.Join("", hash.Select(b => b.ToString("x2")).ToArray());
                accountManager.Password = hashedPassword;
                //Create json 
                String createAccountJson = JsonConvert.SerializeObject(accountManager);
                //Send json
                try
                {
                    HttpWebRequest webRequest = WebRequest.Create(ApplicationConstants.serverCreateAccountURL) as HttpWebRequest;
                    webRequest.ContentType = "application/json";
                    webRequest.Method = "POST";
                    JsonParser.SendToServer(webRequest, createAccountJson);
                    HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse;
                    //If response OK, quit
                    if (webResponse.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        succeed = true;
                        return;
                    }
                }
                catch (Exception exception)
                {
                    WebException webException = exception as WebException;
                    HttpWebResponse response = webException.Response as HttpWebResponse;
                    //If conflict, warn user
                    if (response.StatusCode.Equals(HttpStatusCode.Conflict))
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                        errorMessage = reader.ReadToEnd();
                        error = true;
                        return;
                    }
                }
            });

            if(error)
            {
                await DisplayAlert("Erreur", errorMessage, "OK");
            }
            if(succeed)
            {
                await DisplayAlert("Création de compte", "Votre compte a bien été créé", "OK");
                IndicatorOff();
                await Navigation.PopAsync();
            }
            if(wrongCompleted)
            {
                await DisplayAlert("Erreur", "Tous les champs doivent être renseignés", "OK");
            }
            if(wrongMail)
            {
                await DisplayAlert("Erreur", "Adresse mail invalide", "OK");
            }
            if(wrongPassword)
            {
                await DisplayAlert("Erreur", "Le mot de passe doit contenir au moins 6 caractères dont au moins une lettre en majuscule", "OK");
            }
            if(startAnonym)
            {
                await DisplayAlert("Erreur", "Le login ne peut pas commencer par 'Jury'", "OK");
            }

            IndicatorOff();
        }

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
            LoginEntry.IsEnabled = false;
            MailEntry.IsEnabled = false;
            PasswordEntry.IsEnabled = false;
            NomEntry.IsEnabled = false;
            PrenomEntry.IsEnabled = false;
            InscriptionButton.IsEnabled = false;
        }

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
            LoginEntry.IsEnabled = true;
            MailEntry.IsEnabled = true;
            PasswordEntry.IsEnabled = true;
            NomEntry.IsEnabled = true;
            PrenomEntry.IsEnabled = true;
            InscriptionButton.IsEnabled = true;
        }
    }
}