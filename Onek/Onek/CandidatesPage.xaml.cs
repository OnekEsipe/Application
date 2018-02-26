using Onek.data;
using Onek.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Onek
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CandidatesPage : ContentPage
    {
        public ObservableCollection<Candidate> Items { get; set; }
        private Event CurrentEvent { get; set; }
        private User LoggedUser { get; set; }

        public CandidatesPage(Event e, User loggedUser)
        {
            InitializeComponent();
            CurrentEvent = e;
            LoggedUser = loggedUser;
            Items = new ObservableCollection<Candidate>(CurrentEvent.Jurys.First().Candidates.OrderBy(x => x.FullName));

            foreach (Candidate c in Items)
            {
                c.eval = findEvaluation(c);
                checkStatus(c);
                c.IsSigned = c.eval.isSigned;
            }


            MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //Check if evaluation already exists. If not create new Evaluation
            Candidate SelectedCandidate = (e.Item as Candidate).Clone() as Candidate;

            await Navigation.PushAsync(new NotationOverviewPage(CurrentEvent, Items, SelectedCandidate, LoggedUser));

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        void OnFilterChanged(object sender, EventArgs e)
        {
            if (FilterCandidateEntry.Text == null)
            {
                MyListView.ItemsSource = Items;
            }
            else
            {
                MyListView.ItemsSource = Items.Where(eventItem => eventItem.FullName.ToLower().Contains(FilterCandidateEntry.Text.ToLower())
                                                           && (eventItem.FullName.ToLower().IndexOf(FilterCandidateEntry.Text.ToLower()) == 0
                                                               || eventItem.FullName.ToLower()[eventItem.FullName.ToLower().IndexOf(FilterCandidateEntry.Text.ToLower()) - 1] == ' '));
            }
        }

        private Evaluation findEvaluation(Candidate candidate)
        {
            int idCandidate = candidate.Id;
            Evaluation evaluation = null;

            evaluation = CurrentEvent.GetEvaluationForCandidate(idCandidate);
            evaluation.IdEvent = CurrentEvent.Id;

            if (File.Exists(Path.Combine(ApplicationConstants.jsonDataDirectory, idCandidate
                + "-" + LoggedUser.Id + "-" + CurrentEvent.Id + "-evaluation.json")))
            {
                String jsonString = JsonParser.ReadJsonFromInternalMemeory(idCandidate,
                    LoggedUser.Id, CurrentEvent.Id);
                Evaluation localevaluation = JsonParser.DeserializeJsonEvaluation(jsonString);

                if (localevaluation.LastUpdatedDate != null && evaluation.LastUpdatedDate != null && localevaluation.LastUpdatedDate > evaluation.LastUpdatedDate)
                {
                    evaluation = localevaluation;
                }
            }

            foreach (Criteria cEvent in CurrentEvent.Criterias)
            {
                if (evaluation.Criterias.Count == 0)
                {
                    evaluation.Criterias.Add(cEvent.Clone() as Criteria);
                }
                else
                {
                    foreach (Criteria cEval in evaluation.Criterias)
                    {
                        if (cEval.Id == cEvent.Id)
                        {
                            break;
                        }
                        if (evaluation.Criterias.IndexOf(cEval) == evaluation.Criterias.Count - 1)
                        {
                            evaluation.Criterias.Add(cEvent.Clone() as Criteria);
                        }
                    }
                }
            }
            
            return evaluation;
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

        protected override void OnAppearing()
        {            
            foreach (Candidate c in Items)
            {
                c.eval = findEvaluation(c);
            }
            MyListView.ItemsSource = Items;
            
            base.OnAppearing();
        }

    }
}
