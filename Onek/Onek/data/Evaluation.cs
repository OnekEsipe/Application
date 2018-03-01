using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Onek.data
{
    /// <summary>
    /// Data class to store Evaluation information
    /// </summary>
    public class Evaluation : INotifyPropertyChanged, ICloneable
    {
        //Variables
        private String comment;
        //Properties
        public int Id { get; set; }
        public int IdJury { get; set; }
        public int IdEvent { get; set; }
        public int IdCandidate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        private ObservableCollection<Criteria> criterias = new ObservableCollection<Criteria>();
        public bool IsModified { get; set; } = false;
        public bool IsSigned { get; set; }
        public List<Signature> Signatures { get; set; } = new List<Signature>();

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

        /// <summary>
        /// Check if a candidate has an evaluation
        /// </summary>
        /// <param name="idCandidate">id of the candidate</param>
        /// <returns>Boolean true if the candidate has an evaluation and false if not</returns>
        public Boolean HasEvaluation(int idCandidate)
        {
            return IdCandidate == idCandidate;
        }

        //INotifyPropertyChanged interface implementation

        /// <summary>
        /// Event triggered when a property is changed, used to refresh values of observableCollections
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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

        //IClonable interface implementation

        /// <summary>
        /// Clone object and his children
        /// </summary>
        /// <returns>Object which represents an Evakuation</returns>
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