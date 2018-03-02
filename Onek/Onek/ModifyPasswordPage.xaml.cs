using Onek.data;
using Onek.utils;
using Plugin.Connectivity;
using System;
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
        //Variable
        private User currentUser;
        //Properties
        private bool WrongCompleted { get; set; }
        private bool NotIdentic { get; set; }
        private bool WrongPassword { get; set; }
        private bool Succeeded { get; set; }
        private bool ErrorRequest { get; set; }
        private bool ErrorConflict { get; set; }
        
        /// <summary>
        /// ModifyPasswordPage constructor
        /// </summary>
        /// <param name="loggedUser"></param>
        public ModifyPasswordPage (User loggedUser)
		{
			InitializeComponent ();
            currentUser = loggedUser;
		}

        /// <summary>
        /// Send the old and new password to the server, if the password are good, 
        /// the password for the user is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnButtonChangePasswordPressed(object sender, EventArgs e)
        {
            IndicatorOn();

            WrongCompleted = false;
            NotIdentic = false;
            WrongPassword = false;
            Succeeded = false;
            ErrorRequest = false;
            ErrorConflict = false;

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
                        WrongCompleted = true;
                        return;
                    }
                    if (!newPassord.Equals(confirmedPassword))
                    {
                        NotIdentic = true;
                        return;
                    }
                    if (!CreateAccountManager.CheckPassword(newPassord))
                    {
                        WrongPassword = true;
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
                            Succeeded = true;
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
                            ErrorConflict = true;
                            return;
                        }
                        else
                        {
                            ErrorRequest = true;
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

            if(Succeeded)
            {
                await DisplayAlert("Changer de mot de passe", "Votre mot de passe a bien été modifié", "OK");
                await Navigation.PopAsync();
                IndicatorOff();
            }
            if(WrongCompleted)
            {
                await DisplayAlert("Erreur", "Tous les champs doivent être renseignés", "OK");
            }
            if(WrongPassword)
            {
                await DisplayAlert("Erreur", "Le nouveau mot de passe doit contenir au moins 6 caractères dont au moins une majuscule", "OK");
            }
            if(ErrorConflict)
            {
                await DisplayAlert("Erreur", "Le changement de mot de passe n'a pas pu être effectué", "OK");
            }
            if(ErrorRequest)
            {
                await DisplayAlert("Erreur", "Erreur lors de l'envoi ou du traitement de la requête", "OK");
            }
            if(NotIdentic)
            {
                await DisplayAlert("Erreur", "Le nouveau mot de passe et la confirmation doivent être identique", "OK");
            }

            IndicatorOff();
        }

        /// <summary>
        /// Display the loading indicate while change password is processing
        /// </summary>
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

        /// <summary>
        /// Remove the loading indicator
        /// </summary>
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