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
        private Criteria CurrentCriteria;
        public String Comment { get; set; }

        public NotationPage(Criteria c)
        {
            InitializeComponent();
            CurrentCriteria = c;

            Items = new ObservableCollection<Descriptor>(CurrentCriteria.Descriptor);
            Comment = CurrentCriteria.Comment;

            MyListView.ItemsSource = Items;
            ButtonCommentaireCritere.Text = c.Comment;

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
            string text = CurrentCriteria.Comment;
            CurrentCriteria.Comment = await InputDialog.InputBox(this.Navigation, title, text, CurrentCriteria.Comment);
            ButtonCommentaireCritere.Text = CurrentCriteria.Comment;
        }

        void OnButtonValiderClicked(object sender, EventArgs e)
        {
            // Valider (et retour ?)
            CurrentCriteria.SelectedDescriptor = SelectedDescripteur;
            CurrentCriteria.SelectedLevel = SelectedDescripteur.Level;
            Navigation.PopAsync();
        }

        void OnButtonRetourClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}
