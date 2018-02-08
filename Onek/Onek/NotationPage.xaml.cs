using Onek.data;
using Onek.utils;
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
    public partial class NotationPage : ContentPage
    {
        public ObservableCollection<Descriptor> Items { get; set; }
        public Descriptor SelectedDescripteur { get; set; }
        public string Commentaire { get; set; }
        private Criteria CurrentCriteria;

        public NotationPage(Criteria c)
        {
            InitializeComponent();
            CurrentCriteria = c;

            Commentaire = "Ceci est un commentaire de critère";
            Items = new ObservableCollection<Descriptor>(CurrentCriteria.Descriptor);

            MyListView.ItemsSource = Items;
            ButtonCommentaireCritere.Text = Commentaire;
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            SelectedDescripteur = e.Item as Descriptor;
            DescriptionBox.Text = SelectedDescripteur.Text;
            ButtonValider.IsEnabled = true;
        }

        async void OnCritereCommentaireClicked(object sender, EventArgs e)
        {
            string title = "Commentaire du critère";
            string text = "Entrez un commentaire : ";
            Commentaire = await InputDialog.InputBox(this.Navigation, title, text, Commentaire);
            ButtonCommentaireCritere.Text = Commentaire;
        }

        void OnButtonValiderClicked(object sender, EventArgs e)
        {
            // Valider (et retour ?)
        }

        void OnButtonRetourClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}
