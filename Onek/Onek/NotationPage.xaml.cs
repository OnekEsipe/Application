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

        public NotationPage(Candidate candidate, ObservableCollection<Criteria> criterias, Criteria c)
        {
            InitializeComponent();
            Title = candidate.FullName;
            c.Descriptor = new ObservableCollection<Descriptor>(c.Descriptor.OrderBy(x => x.Level));
            CurrentCriteria = c;
            CriteriaList = criterias;

            buttons.Add("A", AButton);
            buttons.Add("B", BButton);
            buttons.Add("C", CButton);
            buttons.Add("D", DButton);
            buttons.Add("E", EButton);
            buttons.Add("F", FButton);


            Items = new ObservableCollection<Descriptor>(CurrentCriteria.Descriptor.OrderBy(x => x.Level));
            Comment = CurrentCriteria.Comment;
            
            SelectedDescripteur = CurrentCriteria.SelectedDescriptor;
            if (SelectedDescripteur != null)
            {
                DescriptionBox.Text = CurrentCriteria.SelectedDescriptor.Text;

                foreach (Descriptor d in CurrentCriteria.Descriptor)
                {
                    if (d.Level == SelectedDescripteur.Level)
                    {
                        buttons[d.Level].BackgroundColor = Color.DarkBlue;
                        buttons[d.Level].TextColor = Color.White;
                    }
                    else
                    {
                        buttons[d.Level].BackgroundColor = Color.LightBlue;
                        buttons[d.Level].TextColor = Color.Black;
                    }
                }
            }
            else
            {
                DescriptionBox.Text = "";

                foreach (Descriptor d in CurrentCriteria.Descriptor)
                {

                    buttons[d.Level].BackgroundColor = Color.LightBlue;
                    buttons[d.Level].TextColor = Color.Black;
                    
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
                    entry.Value.HeightRequest = 50;
                    entry.Value.WidthRequest = 50;
                }
                else
                {
                    entry.Value.HeightRequest = 70;
                    entry.Value.WidthRequest = 70;
                }
            }

            EditorCommentaireCritere.Text = c.Comment;

            CritereNameLabel.Text = CurrentCriteria.Text;
            LeftButton.Text = "<";
            RightButton.Text = ">";

            SetVisibilityArrow(CriteriaList.IndexOf(CurrentCriteria));
        }
        

        void OnClickedDescriptor(object sender, ItemTappedEventArgs e)
        {
            SelectedDescripteur = CurrentCriteria.Descriptor.Where(w => w.Level.Equals((sender as Button).Text)).First();

            changeColorButtonsDescriptor();
                  
            DescriptionBox.Text = SelectedDescripteur.Text;
            ButtonValider.IsEnabled = true;
        }

        private void changeColorButtonsDescriptor()
        {
            foreach (Descriptor d in CurrentCriteria.Descriptor)
            {
                if (d.Level == SelectedDescripteur.Level)
                {
                    buttons[d.Level].BackgroundColor = Color.DarkBlue;
                    buttons[d.Level].TextColor = Color.White;
                }
                else
                {
                    buttons[d.Level].BackgroundColor = Color.LightBlue;
                    buttons[d.Level].TextColor = Color.Black;
                }
            }
        }
        
        void OnEditorCommentaireChanged(object sender, EventArgs e)
        {
            if (CurrentCriteria.Comment == null)
            {
                CurrentCriteria.Comment = "";
            }
            Editor editor = sender as Editor;
            if(editor != null && editor.Text != null)
            {
                Comment = editor.Text;
            }

            EditorCommentaireCritere.Text = Comment;
            ButtonValider.IsEnabled = true;
        }

        void OnButtonValiderClicked(object sender, EventArgs e)
        {
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

            EditorCommentaireCritere.Text = Comment;
            SelectedDescripteur = CurrentCriteria.SelectedDescriptor;

            CritereNameLabel.Text = CurrentCriteria.Text;
            if (SelectedDescripteur != null)
            {
                DescriptionBox.Text = CurrentCriteria.SelectedDescriptor.Text;

                foreach (Descriptor d in CurrentCriteria.Descriptor)
                {
                    if (d.Level == SelectedDescripteur.Level)
                    {
                        buttons[d.Level].BackgroundColor = Color.DarkBlue;
                        buttons[d.Level].TextColor = Color.White;
                    }
                    else
                    {
                        buttons[d.Level].BackgroundColor = Color.LightBlue;
                        buttons[d.Level].TextColor = Color.Black;
                    }
                }
            }
            else
            {
                DescriptionBox.Text = "";

                foreach (Descriptor d in CurrentCriteria.Descriptor)
                {
                    buttons[d.Level].BackgroundColor = Color.LightBlue;
                    buttons[d.Level].TextColor = Color.Black;
                    
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
