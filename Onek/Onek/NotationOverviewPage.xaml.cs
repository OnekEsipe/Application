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
        private Candidate CurrentCandidate { get; set; }
        private User LoggedUser { get; set; }
        private bool goToPageNote { get; set; }

        public NotationOverviewPage(Event e, Evaluation evaluation, Candidate candidate, User loggedUser)
        {
            InitializeComponent();

            CurrentEvent = e;
            evaluation.Criterias = new ObservableCollection<Criteria>(evaluation.Criterias.OrderBy(x => x.Category).ThenBy(x => x.Text));

            CurrentCandidate = candidate;

            Eval = evaluation;
            LoggedUser = loggedUser;

            Items = new ObservableCollection<Criteria>(Eval.Criterias);
            MyListView.ItemsSource = Items;
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            ButtonNoter.IsEnabled = true;
            SelectedCritere = e.Item as Criteria;

            //((ListView)sender).SelectedItem = null;
        }

        async void OnLevelButtonClicked(object sender, EventArgs e)
        {
            Button buttonClicked = sender as Button;
            Criteria criteria = buttonClicked.BindingContext as Criteria;
            List<String> buttonsLevel = new List<String>();
            criteria.Descriptor = new ObservableCollection<Descriptor>(criteria.Descriptor.OrderBy(x => x.Level));
            foreach(Descriptor d in criteria.Descriptor)
            {
                buttonsLevel.Add(d.Level);
            }
            String level = await DisplayActionSheet("Selectionnez la note", "Retour", "", buttonsLevel.ToArray());
            if ((level != null) && (!level.Equals("Retour")))
            {
                criteria.SelectedLevel = level;
                Descriptor selectedDescriptor = null;
                foreach (Descriptor d in criteria.Descriptor)
                {
                    if (d.Level.Equals(level))
                    {
                        selectedDescriptor = d;
                    }
                }
                criteria.SelectedDescriptor = selectedDescriptor;
            }
            MyListView.ItemsSource = Items;
        }

        async void OnButtonNoterClicked(object sender, EventArgs e)
        {
            if (SelectedCritere == null)
                return;

            goToPageNote = true;
            await Navigation.PushAsync(new NotationPage(SelectedCritere));
 
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MyListView.ItemsSource = Items;
            goToPageNote = false;
        }

        /*protected override bool OnBackButtonPressed()
        {
            if(CurrentEvent.End < DateTime.Now)
            {
                DisplayAlert("Attention", "Cet évènement a été fermé le " + CurrentEvent.End, "OK");
                return base.OnBackButtonPressed();
            }

            Task<bool> answer = DisplayAlert("Retour", "Voulez vous enregistrer avant de quitter ?", "Oui", "Non");
            answer.ContinueWith(task =>
                {
                    if (task.Result)
                    {
                        SaveEvaluationAsync();
                    }
                });

            return base.OnBackButtonPressed();
        }*/

        protected override void OnDisappearing()
        {
            if (goToPageNote)
            {
                return;
            }

            if (CurrentEvent.End < DateTime.Now)
            {
                DisplayAlert("Attention", "Cet évènement a été fermé le " + CurrentEvent.End, "OK");
                base.OnDisappearing();
            }

            Task<bool> answer = DisplayAlert("Retour", "Voulez vous enregistrer avant de quitter ?", "Oui", "Non");
            answer.ContinueWith(task =>
            {
                if (task.Result)
                {
                    SaveEvaluationAsync();
                }
            });
            base.OnDisappearing();

        }

        async void OnButtonEnregistrerClicked(object sender, EventArgs e)
        {
            // Save and quit
            //send json to server

            if (CurrentEvent.End < DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement a été fermé le " + CurrentEvent.End, "OK");
                await Navigation.PopAsync();
                return;
            }

            SaveEvaluationAsync();
            await Navigation.PopAsync();
        }

        void SaveEvaluationAsync()
        {
            Eval.LastUpdatedDate = DateTime.Now;
            String jsonEval = JsonParser.GenerateJsonEval(Eval);
            JsonParser.WriteJsonInInternalMemory(jsonEval, CurrentCandidate.Id, LoggedUser.Id, CurrentEvent.Id);
            //JsonParser.SendJsonToServer(jsonEval);
            EvaluationSender.AddEvaluationInQueue(jsonEval);
            EvaluationSender.SendJsonEvalToServer();
        }

        async void OnGeneralCommentaireClicked(object sender, EventArgs e)
        {
            string title = "Commentaire de l'évalution";
            string text = "Ecrire un commentaire :";
            Eval.Comment = await InputDialog.InputBox(this.Navigation, title, text, Eval.Comment);
            MyListView.ItemsSource = Items;
        }

        async void OnCritereCommentaireClicked(object sender, EventArgs e)
        {
            string title = "Commentaire du critère";
            string text = "Entrez un commentaire : ";
            Criteria critere = (sender as Button).BindingContext as Criteria;
            critere.Comment = await InputDialog.InputBox(this.Navigation, title, text, critere.Comment);
            MyListView.ItemsSource = Items;
        }
    } 
}
