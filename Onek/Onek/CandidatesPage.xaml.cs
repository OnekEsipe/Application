using Onek.data;
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
    public partial class CandidatesPage : ContentPage
    {
        public ObservableCollection<Candidate> Items { get; set; }
        private Event CurrentEvent { get; set; }

        public CandidatesPage(Event e)
        {
            InitializeComponent();
            CurrentEvent = e;
            Items = new ObservableCollection<Candidate>(CurrentEvent.Jurys.First().Candidates.OrderBy(x => x.FullName));
            MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //Check if evaluation already exists. If not create new Evaluation
            Candidate SelectedCandidate = (e.Item as Candidate);
            int idCandidate = SelectedCandidate.Id;
            Evaluation evaluation = CurrentEvent.GetEvaluationForCandidate(idCandidate);
            if (evaluation == null)
            {
                evaluation = new Evaluation();
                evaluation.IdCandidate = SelectedCandidate.Id;
                evaluation.IdEvent = CurrentEvent.Id;
                evaluation.Criterias = new ObservableCollection<Criteria>();
                foreach(Criteria c in CurrentEvent.Criterias){
                    evaluation.Criterias.Add(c.Clone() as Criteria);
                }
                CurrentEvent.Evaluations.Add(evaluation); 
            }

            /*foreach (Criteria c in evaluation.Criterias)
            {
                if(c.SelectedDescriptor == null)
                    c.SelectedDescriptor = c.Descriptor.First();
            }*/
            await Navigation.PushAsync(new NotationOverviewPage(CurrentEvent, evaluation));

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
            evaluation = null;
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
    }
}
