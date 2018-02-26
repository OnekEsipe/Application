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

        private bool wrongCompleted { get; set; }
        private bool notIdentic { get; set; }
        private bool wrongPassword { get; set; }
        private bool succeeded { get; set; }
        private bool errorRequest { get; set; }
        private bool errorConflict { get; set; }
        
        public ModifyPasswordPage (User loggedUser)
		{
			InitializeComponent ();
            currentUser = loggedUser;
		}

        async void OnButtonChangePasswordPressed(object sender, EventArgs e)
        {
            IndicatorOn();

            wrongCompleted = false;
            notIdentic = false;
            wrongPassword = false;
            succeeded = false;
            errorRequest = false;
            errorConflict = false;

            String oldPassword = OldPasswordEntry.Text;
            String newPassord = NewPasswordEntry.Text;
            String confirmedPassword = ConfirmPasswordEntry.Text;

            await Task.Run(async () =>
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                   
                    if (oldPassword == null || newPassord == null || confirmedPassword == null ||
                       oldPassword.Equals("") || newPassord.Equals("") || confirmedPassword.Equals(""))
                    {
                        wrongCompleted = true;
                        return;
                    }
                    if (!newPassord.Equals(confirmedPassword))
                    {
                        notIdentic = true;
                        return;
                    }
                    if (!CreateAccountManager.CheckPassword(newPassord))
                    {
                        wrongPassword = true;
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
                            succeeded = true;
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
                            errorConflict = true;
                            return;
                        }
                        else
                        {
                            errorRequest = true;
                            return;
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Erreur", "Vous devez être connecter à internet pour modifier votre mot de passe", "OK");
                    return;
                }
            });

            if(succeeded)
            {
                await DisplayAlert("Changer de mot de passe", "Votre mot de passe a bien été modifié", "OK");
                await Navigation.PopAsync();
                IndicatorOff();
            }
            if(wrongCompleted)
            {
                await DisplayAlert("Erreur", "Tous les champs doivent être renseignés", "OK");
            }
            if(wrongPassword)
            {
                await DisplayAlert("Erreur", "Le nouveau mot de passe doit contenir au moins 6 caractères dont au moins une majuscule", "OK");
            }
            if(errorConflict)
            {
                await DisplayAlert("Erreur", "Le changement de mot de passe n'a pas pu être effectué", "OK");
            }
            if(errorRequest)
            {
                await DisplayAlert("Erreur", "Erreur lors de l'envoi ou du traitement de la requête", "OK");
            }
            if(notIdentic)
            {
                await DisplayAlert("Erreur", "Le nouveau mot de passe et la confirmation doivent être identique", "OK");
            }

            IndicatorOff();
        }

        private void IndicatorOn()
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
            OldPasswordEntry.IsEnabled = false;
            NewPasswordEntry.IsEnabled = false;
            ConfirmPasswordEntry.IsEnabled = false;
            ChangePasswordButton.IsEnabled = false;
        }

        private void IndicatorOff()
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
            OldPasswordEntry.IsEnabled = true;
            NewPasswordEntry.IsEnabled = true;
            ConfirmPasswordEntry.IsEnabled = true;
            ChangePasswordButton.IsEnabled = true;
        }
    }
}