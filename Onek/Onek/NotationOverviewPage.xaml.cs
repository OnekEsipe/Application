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
        public ObservableCollection<Critere> Items { get; set; }
        public string Commentaire { get; set; }
        public Critere SelectedCritere { get; set; }

        public NotationOverviewPage()
        {
            InitializeComponent();

            Commentaire = "Ceci est un commentaire";

            Items = new ObservableCollection<Critere>();

            Items.Add(new Critere() {CritereText = "Aisance", Descripteurs = new List<Descripteur> {new Descripteur() { Niveau = "A", Description = "TB" }
            , new Descripteur() { Niveau = "B", Description = "B" }, new Descripteur() { Niveau = "C", Description = "AB" } }, Commentaire = "-" });

            Items.Add(new Critere() {CritereText = "Diction", Descripteurs = new List<Descripteur> {new Descripteur() { Niveau = "A", Description = "TB" }
            , new Descripteur() { Niveau = "B", Description = "B" }, new Descripteur() { Niveau = "C", Description = "AB" },
                new Descripteur() {Niveau = "D",Description ="PB" } }, Commentaire = "Très bon niveau" });

            Items.Add(new Critere() {CritereText = "Contenu", Descripteurs = new List<Descripteur> {new Descripteur() { Niveau = "A", Description = "TB" }
            , new Descripteur() { Niveau = "B", Description = "B" }}, Commentaire = "-" });

            MyListView.ItemsSource = Items;
            ButtonCommentaireGeneral.Text = Commentaire;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //Changer couleur et Enregistrer Critere cliqué
        }

        async void OnButtonNoterClicked(object sender, EventArgs e)
        {
            //Ecran Noter
        }

        async void OnButtonEnregistrerClicked(object sender, EventArgs e)
        {
            // Enregistrer et Sortir
        }
    }
    
    public class Critere
    {
        public String CritereText { get; set; }
        public List<Descripteur> Descripteurs { get; set; }
        public String Commentaire { get; set; }
    }

    public class Descripteur
    {
        public String Niveau { get; set; }
        public String Description { get; set; }
    }
}
