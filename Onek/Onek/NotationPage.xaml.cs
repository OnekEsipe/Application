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
    public partial class NotationPage : ContentPage
    {
        private ObservableCollection<Criteria> CriteriaList { get; set; }
        public ObservableCollection<Descriptor> Items { get; set; }
        public Descriptor SelectedDescripteur { get; set; }
        public Criteria CurrentCriteria;
        public String Comment { get; set; } = "";
        public Constraint constraintX { get; set; }
        private Dictionary<string, Button> buttons =  new Dictionary<string, Button>();

        /// <summary>
        /// Constuctor for notation page
        /// </summary>
        /// <param name="candidate">Candidate to note</param>
        /// <param name="criterias">List of criteria for the event</param>
        /// <param name="c">Current criteria to note</param>
        public NotationPage(Candidate candidate, ObservableCollection<Criteria> criterias, Criteria c)
        {
            InitializeComponent();
            Title = candidate.FullName;
            c.Descriptor = new ObservableCollection<Descriptor>(c.Descriptor.OrderBy(x => x.Level));
            CurrentCriteria = c;
            CriteriaList = criterias;

            //Add buttons for Levels
            buttons.Add("A", AButton);
            buttons.Add("B", BButton);
            buttons.Add("C", CButton);
            buttons.Add("D", DButton);
            buttons.Add("E", EButton);
            buttons.Add("F", FButton);

            if (Device.RuntimePlatform == Device.iOS)
            {
                AButton.CornerRadius = 22;
                BButton.CornerRadius = 22;
                CButton.CornerRadius = 22;
                DButton.CornerRadius = 22;
                EButton.CornerRadius = 22;
                FButton.CornerRadius = 22;
                LeftButton.CornerRadius = 20;
                RightButton.CornerRadius = 20;
            }

            Items = new ObservableCollection<Descriptor>(CurrentCriteria.Descriptor.OrderBy(x => x.Level));
            Comment = CurrentCriteria.Comment;
            
            //Display correct number of level
            SelectedDescripteur = CurrentCriteria.SelectedDescriptor;
            if (SelectedDescripteur != null)
            {
                DescriptionBox.Text = CurrentCriteria.SelectedDescriptor.Text;

                changeColorButtonsDescriptor();
            }
            else
            {
                DescriptionBox.Text = "";

                foreach (Descriptor d in CurrentCriteria.Descriptor)
                {

                    buttons[d.Level].BackgroundColor = Color.FromHex("#2399e5");
                    
                }
            }

            int count = CurrentCriteria.Descriptor.Count;
            foreach (KeyValuePair<string, Button> entry in buttons)
            {
                if (count > 0)
                {
                    entry.Value.IsVisible = true;
                }
                else
                {
                    entry.Value.IsVisible = false;
                }
                count--;


                if (Device.Idiom == TargetIdiom.Phone)
                {
                    entry.Value.HeightRequest = 45;
                    entry.Value.WidthRequest = 45;
                }
                else
                {
                    entry.Value.HeightRequest = 70;
                    entry.Value.WidthRequest = 70;
                }
            }

            //Display arrows
            EditorCommentaireCritere.Text = c.Comment;

            CritereNameLabel.Text = CurrentCriteria.Text;
            LeftButton.Text = "<";
            RightButton.Text = ">";

            SetVisibilityArrow(CriteriaList.IndexOf(CurrentCriteria));
        }
        
        /// <summary>
        /// Event thrown when a Level Button is touched
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClickedDescriptor(object sender, ItemTappedEventArgs e)
        {
            SelectedDescripteur = CurrentCriteria.Descriptor.Where(w => w.Level.Equals((sender as Button).Text)).First();

            changeColorButtonsDescriptor();
                  
            DescriptionBox.Text = SelectedDescripteur.Text;
            ButtonValider.IsEnabled = true;
        }

        /// <summary>
        /// Change the color of the Level Button
        /// </summary>
        private void changeColorButtonsDescriptor()
        {
            foreach (Descriptor d in CurrentCriteria.Descriptor)
            {
                if (d.Level == SelectedDescripteur.Level)
                {
                    buttons[d.Level].BackgroundColor = Color.FromHex("#070735");
                }
                else
                {
                    buttons[d.Level].BackgroundColor = Color.FromHex("#2399e5");
                }
            }
        }
        
        /// <summary>
        /// Event thrown when the comment for the criteria is changed
        /// Check if the text doesn't exceed the limit ad save it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnEditorCommentaireChanged(object sender, EventArgs e)
        {
            if (CurrentCriteria.Comment == null)
            {
                CurrentCriteria.Comment = "";
            }
            Editor editor = sender as Editor;
            string input = editor.Text;
            if (input.Length > 500)
            {
                input = input.Substring(0, 500);
                editor.Text = input;
            }
            CommentaireLabel.Text = "Commentaire du critère (" + (500 - input.Length) + " caractères restants) :";

            if (editor != null && editor.Text != null)
            {
                Comment = editor.Text;
            }

            EditorCommentaireCritere.Text = Comment;
            ButtonValider.IsEnabled = true;
        }

        /// <summary>
        /// Event thrown when "Enregistrer" button is touched
        /// Save the evaluation and come back to overview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnButtonValiderClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => { ButtonValider.IsEnabled = false; });
            SaveCriteria();
            Device.BeginInvokeOnMainThread(() => { ButtonValider.IsEnabled = true; });
            Navigation.PopAsync();
        }

        /// <summary>
        /// Method saving the level end the comment for a criteria
        /// </summary>
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

        /// <summary>
        /// Event thrown when button "Retour" is touched
        /// Go back to overview page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnButtonRetourClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        /// <summary>
        /// Event thrown when Left Arrow is touched
        /// Switch criteria and save current one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLeftButtonClickedAsync(object sender, EventArgs e)
        {
            SaveCriteria();
            int index = CriteriaList.IndexOf(CurrentCriteria);
            Criteria leftCriteria = CriteriaList[index - 1];

           changeCriteria(leftCriteria, index - 1);
        }

        /// <summary>
        /// Event thrown when Right Arrow is touched
        /// Switch criteria and save current one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRightButtonClickedAsync(object sender, EventArgs e)
        {
            SaveCriteria();
            int index = CriteriaList.IndexOf(CurrentCriteria);
            Criteria leftCriteria = CriteriaList[index + 1];

            changeCriteria(leftCriteria, index + 1);
        }

        /// <summary>
        /// Find and Change the criteria depending on an index
        /// </summary>
        /// <param name="newCriteria">Criteria to display</param>
        /// <param name="index">Index of the criteria in CriteriaList</param>
        void changeCriteria(Criteria newCriteria, int index)
        {
            CurrentCriteria = newCriteria;

            Items = new ObservableCollection<Descriptor>(CurrentCriteria.Descriptor.OrderBy(x => x.Level));
            Comment = CurrentCriteria.Comment;

            EditorCommentaireCritere.Text = Comment;
            SelectedDescripteur = CurrentCriteria.SelectedDescriptor;

            CritereNameLabel.Text = CurrentCriteria.Text;
            if (SelectedDescripteur != null)
            {
                DescriptionBox.Text = CurrentCriteria.SelectedDescriptor.Text;

                changeColorButtonsDescriptor();
            }
            else
            {
                DescriptionBox.Text = "";

                foreach (Descriptor d in CurrentCriteria.Descriptor)
                {
                    buttons[d.Level].BackgroundColor = Color.FromHex("#2399e5");
                    
                }
            }

            int count = CurrentCriteria.Descriptor.Count;
            foreach (KeyValuePair<string, Button> entry in buttons)
            {
                if (count > 0)
                {
                    entry.Value.IsVisible = true;
                }
                else
                {
                    entry.Value.IsVisible = false;
                }
                count--;
            }

            SetVisibilityArrow(index);
        }

        /// <summary>
        /// Display or not the arrow buttons depending of the criteria
        /// </summary>
        /// <param name="index">Index of the criteria</param>
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
