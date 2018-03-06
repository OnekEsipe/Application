using Onek.data;
using Onek.utils;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Onek
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsPage : ContentPage
    {
        //Properties
        private ObservableCollection<Event> Items { get; set; }
        private User LoggedUser { get; set; }
        private List<Event> Events = new List<Event>();

        /// <summary>
        /// Events page constructor, initialize variables and content of view
        /// </summary>
        /// <param name="user"></param>
        public EventsPage(User user)
        {
            InitializeComponent();
            
            LoggedUser = user;

            Events = JsonParser.DeserializeJson(LoggedUser);

            Items = new ObservableCollection<Event>(Events.OrderBy(x => x.Name));

			MyListView.ItemsSource = Items;

            //Disable register by code for an anonymous jury
            if(LoggedUser.Login.ToLower().StartsWith("jury"))
            {
                ButtonCode.IsVisible = false;
            }
            else
            {
                ButtonCode.IsVisible = true;
            }
        }

        /// <summary>
        /// Called to refresh the events listview
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        protected void RefreshEventsList(object sender, EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                Task.Run(() =>
                {
                    List<int> Events_id = new List<int>();
                    Events_id = JsonParser.GetEventsIdToDownload(LoggedUser);
                    LoggedUser.Events_id = Events_id;
                //Download and Deserialize json events
                Events = JsonParser.DeserializeJson(LoggedUser);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Items = new ObservableCollection<Event>(Events.OrderBy(x => x.Name));
                        MyListView.ItemsSource = Items;
                    });
                });
            }
            MyListView.EndRefresh();
        }

        /// <summary>
        /// Executed when an event is clicked, redirect to candidates page which 
        /// display all the candidate for associated to the user for this event
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">ItemTappedEventArgs</param>
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            (sender as ListView).IsEnabled = false;

            Event TappedEvent = e.Item as Event;
            if (TappedEvent.Begin > DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement ouvrira le " + TappedEvent.Begin, "OK");
                (sender as ListView).IsEnabled = true;
                return;
            }

            if (TappedEvent.End < DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement a été fermé le " + TappedEvent.End, "OK");
                (sender as ListView).IsEnabled = true;
                return;
            }

            try
            {
                CandidatesPage candidatesPage = new CandidatesPage(TappedEvent, LoggedUser);
                await Navigation.PushAsync(candidatesPage);
            }
            catch(Exception)
            {
                await DisplayAlert("Erreur", "Cet événement rencontre un problème. Contactez votre organisateur.", "OK");
            }

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
            (sender as ListView).IsEnabled = true;
        }

        /// <summary>
        /// Called when the user fill the filter entry to filter the events, refresh the listview items
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        void OnFilterChanged(object sender, EventArgs e)
        {
            if (FilterEventEntry.Text == null)
            {
                MyListView.ItemsSource = Items;
            }
            else
            {
                MyListView.ItemsSource =  Items.Where(eventItem => eventItem.Name.ToLower().Contains(FilterEventEntry.Text.ToLower()) 
                                                            && (eventItem.Name.ToLower().IndexOf(FilterEventEntry.Text.ToLower()) == 0 
                                                                || eventItem.Name.ToLower()[eventItem.Name.ToLower().IndexOf(FilterEventEntry.Text.ToLower())-1] == ' '));
            }
        }

        /// <summary>
        /// Called when user clicks on disconnect button, redirect to login page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnButtonDeconnexionClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Deconnexion", "Voulez-vous vraiment vous deconnecter ?", "Oui", "Non");
            if (answer)
            {
                await Navigation.PopAsync();
            }
        }

        /// <summary>
        /// Called when user want to register to an event with a code, open a input box to fill the code 
        /// and send a request to the server
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        async void OnButtonCodeClicked(object sender, EventArgs e)
        {
            //Check connection
            if (CrossConnectivity.Current.IsConnected)
            {
                string title = "S'inscrire à un évènement";
                string text = "Entrez un code : ";
                string code = await InputDialog.InputBox(this.Navigation, title, text,"");
                //Check code lenght
                if(code.Length == 0)
                {
                    return;
                }
                if (code.Length != 10)
                {
                    await DisplayAlert(title, "Le code saisit n'est pas conforme", "", "OK");
                    return;
                }
                //Create json to send to server
                String jsonEventCode = "{\"Login\":\"" + LoggedUser.Login + "\", \"Event_code\":\"" + code + "\"}";
                //Send json to server
                try
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ApplicationConstants.serverRegisterEventURL);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    JsonParser.SendToServer(httpWebRequest, jsonEventCode);
                    HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        await DisplayAlert(title, "Inscription à l'évènement confirmée", "", "OK");
                        return;
                    }
                } catch (Exception exception)
                {
                    WebException webException = exception as WebException;
                    HttpWebResponse response = webException.Response as HttpWebResponse;
                    if (response.StatusCode.Equals(HttpStatusCode.Conflict) ||
                        response.StatusCode.Equals(HttpStatusCode.Forbidden))
                    {
                        Stream responseStream = response.GetResponseStream();
                        StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                        String textResponse = streamReader.ReadToEnd();
                        await DisplayAlert(title, textResponse, "", "OK");
                        return;
                    }
                    await DisplayAlert(title, "Erreur lors de l'envoi au serveur, veuillez réessayer", "", "OK");
                }
            }
            else
            {
                await DisplayAlert("Connexion", "Vous devez être connecté à internet pour vous inscrire avec un code", "", "OK");
                return;
            }
        }

        /// <summary>
        /// Called when user want to change his password, open modify password page
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        async void OnButtonChangePasswordClicked(object sender, EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
                await Navigation.PushAsync(new ModifyPasswordPage(LoggedUser));
            else
                await DisplayAlert("Erreur", "Vous devez être en ligne pour modifier votre mot de passe", "OK");
        }
    }
}
