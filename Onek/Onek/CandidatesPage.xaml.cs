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
        public ObservableCollection<string> Items { get; set; }
        private Event CurrentEvent { get; set; }
        private List<Candidate> Candidates;

        public CandidatesPage(Event e)
        {
            InitializeComponent();
            
            Items = new ObservableCollection<string>();
            Candidates = new List<Candidate>();
            CurrentEvent = e;
            Candidates = CurrentEvent.Jurys.First().Candidates;
            foreach (Candidate candidate in Candidates)
            {
                Items.Add(candidate.FirstName + " " + candidate.LastName);
            }
            MyListView.ItemsSource = Items;
        }

        /*protected override void OnAppearing()
        {
            base.OnAppearing();
            
        }*/

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await Navigation.PushAsync(new NotationOverviewPage());

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
                MyListView.ItemsSource = Items.Where(eventItem => eventItem.ToLower().Contains(FilterCandidateEntry.Text.ToLower())
                                                           && (eventItem.ToLower().IndexOf(FilterCandidateEntry.Text.ToLower()) == 0
                                                               || eventItem.ToLower()[eventItem.ToLower().IndexOf(FilterCandidateEntry.Text.ToLower()) - 1] == ' '));
            }
        }
    }
}
