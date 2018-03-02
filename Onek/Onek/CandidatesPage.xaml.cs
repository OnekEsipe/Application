using Onek.data;
using Onek.utils;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Onek
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CandidatesPage : ContentPage
    {
        //Properties
        public ObservableCollection<Candidate> Items { get; set; }
        private Event CurrentEvent { get; set; }
        private User LoggedUser { get; set; }

        /// <summary>
        /// Candidate page constructor, initialize variables and content of view
        /// </summary>
        /// <param name="e"></param>
        /// <param name="loggedUser"></param>
        public CandidatesPage(Event e, User loggedUser)
        {
            InitializeComponent();
            CurrentEvent = e;
            Title = e.Name;
            LoggedUser = loggedUser;
            Items = new ObservableCollection<Candidate>(CurrentEvent.Jurys.First().Candidates.OrderBy(x => x.FullName));

            foreach (Candidate c in Items)
            {
                c.eval = FindEvaluation(c);
                c.CheckStatus();
                c.IsSigned = c.eval.IsSigned;
            }


            MyListView.ItemsSource = Items;
        }

        /// <summary>
        /// Called when the user clicks on a candidate, open the notation overview page for this candidate
        /// </summary>
        /// <param name="sender">object, the listview item clicked</param>
        /// <param name="e">ItemTappedEventArgs</param>
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            (sender as ListView).IsEnabled = false;

            //Check if evaluation already exists. If not create new Evaluation
            Candidate SelectedCandidate = (e.Item as Candidate).Clone() as Candidate;

            await Navigation.PushAsync(new NotationOverviewPage(CurrentEvent, Items, SelectedCandidate, LoggedUser));

            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            (sender as ListView).IsEnabled = true;
        }

        /// <summary>
        /// Called when the user fill the filter entry to filter the candidates, refresh the listview items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Search the evaluation for a candidate
        /// </summary>
        /// <param name="candidate">Candidate, the candidate for who we want the evaluation</param>
        /// <returns>Evaluation, the evaluation for the candidate</returns>
        private Evaluation FindEvaluation(Candidate candidate)
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

        /// <summary>
        /// Executed when page is displayed
        /// </summary>
        protected override void OnAppearing()
        {            
            foreach (Candidate c in Items)
            {
                c.eval = FindEvaluation(c);
            }
            MyListView.ItemsSource = Items;
            
            base.OnAppearing();
        }

    }
}
