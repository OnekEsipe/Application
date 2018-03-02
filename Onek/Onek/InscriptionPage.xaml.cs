using Newtonsoft.Json;
using Onek.data;
using Onek.utils;
using System;
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
        //Properties
        private bool WrongCompleted { get; set; }
        private bool WrongPassword { get; set; }
        private bool WrongMail { get; set; }
        private bool Succeed { get; set; }
        private bool Error { get; set; }
        private bool StartAnonym { get; set; }

        /// <summary>
        /// InscriptionPage constructor
        /// </summary>
        public InscriptionPage ()
		{
			InitializeComponent ();
		}


        /// <summary>
        /// Create a jury account by sending account information to the server
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

            Error = false;
            Succeed = false;
            WrongPassword = false;
            WrongMail = false;
            WrongCompleted = false;
            StartAnonym = false;

            await Task.Run(async () =>
            {
                CreateAccountManager accountManager = new CreateAccountManager();
                if (loginText == null || nomText == null || prenomText == null ||
                    mailText == null || passwordText == null || loginText.Equals("") ||
                    nomText.Equals("") || prenomText.Equals("") || mailText.Equals("") ||
                    passwordText.Equals(""))
                {
                    WrongCompleted = true;
                    return;
                }
                if(loginText.ToLower().StartsWith("jury"))
                {
                    StartAnonym = true;
                    return;
                }
                if (CreateAccountManager.CheckPassword(PasswordEntry.Text) == false)
                {
                    WrongPassword = true;
                    return;
                }
                if (CreateAccountManager.CheckMail(MailEntry.Text) == false)
                {
                    WrongMail = true;
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
                        Succeed = true;
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
                        Error = true;
                        return;
                    }
                }
            });

            if(Error)
            {
                await DisplayAlert("Erreur", errorMessage, "OK");
            }
            if(Succeed)
            {
                await DisplayAlert("Création de compte", "Votre compte a bien été créé", "OK");
                IndicatorOff();
                await Navigation.PopAsync();
            }
            if(WrongCompleted)
            {
                await DisplayAlert("Erreur", "Tous les champs doivent être renseignés", "OK");
            }
            if(WrongMail)
            {
                await DisplayAlert("Erreur", "Adresse mail invalide", "OK");
            }
            if(WrongPassword)
            {
                await DisplayAlert("Erreur", "Le mot de passe doit contenir au moins 6 caractères dont au moins une lettre en majuscule", "OK");
            }
            if(StartAnonym)
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