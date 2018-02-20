using Onek.data;
using Onek.utils;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
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
	public partial class ModifyPasswordPage : ContentPage
	{
        private User currentUser;

		public ModifyPasswordPage (User loggedUser)
		{
			InitializeComponent ();
            currentUser = loggedUser;
		}

        async void OnButtonChangePasswordPressed(object sender, EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                String oldPassword = OldPasswordEntry.Text;
                String newPassord = NewPasswordEntry.Text;
                String confirmedPassword = ConfirmPasswordEntry.Text;
                if (oldPassword == null || newPassord == null || confirmedPassword == null ||
                   oldPassword.Equals("") || newPassord.Equals("") || confirmedPassword.Equals(""))
                {
                    await DisplayAlert("Erreur", "Tous les champs doivent être renseignés", "OK");
                    return;
                }
                if (!newPassord.Equals(confirmedPassword))
                {
                    await DisplayAlert("Erreur", "Le nouveau mot de passe et la confirmation doivent être identique", "OK");
                    return;
                }
                if (!CreateAccountManager.CheckPassword(newPassord))
                {
                    await DisplayAlert("Erreur", "Le nouveau mot de passe doit contenir au moins 6 caractères dont au moins une majuscule", "OK");
                    return;
                }
                //hash password
                SHA1Managed sha1 = new SHA1Managed();

                var oldHash = sha1.ComputeHash(Encoding.UTF8.GetBytes(oldPassword));
                String oldPasswordHashed = String.Join("", oldHash.Select(b => b.ToString("x2")).ToArray());

                var newHash = sha1.ComputeHash(Encoding.UTF8.GetBytes(newPassord));
                String newPasswordHashed = String.Join("", newHash.Select(b => b.ToString("x2")).ToArray());

                //Create json
                String jsonModifyPassword = "{ \"Login\":\"" + currentUser.Login + 
                    "\", \"Old_password\":\"" + oldPasswordHashed + "\", \"New_password\":\"" +
                    newPasswordHashed + "\"}";

                //Send json
                try
                {
                    HttpWebRequest webRequest = WebRequest.Create(ApplicationConstants.serverChangePasswordURL) as HttpWebRequest;
                    webRequest.ContentType = "application/json";
                    webRequest.Method = "POST";
                    JsonParser.SendToServer(webRequest, jsonModifyPassword);
                    HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse;
                    //If response OK, quit
                    if (webResponse.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        await DisplayAlert("Changer de mot de passe", "Votre mot de passe a bien été modifié", "OK");
                        await Navigation.PopAsync();
                        return;
                    }
                }
                catch(Exception exception)
                {
                    WebException webException = exception as WebException;
                    HttpWebResponse response = webException.Response as HttpWebResponse;
                    //If conflict, warn user
                    if (response.StatusCode.Equals(HttpStatusCode.Conflict))
                    {
                        await DisplayAlert("Erreur", "Le changement de mot de passe n'a pas pu être effectué", "OK");
                        return;
                    }
                    else
                    {
                        await DisplayAlert("Erreur", "Erreur lors de l'envoi ou du traitement de la requête", "OK");
                        return;
                    }
                }
            }
            else
            {
                await DisplayAlert("Erreur", "Vous devez être connecter à internet pour modifier votre mot de passe", "OK");
                return;
            }
        }

    }
}