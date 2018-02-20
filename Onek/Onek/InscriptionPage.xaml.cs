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
            CreateAccountManager accountManager = new CreateAccountManager();
            if(LoginEntry.Text == null || NomEntry.Text == null || PrenomEntry.Text == null || 
                MailEntry.Text == null || PasswordEntry.Text == null || LoginEntry.Text.Equals("") ||
                NomEntry.Text.Equals("") || PrenomEntry.Text.Equals("")|| MailEntry.Text.Equals("") || 
                PasswordEntry.Text.Equals(""))
            {
                await DisplayAlert("Erreur", "Tous les champs doivent être renseignés", "OK");
                return;
            }
            if(CreateAccountManager.CheckPassword(PasswordEntry.Text) == false)
            {
                await DisplayAlert("Erreur", "Le mot de passe doit contenir au moins 6 caractères dont au moins une lettre en majuscule", "OK");
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
                    await DisplayAlert("Création de compte", "Votre compte a bien été créé", "OK");
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
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    String errorMessage = reader.ReadToEnd();
                    await DisplayAlert("Erreur", errorMessage, "OK");
                    return;
                }
            }
        }
	}
}