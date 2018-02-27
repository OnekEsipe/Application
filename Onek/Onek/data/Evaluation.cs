using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Onek.data
{
    public class Evaluation : INotifyPropertyChanged, ICloneable
    {
        public int Id { get; set; }
        public int IdJury { get; set; }
        public int IdEvent { get; set; }
        public int IdCandidate { get; set; }
        private String comment = "Zone de commentaire";
        public DateTime LastUpdatedDate { get; set; }
        private ObservableCollection<Criteria> criterias = new ObservableCollection<Criteria>();
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsModified { get; set; } = false;
        public bool IsSigned { get; set; }
        public List<Signature> Signatures { get; set; } = new List<Signature>();

        public Boolean hasEvaluation(int idCandidate)
        {
            return IdCandidate == idCandidate;
        }

        public String Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                IsModified = true;
                OnPropertyChanged("Comment");
            }
        }

        public ObservableCollection<Criteria> Criterias
        {
            get { return criterias; }
            set
            {
                criterias = value;
                IsModified = true;
                OnPropertyChanged("Criterias");
            }
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        protected void OnPropertyChanged(String propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            Evaluation clone = MemberwiseClone() as Evaluation;

            clone.Criterias = new ObservableCollection<Criteria>();

            foreach(Criteria criteria in Criterias)
            {
                clone.Criterias.Add(criteria.Clone() as Criteria);
            }

            return clone;
        }
    }
}