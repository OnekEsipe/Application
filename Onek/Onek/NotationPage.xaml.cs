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
        private ObservableCollection<Criteria> CriteriaList { get; set; }
        public ObservableCollection<Descriptor> Items { get; set; }
        public Descriptor SelectedDescripteur { get; set; }
        public Criteria CurrentCriteria;
        public String Comment { get; set; } = "";
        private Button lastButton { get; set; }

        public NotationPage(ObservableCollection<Criteria> criterias, Criteria c)
        {
            InitializeComponent();
            c.Descriptor = new ObservableCollection<Descriptor>(c.Descriptor.OrderBy(x => x.Level));
            CurrentCriteria = c;
            CriteriaList = criterias;

            Items = new ObservableCollection<Descriptor>(CurrentCriteria.Descriptor.OrderBy(x => x.Level));
            Comment = CurrentCriteria.Comment;

            MyListView.ItemsSource = Items;
            SelectedDescripteur = CurrentCriteria.SelectedDescriptor;
            if (SelectedDescripteur != null)
            {
                MyListView.SelectedItem = SelectedDescripteur;
                DescriptionBox.Text = CurrentCriteria.SelectedDescriptor.Text;
            }
            else
            {
                DescriptionBox.Text = "";
            }

            ButtonCommentaireCritere.Text = c.Comment;

            CritereNameLabel.Text = CurrentCriteria.Text;
            LeftButton.Text = "<";
            RightButton.Text = ">";

            SetVisibilityArrow(CriteriaList.IndexOf(CurrentCriteria));
        }
        

        void OnClickedDescriptor(object sender, ItemTappedEventArgs e)
        {
            if (lastButton != null)
            {
                lastButton.BackgroundColor = Color.LightBlue;
            }
            (sender as Button).BackgroundColor = Color.DarkBlue;
            (sender as Button).TextColor = Color.White;
            lastButton = (sender as Button);

            SelectedDescripteur = CurrentCriteria.Descriptor.Where(w => w.Level.Equals(lastButton.Text)).First();

            DescriptionBox.Text = SelectedDescripteur.Text;
            ButtonValider.IsEnabled = true;
        }

        void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        async void OnCritereCommentaireClicked(object sender, EventArgs e)
        {
            string title = "Commentaire du critère";
            string text = "Ecrire un commentaire :";
            if (CurrentCriteria.Comment == null)
            {
                CurrentCriteria.Comment = "";
            }
            string answer = await InputDialog.InputBoxWithSize(this.Navigation, title, text, CurrentCriteria.Comment,500);
            if(!answer.Equals(Comment))
            {
                Comment = answer;
            }
            ButtonCommentaireCritere.Text = Comment;
        }

        void OnButtonValiderClicked(object sender, EventArgs e)
        {
            // Valider (et retour ?)
            SaveCriteria();
            Navigation.PopAsync();
        }

        void SaveCriteria()
        {
            CurrentCriteria.SelectedDescriptor = SelectedDescripteur;
            if (SelectedDescripteur != null)
            {
                CurrentCriteria.SelectedLevel = SelectedDescripteur.Level;
            }
            else
            {
                CurrentCriteria.SelectedLevel = "";
            }
            CurrentCriteria.Comment = Comment;
        }

        void OnButtonRetourClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        void OnLeftButtonClickedAsync(object sender, EventArgs e)
        {
            SaveCriteria();
            int index = CriteriaList.IndexOf(CurrentCriteria);
            Criteria leftCriteria = CriteriaList[index - 1];

           changeCriteria(leftCriteria, index - 1);
        }

        void OnRightButtonClickedAsync(object sender, EventArgs e)
        {
            SaveCriteria();
            int index = CriteriaList.IndexOf(CurrentCriteria);
            Criteria leftCriteria = CriteriaList[index + 1];

            changeCriteria(leftCriteria, index + 1);
        }

        void changeCriteria(Criteria newCriteria, int index)
        {
            CurrentCriteria = newCriteria;

            Items = new ObservableCollection<Descriptor>(CurrentCriteria.Descriptor.OrderBy(x => x.Level));
            Comment = CurrentCriteria.Comment;

            MyListView.ItemsSource = Items;
            ButtonCommentaireCritere.Text = Comment;
            SelectedDescripteur = CurrentCriteria.SelectedDescriptor;

            CritereNameLabel.Text = CurrentCriteria.Text;
            if (SelectedDescripteur != null)
            {
                MyListView.SelectedItem = SelectedDescripteur;
                DescriptionBox.Text = CurrentCriteria.SelectedDescriptor.Text;
            }
            else
            {
                DescriptionBox.Text = "";
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
            if (index == CriteriaList.Count - 1)
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
