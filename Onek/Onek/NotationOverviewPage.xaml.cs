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
    public partial class NotationOverviewPage : ContentPage
    {
        public ObservableCollection<Critere> Items { get; set; }

        public NotationOverviewPage()
        {
            InitializeComponent();

            Items = new ObservableCollection<Critere>();

            Items.Add(new Critere() {CritereText = "Aisance", Niveaux = new List<string> { "A", "B", "C" }, Commentaire = "-" });
            Items.Add(new Critere() {CritereText = "Diction", Niveaux = new List<string> { "A", "B", "C", "D" }, Commentaire = "Très bon niveau" });
            Items.Add(new Critere() {CritereText = "Contenu", Niveaux = new List<string> { "A", "B" }, Commentaire = "-" });

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
    
    public class Critere
    {
        public String CritereText { get; set; }
        public List<String> Niveaux { get; set; }
        public String Commentaire { get; set; }
    }
}
