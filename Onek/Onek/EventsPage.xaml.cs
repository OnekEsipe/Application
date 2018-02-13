using Onek.data;
using Onek.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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
            Events = JsonParser.DeserializeJson("loginUser");
            Items = new ObservableCollection<Event>(Events.OrderBy(x => x.Name));
			MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            
            CandidatesPage candidatesPage = new CandidatesPage(e.Item as Event);
            //candidatesPage.BindingContext = e;
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
            await Navigation.PopAsync();
        }

        async void OnButtonCodeClicked(object sender, EventArgs e)
        {
            string title = "S'inscrire à un évènement";
            string text = "Entrez un code : ";
            string myinput = await InputDialog.InputBox(this.Navigation, title, text,"");
        }
    }
}
