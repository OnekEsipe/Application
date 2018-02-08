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
    public partial class NotationOverviewPage : ContentPage
    {
        public ObservableCollection<Criteria> Items { get; set; }
        public string Commentaire { get; set; }
        public Criteria SelectedCritere { get; set; }
        private Event CurrentEvent { get; set; }
        private Evaluation Eval { get; set; }

        public NotationOverviewPage(Event e, Evaluation evaluation)
        {
            InitializeComponent();

            CurrentEvent = e;
            Eval = evaluation;
            Commentaire = "Ceci est un commentaire";

            Items = new ObservableCollection<Criteria>(Eval.Criterias);
            MyListView.ItemsSource = Items;
            ButtonCommentaireGeneral.Text = Commentaire;
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            ButtonNoter.IsEnabled = true;
            SelectedCritere = e.Item as Criteria;

            //((ListView)sender).SelectedItem = null;
        }

        void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Criteria critere = (sender as Picker).BindingContext as Criteria;
            critere.SelectedDescriptorIndex = (sender as Picker).SelectedIndex;
        }

        async void OnButtonNoterClicked(object sender, EventArgs e)
        {
            if (SelectedCritere == null)
                return;

            await Navigation.PushAsync(new NotationPage(SelectedCritere));
        }

        async void OnButtonEnregistrerClicked(object sender, EventArgs e)
        {
            // Enregistrer et Sortir
            CurrentEvent.Evaluations.Add(Eval);
            await Navigation.PopAsync();
        }

        async void OnGeneralCommentaireClicked(object sender, EventArgs e)
        {
            string title = "Commentaire de l'évalution";
            string text = Eval.Comment;
            Eval.Comment = await InputDialog.InputBox(this.Navigation, title, text, Commentaire);
            ButtonCommentaireGeneral.Text = Eval.Comment;
        }

        async void OnCritereCommentaireClicked(object sender, EventArgs e)
        {
            string title = "Commentaire du critère";
            string text = "Entrez un commentaire : ";
            Criteria critere = (sender as Button).BindingContext as Criteria;
            critere.Comment = await InputDialog.InputBox(this.Navigation, title, text, critere.Comment);
            MyListView.ItemsSource = Items;
            (sender as Button).Text = critere.Comment;
        }
    } 
}
