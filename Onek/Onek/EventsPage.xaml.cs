using System;
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
        public ObservableCollection<string> Items { get; set; }

        public EventsPage()
        {
            InitializeComponent();

            Items = new ObservableCollection<string>
            {
                "Olympiade 2017",
                "Developpement Durable",
                "Exposés techniques",
                "Présentation des outils",
                "Soutenance d'ingénieurs"
            };
			
			MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await Navigation.PushAsync(new CandidatesPage());

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
