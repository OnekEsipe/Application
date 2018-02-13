﻿using Onek.data;
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

        public NotationOverviewPage(Event e, Evaluation evaluation)
        {
            InitializeComponent();

            CurrentEvent = e;
            evaluation.Criterias = new ObservableCollection<Criteria>(evaluation.Criterias.OrderBy(x => x.Category).ThenBy(x => x.Text));
            Eval = evaluation;

            Items = new ObservableCollection<Criteria>(Eval.Criterias);
            MyListView.ItemsSource = Items;
            ButtonCommentaireGeneral.Text = Eval.Comment;
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
            foreach(Descriptor d in criteria.Descriptor)
            {
                buttonsLevel.Add(d.Level);
            }
            String level = await DisplayActionSheet("Selectionnez la note", "Retour", "", buttonsLevel.ToArray());
            if (!level.Equals("Retour"))
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

            await Navigation.PushAsync(new NotationPage(SelectedCritere));
 
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MyListView.ItemsSource = Items;
        }

        async void OnButtonEnregistrerClicked(object sender, EventArgs e)
        {
            // Enregistrer et Sortir
            //send json to server
            JsonParser.SendJsonToServer(Eval);
            await Navigation.PopAsync();
        }

        async void OnGeneralCommentaireClicked(object sender, EventArgs e)
        {
            string title = "Commentaire de l'évalution";
            string text = "Ecrire un commentaire :";
            Eval.Comment = await InputDialog.InputBox(this.Navigation, title, text, Eval.Comment);
            MyListView.ItemsSource = Items;
            ButtonCommentaireGeneral.Text = Eval.Comment;
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
