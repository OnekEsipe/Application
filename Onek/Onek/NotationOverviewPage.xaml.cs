using Onek.data;
using Onek.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
        private bool comeBackFromSigning { get; set; }
        private ObservableCollection<Candidate> CandidateList { get; set; }

        public NotationOverviewPage(Event e, ObservableCollection<Candidate> candidates, Candidate candidate, User loggedUser)
        {
            InitializeComponent();

            CurrentCandidate = candidate;
            CandidateList = candidates;
            LoggedUser = loggedUser;
            CurrentEvent = e;

            Evaluation evaluation = candidate.eval;
            evaluation.Criterias = new ObservableCollection<Criteria>(evaluation.Criterias.OrderBy(x => x.Category).ThenBy(x => x.Text));

            Eval = evaluation;

            foreach (Criteria c in Eval.Criterias)
            {
                c.isModified = false;
            }
            Eval.isModified = false;

            comeBackFromSigning = false;

            CandidateNameLabel.Text = CurrentCandidate.FullName;
            LeftButton.Text = "<";
            RightButton.Text = ">";

            if (Eval.Criterias.All(c => !c.SelectedLevel.Equals("")))
            {
                ButtonEnregister.Text = "Signer";
            }
            else
            {
                ButtonEnregister.Text = "Enregistrer";
            }

            int index = CandidateList.IndexOf(CandidateList.Where(x => x.Id == CurrentCandidate.Id).First());

            SetVisibilityArrow(index);

            Items = new ObservableCollection<Criteria>(Eval.Criterias);
            MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            if (Eval.isSigned)
            {
                await DisplayAlert("Erreur", "Vous avez déjà signé et validé cette évaluation", "OK");
                return;
            }

            ButtonNoter.IsEnabled = true;
            SelectedCritere = e.Item as Criteria;

            //((ListView)sender).SelectedItem = null;
        }

        async void OnLevelButtonClicked(object sender, EventArgs e)
        {
            if (Eval.isSigned)
            {
                await DisplayAlert("Erreur", "Vous avez déjà signé et validé cette évaluation", "OK");
                return;
            }

            Button buttonClicked = sender as Button;
            Criteria criteria = buttonClicked.BindingContext as Criteria;
            List<String> buttonsLevel = new List<String>();
            criteria.Descriptor = new ObservableCollection<Descriptor>(criteria.Descriptor.OrderBy(x => x.Level));
            foreach (Descriptor d in criteria.Descriptor)
            {
                buttonsLevel.Add(d.Level);
            }
            String level = await DisplayActionSheet("Selectionnez la note", "Retour", "", buttonsLevel.ToArray());
            if ((level != null) && (!level.Equals("Retour")) && (!level.Equals(criteria.SelectedLevel)))
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

            if (Eval.Criterias.All(c => !c.SelectedLevel.Equals("")))
            {
                ButtonEnregister.Text = "Signer";
            }
            else
            {
                ButtonEnregister.Text = "Enregistrer";
            }
        }

        async void OnButtonNoterClicked(object sender, EventArgs e)
        {
            if (Eval.isSigned)
            {
                await DisplayAlert("Erreur", "Vous avez déjà signé et validé cette évaluation", "OK");
                return;
            }
            if (SelectedCritere == null)
                return;

            goToPageNote = true;
            await Navigation.PushAsync(new NotationPage(CurrentCandidate, Eval.Criterias, SelectedCritere));

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MyListView.ItemsSource = Items;

            if (Eval.Criterias.All(c => !c.SelectedLevel.Equals("")))
            {
                ButtonEnregister.Text = "Signer";
            }
            else
            {
                ButtonEnregister.Text = "Enregistrer";
            }

            if (!Eval.isSigned && comeBackFromSigning)
            {
                return;
            }
            if (goToPageNote)
            {
                SaveEvaluation();
            }
            
            comeBackFromSigning = false;
            goToPageNote = false;

        }

        protected override async void OnDisappearing()
        {
            if (goToPageNote)
            {
                return;
            }
            if (CurrentEvent.End < DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement a été fermé le " + CurrentEvent.End, "OK");
                base.OnDisappearing();
            }

            if (Eval.isModified)
            {
                bool answer = await DisplayAlert("Retour", "Voulez vous enregistrer avant de quitter ?", "Oui", "Non");
                if (answer)
                {
                    SaveEvaluation();
                }
                base.OnDisappearing();
                return;
            }

            foreach (Criteria c in Eval.Criterias)
            {
                if (c.isModified)
                {
                    bool answer = await DisplayAlert("Retour", "Voulez vous enregistrer avant de quitter ?", "Oui", "Non");
                    if (answer)
                    {
                        SaveEvaluation();
                    }
                    base.OnDisappearing();
                    return;
                }
            }

            base.OnDisappearing();

        }


        async Task ConfirmSaveBeforeSwitchAsync()
        {
            if (CurrentEvent.End < DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement a été fermé le " + CurrentEvent.End, "OK");
                base.OnDisappearing();
            }

            if (Eval.isModified)
            {
                bool answer = await DisplayAlert("Retour", "Voulez vous enregistrer avant de quitter ?", "Oui", "Non");
                if (answer)
                {
                    SaveEvaluation();
                }
                base.OnDisappearing();
                return;
            }

            foreach (Criteria c in Eval.Criterias)
            {
                if (c.isModified)
                {
                    bool answer = await DisplayAlert("Retour", "Voulez vous enregistrer avant de quitter ?", "Oui", "Non");
                    if (answer)
                    {
                        SaveEvaluation();
                    }
                    return;
                }
            }
        }

        async void OnButtonEnregistrerClicked(object sender, EventArgs e)
        {
            // Save and quit
            //send json to server

            goToPageNote = true;
            comeBackFromSigning = false;

            if (CurrentEvent.End < DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement a été fermé le " + CurrentEvent.End, "OK");
                await Navigation.PopAsync();
                return;
            }
            if (!Eval.isSigned)
            {
                SaveEvaluation();
                if (Eval.isSigned)
                {
                    await Navigation.PopAsync();
                }
            }
            else
            {
                await DisplayAlert("Erreur", "Vous avez déjà signé et validé cette évaluation", "OK");
            }
        }

        async void SaveEvaluation()
        {
            if (Eval.Criterias.All(c => !c.SelectedLevel.Equals("")) && !Eval.isSigned && CurrentEvent.SignatureNeeded)
            {
                await Navigation.PushAsync(new SigningPage(Eval));
                comeBackFromSigning = true;
            }
            else
            {
                Eval.LastUpdatedDate = DateTime.Now;
                String jsonEval = JsonParser.GenerateJsonEval(Eval);
                JsonParser.WriteJsonInInternalMemory(jsonEval, CurrentCandidate.Id, LoggedUser.Id, CurrentEvent.Id);
                EvaluationSender.AddEvaluationInQueue(jsonEval);
                EvaluationSender.SendJsonEvalToServer();


                foreach (Criteria c in Eval.Criterias)
                {
                    c.isModified = false;
                }
                Eval.isModified = false;

                int index = CandidateList.IndexOf(CandidateList.Where(x => x.Id == CurrentCandidate.Id).First());
                CandidateList[index] = CurrentCandidate;

                checkStatus(CurrentCandidate);
                if (Eval.Criterias.All(c => !c.SelectedLevel.Equals("")))
                {
                    ButtonEnregister.Text = "Signer";
                }
                else
                {
                    ButtonEnregister.Text = "Enregistrer";
                }
            }
        }

        private void checkStatus(Candidate candidate)
        {
            int numberOfNoted = 0;
            foreach (Criteria criteria in candidate.eval.Criterias)
            {
                if (!criteria.SelectedLevel.Equals(""))
                {
                    numberOfNoted++;
                }
            }
            if (numberOfNoted == 0)
            {
                candidate.StatusImage = "red.png";
                return;
            }
            if (numberOfNoted == candidate.eval.Criterias.Count)
            {
                candidate.StatusImage = "green.png";
                return;
            }
            candidate.StatusImage = "yellow.png";
        }

        async void OnGeneralCommentaireClicked(object sender, EventArgs e)
        {
            if (Eval.isSigned)
            {
                await DisplayAlert("Erreur", "Vous avez déjà signé et validé cette évaluation", "OK");
                return;
            }

            goToPageNote = true;
            comeBackFromSigning = false;

            string title = "Commentaire de l'évalution";
            string text = "Ecrire un commentaire :";
            if (Eval.Comment == null)
            {
                Eval.Comment = "";
            }
            string answer = await InputDialog.InputBoxWithSize(this.Navigation, title, text, Eval.Comment, 500);
            if (!answer.Equals(Eval.Comment))
            {
                Eval.Comment = answer;
            }
            MyListView.ItemsSource = Items;
        }

        async void OnCritereCommentaireClicked(object sender, EventArgs e)
        {
            if (Eval.isSigned)
            {
                await DisplayAlert("Erreur", "Vous avez déjà signé et validé cette évaluation", "OK");
                return;
            }

            goToPageNote = true;
            comeBackFromSigning = false;

            string title = "Commentaire du critère";
            string text = "Entrez un commentaire : ";
            Criteria critere = (sender as Button).BindingContext as Criteria;
            if (critere.Comment == null)
            {
                critere.Comment = "";
            }
            string answer = await InputDialog.InputBoxWithSize(this.Navigation, title, text, critere.Comment, 500);
            if (!answer.Equals(critere.Comment))
            {
                critere.Comment = answer;
            }
            MyListView.ItemsSource = Items;
        }

        async Task OnLeftButtonClickedAsync(object sender, EventArgs e)
        {
            await ConfirmSaveBeforeSwitchAsync();

            int index = CandidateList.IndexOf(CandidateList.Where(x => x.Id == CurrentCandidate.Id).First());
            Candidate leftCandidate = CandidateList[index - 1].Clone() as Candidate;

            changeCandidate(leftCandidate, index - 1);
        }

        async Task OnRightButtonClickedAsync(object sender, EventArgs e)
        {
            await ConfirmSaveBeforeSwitchAsync();

            int index = CandidateList.IndexOf(CandidateList.Where(x => x.Id == CurrentCandidate.Id).First());
            Candidate rightCandidate = CandidateList[index + 1].Clone() as Candidate;

            changeCandidate(rightCandidate, index + 1);
        }

        void changeCandidate(Candidate newCandidate, int index)
        {
            CurrentCandidate = newCandidate;

            Evaluation evaluation = CurrentCandidate.eval;

            evaluation.Criterias = new ObservableCollection<Criteria>(evaluation.Criterias.OrderBy(x => x.Category).ThenBy(x => x.Text));

            Eval = evaluation;

            foreach (Criteria c in Eval.Criterias)
            {
                c.isModified = false;
            }

            Eval.isModified = false;
            comeBackFromSigning = false;

            CandidateNameLabel.Text = CurrentCandidate.FullName;

            Items = new ObservableCollection<Criteria>(Eval.Criterias);
            MyListView.ItemsSource = Items;

            if (Eval.Criterias.All(c => !c.SelectedLevel.Equals("")))
            {
                ButtonEnregister.Text = "Signer";
            }
            else
            {
                ButtonEnregister.Text = "Enregistrer";
            }

            SetVisibilityArrow(index);
        }

        void SetVisibilityArrow(int index)
        {
            if (index == 0)
            {
                LeftButton.IsEnabled = false;
            }
            else
            {
                LeftButton.IsEnabled = true;
            }
            if (index == CandidateList.Count - 1)
            {
                RightButton.IsEnabled = false;
            }
            else
            {
                RightButton.IsEnabled = true;
            }
        }
    }
}

