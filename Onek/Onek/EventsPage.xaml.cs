using Onek.data;
using Onek.utils;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Onek
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsPage : ContentPage
    {
        private ObservableCollection<Event> Items { get; set; }
        private User LoggedUser { get; set; }
        private List<Event> Events = new List<Event>();
        

        public EventsPage(User user)
        {
            InitializeComponent();
            
            LoggedUser = user;

            Events = JsonParser.DeserializeJson(LoggedUser);

            Items = new ObservableCollection<Event>(Events.OrderBy(x => x.Name));

			MyListView.ItemsSource = Items;

            if(LoggedUser.Login.ToLower().StartsWith("jury"))
            {
                ButtonCode.IsVisible = false;
            }
            else
            {
                ButtonCode.IsVisible = true;
            }
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            Event TappedEvent = e.Item as Event;
            if (TappedEvent.Begin > DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement ouvrira le " + TappedEvent.Begin, "OK");
                return;
            }

            if (TappedEvent.End < DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement a été fermé le " + TappedEvent.End, "OK");
                return;
            }

            CandidatesPage candidatesPage = new CandidatesPage(TappedEvent, LoggedUser);
            await Navigation.PushAsync(candidatesPage);

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

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

        async void OnButtonDeconnexionClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Deconnexion", "Voulez-vous vraiment vous deconnecter ?", "Oui", "Non");
            if (answer)
            {
                await Navigation.PopAsync();
            }
        }

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
                    if (response.StatusCode.Equals(HttpStatusCode.Forbidden))
                    {
                        Stream responseStream = response.GetResponseStream();
                        StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                        String textResponse = streamReader.ReadToEnd();
                        await DisplayAlert(title, textResponse, "", "OK");
                        return;
                    }
                    else if (response.StatusCode.Equals(HttpStatusCode.Conflict))
                    {
                        await DisplayAlert(title, "Le code évènement rentré est incorrect", "", "OK");
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

        async void OnButtonChangePasswordClicked(object sender, EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
                await Navigation.PushAsync(new ModifyPasswordPage(LoggedUser));
            else
                await DisplayAlert("Erreur", "Vous devez être en ligne pour modifier votre mot de passe", "OK");
        }
    }
}
