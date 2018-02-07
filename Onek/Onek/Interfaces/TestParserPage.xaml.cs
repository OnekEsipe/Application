using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Onek.Interfaces
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestParserPage : ContentPage
	{
        private ObservableCollection<Event> EventsCollection { get; set; }

		public TestParserPage ()
		{
			InitializeComponent ();

            List<Event> Events = JsonParser.DeserializeJson("bobLeponge");
            foreach(Event e in Events)
            {
                EventsCollection.Add(e);
            }
            EventsListView.ItemsSource = EventsCollection;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Erreur", "Pas d'action de définie", "OK");
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

    }
}