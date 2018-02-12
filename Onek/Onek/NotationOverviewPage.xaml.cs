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
        public Criteria SelectedCritere { get; set; }
        private Event CurrentEvent { get; set; }
        private Evaluation Eval { get; set; }

        public NotationOverviewPage(Event e, Evaluation evaluation)
        {
            InitializeComponent();

            CurrentEvent = e;
            Eval = evaluation;

            Items = new ObservableCollection<Criteria>(Eval.Criterias);
            MyListView.ItemsSource = Items;
            ButtonCommentaireGeneral.Text = Eval.Comment;
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
            Picker LevelPicker = sender as Picker;
            if (LevelPicker.IsFocused)
            {
                Criteria critere = LevelPicker.BindingContext as Criteria;
                critere.SelectedDescriptor = LevelPicker.SelectedItem as Descriptor;
                //LevelPicker.SelectedItem = critere.SelectedDescriptor;
            }
            
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
            //send json to server
            JsonParser.SendJsonToServer(Eval);
            await Navigation.PopAsync();
        }

        async void OnGeneralCommentaireClicked(object sender, EventArgs e)
        {
            string title = "Commentaire de l'évalution";
            string text = Eval.Comment;
            Eval.Comment = await InputDialog.InputBox(this.Navigation, title, text, Eval.Comment);
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
