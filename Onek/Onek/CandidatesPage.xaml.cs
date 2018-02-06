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
    public partial class CandidatesPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public CandidatesPage()
        {
            InitializeComponent();

            Items = new ObservableCollection<string>
            {
                "Julien Brossard",
                "Antoine Ganthier",
                "Julien Plancqueel",
                "Ichrak Rezgui",
                "Hamed Tamela",
                "Rodolphe Drocourt",
                "Axel Rolo",
                "Yanis Salah",
                "Florie Monnier",
                "Etienne Duris",
                "Hugo Feuillatre",
                "Medalie Noubigh",
                "Hugo Fourcade",
                "Pauline Lott",
                "Vincent Leman",
                "Etienne Jannot"
            };
			
			MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
