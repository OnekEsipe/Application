﻿using Onek.data;
using Onek.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Onek
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotationOverviewPage : ContentPage
    {
        //Properties
        public ObservableCollection<Criteria> Items { get; set; }
        public Criteria SelectedCritere { get; set; }
        private Event CurrentEvent { get; set; }
        private Evaluation Eval { get; set; }
        private Candidate CurrentCandidate { get; set; }
        private User LoggedUser { get; set; }
        private bool GoToPageNote { get; set; }
        private ObservableCollection<Candidate> CandidateList { get; set; }
        private bool SwitchCandidate { get; set; } = false;
        private bool ComeBackSigning { get; set; } = false;

        /// <summary>
        /// NotationOverviewPage constuctor
        /// </summary>
        /// <param name="e">Event, the selected event</param>
        /// <param name="candidates">List<Candidate>Candidates for the event</Candidate></param>
        /// <param name="candidate">Candidate, the candidate that the user evaluates</param>
        /// <param name="loggedUser">User, the logged user</param>
        public NotationOverviewPage(Event e, ObservableCollection<Candidate> candidates, Candidate candidate, User loggedUser)
        {
            InitializeComponent();

            Title = e.Name;

            StatusImage.Source = candidate.StatusImage;

            CurrentCandidate = candidate;
            CurrentCandidate.CheckStatus();
            ChangeStatusImage();
            CandidateList = candidates;
            LoggedUser = loggedUser;
            CurrentEvent = e;

            Evaluation evaluation = candidate.eval;
            evaluation.Criterias = new ObservableCollection<Criteria>(evaluation.Criterias.OrderBy(x => x.Category).ThenBy(x => x.Id));

            Eval = evaluation;

            foreach (Criteria c in Eval.Criterias)
            {
                c.IsModified = false;
            }
            Eval.IsModified = false;

            CandidateNameLabel.Text = CurrentCandidate.FullName;
            LeftButton.Text = "<";
            RightButton.Text = ">";

            if(Device.RuntimePlatform == Device.iOS)
            {
                LeftButton.CornerRadius = 20;
                RightButton.CornerRadius = 20;
            }

            if (!CurrentEvent.SigningNeeded)
            {
                ButtonSigner.IsVisible = false;
            }
            if (Eval.Criterias.All(c => !c.SelectedLevel.Equals("")))
            {
                ButtonSigner.IsEnabled = true;
            }
            else
            {
                ButtonSigner.IsEnabled = false;
            }

            int index = CandidateList.IndexOf(CandidateList.Where(x => x.Id == CurrentCandidate.Id).First());

            SetVisibilityArrow(index);

            Items = new ObservableCollection<Criteria>(Eval.Criterias);
            MyListView.ItemsSource = Items;
            
        }

        /// <summary>
        /// Click on a criteria and displaying notationPage
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">ItemTappedEventArgs</param>
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {
                ((ListView)sender).SelectedItem = null;
                return;
            }
            //Disable touch event
            MyListView.IsEnabled = false;

            if (Eval.IsSigned)
            {
                await DisplayAlert("Erreur", "Vous avez déjà signé et validé cette évaluation", "OK");
                ((ListView)sender).SelectedItem = null;
                MyListView.IsEnabled = true;
                return;
            }

            SelectedCritere = e.Item as Criteria;
            if (SelectedCritere == null)
            {
                ((ListView)sender).SelectedItem = null;
                MyListView.IsEnabled = true;
                return;
            }

            GoToPageNote = true;
            await Navigation.PushAsync(new NotationPage(CurrentCandidate, Eval.Criterias, SelectedCritere));
            MyListView.IsEnabled = true;
            ((ListView)sender).SelectedItem = null;
        }

        /// <summary>
        /// Select a grade for a criteria
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        async void OnLevelButtonClicked(object sender, EventArgs e)
        {
            MyListView.IsEnabled = false;

            if (Eval.IsSigned)
            {
                await DisplayAlert("Erreur", "Vous avez déjà signé et validé cette évaluation", "OK");
                MyListView.IsEnabled = true;
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
            String level = await DisplayActionSheet("Selectionnez la note", "Retour", null, buttonsLevel.ToArray());
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
                if (selectedDescriptor != null && !selectedDescriptor.Equals(criteria.SelectedDescriptor))
                {
                    criteria.SelectedDescriptor = selectedDescriptor;
                    Eval.IsModified = true;
                    GoToPageNote = false;
                }
            }
            MyListView.ItemsSource = Items;

            //Check if button signer must be enabled or not
            if (Eval.Criterias.All(c => !c.SelectedLevel.Equals("")))
            {
                ButtonSigner.IsEnabled = true;
            }
            else
            {
                ButtonSigner.IsEnabled = false;
            }

            if (SelectedCritere == null)
            {

                MyListView.IsEnabled = true;
                return;
            }

            //GoToPageNote = true;
            MyListView.IsEnabled = true;
            
        }

        /// <summary>
        /// Executed when window is displayed
        /// </summary>
        protected override void OnAppearing()
        {
            //Add the footer to the list view
            AddFooter();

            base.OnAppearing();
            Title = CurrentEvent.Name;

            MyListView.ItemsSource = Items;

            CurrentCandidate.CheckStatus();
            ChangeStatusImage();

            if(Eval.IsModified)
            {
                GoToPageNote = false;
            }

            if (!CurrentEvent.SigningNeeded)
            {
                ButtonSigner.IsVisible = false;
            }
            if (Eval.Criterias.All(c => !c.SelectedLevel.Equals("")))
            {
                ButtonSigner.IsEnabled = true;
            }
            else
            {
                ButtonSigner.IsEnabled = false;
            }
            
            GoToPageNote = false;

            if (ComeBackSigning && Eval.IsSigned)
            {
                SaveEvaluation(false);
                ComeBackSigning = false;
            }
                        
        }

        /// <summary>
        /// Executed when the page disappear
        /// </summary>
        protected override async void OnDisappearing()
        {
            if (GoToPageNote)
            {
                MyListView.Footer = null; 
                return;
            }
            if (CurrentEvent.End < DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement a été fermé le " + CurrentEvent.End, "OK");
                base.OnDisappearing();
            }
            if (Eval.IsModified)
            {
                bool answer = await DisplayAlert("Retour", "Voulez vous enregistrer avant de quitter ?", "Oui", "Non");
                if (answer)
                {
                    SaveEvaluation(false);
                }
                base.OnDisappearing();
                return;
            }

            foreach (Criteria c in Eval.Criterias)
            {
                if (c.IsModified)
                {
                    bool answer = await DisplayAlert("Retour", "Voulez vous enregistrer avant de quitter ?", "Oui", "Non");
                    if (answer)
                    {
                        SaveEvaluation(false);
                    }
                    base.OnDisappearing();
                    return;
                }
            }

            base.OnDisappearing();

        }

        /// <summary>
        /// Show pop-up to ask user if he wants to save his changes before switching to an other candidate
        /// </summary>
        /// <returns></returns>
        async Task ConfirmSaveBeforeSwitchAsync()
        {
            if (CurrentEvent.End < DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement a été fermé le " + CurrentEvent.End, "OK");
                base.OnDisappearing();
            }

            if (Eval.IsModified)
            {
                bool answer = await DisplayAlert("Retour", "Voulez vous enregistrer avant de quitter ?", "Oui", "Non");
                if (answer)
                {
                    SaveEvaluation(false);
                }
                base.OnDisappearing();
                return;
            }

            foreach (Criteria c in Eval.Criterias)
            {
                if (c.IsModified)
                {
                    bool answer = false;
                    if (SwitchCandidate)
                        answer = await DisplayAlert("Changement de candidat", "Voulez vous enregistrer avant de passer au candidat suivant ?", "Oui", "Non");
                    else
                        answer = await DisplayAlert("Retour", "Voulez vous enregistrer avant de quitter ?", "Oui", "Non");
                    if (answer)
                    {
                        SaveEvaluation(false);
                    }
                    SwitchCandidate = false;
                    return;
                }
            }
        }

        /// <summary>
        /// On save button clicked, save evaluation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnButtonEnregistrerClicked(object sender, EventArgs e)
        {
            // Save and quit
            //send json to server

            GoToPageNote = true;
            //comeBackFromSigning = false;

            if (CurrentEvent.End < DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement a été fermé le " + CurrentEvent.End, "OK");
                await Navigation.PopAsync();
                return;
            }
            if (!Eval.IsSigned)
            {
                SaveEvaluation(false);
            }
            else
            {
                await DisplayAlert("Erreur", "Vous avez déjà signé et validé cette évaluation", "OK");
                return;
            }
        }

        /// <summary>
        /// On click on sign button, sign and save evaluation
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        async void OnButtonSignerClicked(object sender, EventArgs e)
        {
            GoToPageNote = true;

            if (Eval.IsSigned)
            {
                await DisplayAlert("Erreur", "Vous avez déjà signé et validé cette évaluation", "OK");
                return;
            }
            if (CurrentEvent.End < DateTime.Now)
            {
                await DisplayAlert("Attention", "Cet évènement a été fermé le " + CurrentEvent.End, "OK");
                await Navigation.PopAsync();
                return;
            }
            SaveEvaluation(true);
        }

        /// <summary>
        /// Save the evaluation and add signature if needed and if asked
        /// </summary>
        /// <param name="signature">Boolean, true if the user has clicked on sign button</param>
        async void SaveEvaluation(Boolean signature)
        {
            if (signature && Eval.Criterias.All(c => !c.SelectedLevel.Equals("")) && !Eval.IsSigned)
            {
                ComeBackSigning = true;
                await Navigation.PushAsync(new SigningPage(Eval, CurrentCandidate));
            }
            Eval.LastUpdatedDate = DateTime.Now;
            String jsonEval = JsonParser.GenerateJsonEval(Eval);

            JsonParser.WriteJsonInInternalMemory(jsonEval, CurrentCandidate.Id, LoggedUser.Id, CurrentEvent.Id);

            foreach (Criteria c in Eval.Criterias)
            {
                c.IsModified = false;
            }
            Eval.IsModified = false;

            int index = CandidateList.IndexOf(CandidateList.Where(x => x.Id == CurrentCandidate.Id).First());
            CandidateList[index] = CurrentCandidate;

            CurrentCandidate.CheckStatus();
            ChangeStatusImage();
            if (Eval.Criterias.All(c => !c.SelectedLevel.Equals("")))
            {
                ButtonSigner.IsEnabled = true;
            }
            else
            {
                ButtonSigner.IsEnabled = false;
            }

            await Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() => { ButtonEnregister.IsEnabled = false; });
                EvaluationSender.AddEvaluationInQueue(jsonEval);
                EvaluationSender.SendJsonEvalToServer();
                Device.BeginInvokeOnMainThread(() => { ButtonEnregister.IsEnabled = true; });
            });
        }

        /// <summary>
        /// Check the status of notation and set a color for each status (no grade, in progress, graded)
        /// </summary>
        private void ChangeStatusImage()
        {           
            StatusImage.Source = CurrentCandidate.StatusImage;
        }

        /// <summary>
        /// Click on criteria comment and add or modify the comment for a criteria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnCritereCommentaireClicked(object sender, EventArgs e)
        {
            if (Eval.IsSigned)
            {
                await DisplayAlert("Erreur", "Vous avez déjà signé et validé cette évaluation", "OK");
                return;
            }

            GoToPageNote = true;

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

        /// <summary>
        /// On click on left arrow to switch between candidates
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        /// <returns></returns>
        async Task OnLeftButtonClickedAsync(object sender, EventArgs e)
        {
            SwitchCandidate = true;
            await ConfirmSaveBeforeSwitchAsync();
            int index = CandidateList.IndexOf(CandidateList.Where(x => x.Id == CurrentCandidate.Id).First());
            Candidate leftCandidate = CandidateList[index - 1].Clone() as Candidate;

            ChangeCandidate(leftCandidate, index - 1);
        }

        /// <summary>
        /// On click on right arrow to switch between candidates
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        /// <returns></returns>
        async Task OnRightButtonClickedAsync(object sender, EventArgs e)
        {
            SwitchCandidate = true;
            await ConfirmSaveBeforeSwitchAsync();
            int index = CandidateList.IndexOf(CandidateList.Where(x => x.Id == CurrentCandidate.Id).First());
            Candidate rightCandidate = CandidateList[index + 1].Clone() as Candidate;

            ChangeCandidate(rightCandidate, index + 1);
        }

        /// <summary>
        /// Switch to an other candidate
        /// </summary>
        /// <param name="newCandidate"></param>
        /// <param name="index"></param>
        void ChangeCandidate(Candidate newCandidate, int index)
        {
            CurrentCandidate = newCandidate;

            Evaluation evaluation = CurrentCandidate.eval;

            evaluation.Criterias = new ObservableCollection<Criteria>(evaluation.Criterias.OrderBy(x => x.Category).ThenBy(x => x.Id));

            Eval = evaluation;
            //refresh footer values
            MyListView.Footer = null;

            AddFooter();
            CurrentCandidate.CheckStatus();
            ChangeStatusImage();

            foreach (Criteria c in Eval.Criterias)
            {
                c.IsModified = false;
            }
            Eval.IsModified = false;

            CandidateNameLabel.Text = CurrentCandidate.FullName;

            Items = new ObservableCollection<Criteria>(Eval.Criterias);
            MyListView.ItemsSource = Items;

            if (!CurrentEvent.SigningNeeded)
            {
                ButtonSigner.IsVisible = false;
            }
            if (Eval.Criterias.All(c => !c.SelectedLevel.Equals("")))
            {
                ButtonSigner.IsEnabled = true;
            }
            else
            {
                ButtonSigner.IsEnabled = false;
            }

            SetVisibilityArrow(index);
        }

        /// <summary>
        /// Set the visibility of arrow to switch between candidate
        /// </summary>
        /// <param name="index"></param>
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

        /// <summary>
        /// Called to change the size of the editor to adapt the size to the text
        /// </summary>
        /// <param name="view">View</param>
        private static void Invalidate(View view)
        {
            if(view == null)
            {
                return;
            }
            var method = typeof(View).GetMethod("InvalidateMeasure", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(view, null);
        }

        /// <summary>
        /// Add a footer to the list view to edit the general comment
        /// </summary>
        private void AddFooter()
        {
            StackLayout footerLayout = new StackLayout();
            //Title
            Label footerLabelTitle = new Label { HorizontalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0,20)};
            footerLabelTitle.Text = "Commentaire général";
            //Message 
            Label footerLabelMsg = new Label();
            String text = "Entrez un commentaire";
            footerLabelMsg.Text = text + " (500 caractères restants) :";
            //Editor
            Editor footerEditor = new Editor();
            if (!Eval.IsSigned)
            {
                //Called when the text of editor change
                footerEditor.TextChanged += (sender, ev) =>
                {
                //Change size of editor to adapt to text
                Invalidate(footerEditor);
                //Display the remaining number of characters
                string input = footerEditor.Text;
                    if (input.Length > 500)
                    {
                        input = input.Substring(0, 500);
                        footerEditor.Text = input;
                    }
                    footerLabelMsg.Text = text + " (" + (500 - input.Length) + " caractères restants) :";
                };
            }
            else
            {
                footerEditor.IsEnabled = false;
            }
            footerEditor.BindingContext = Eval;
            footerEditor.SetBinding(Editor.TextProperty, "Comment");
            Eval.IsModified = false;
            footerLayout.Children.Add(footerLabelTitle);
            footerLayout.Children.Add(footerLabelMsg);
            footerLayout.Children.Add(footerEditor);
            MyListView.Footer = footerLayout;
        }
    }
}

