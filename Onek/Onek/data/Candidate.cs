using Onek.data;
using System;
using System.ComponentModel;

namespace Onek
{
    /// <summary>
    /// Data class to store candidate information
    /// </summary>
    public class Candidate : INotifyPropertyChanged, ICloneable
    {
        //Properties
        public int Id { get; set; }
        private String statusImage;
        private bool isSigned;
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String FullName { get => FirstName + " " + LastName; }

        /// <summary>
        /// Status image property, redifine get and set methods for the variable statusImage
        /// </summary>
        public String StatusImage
        {
            get
            {
                return statusImage;
            }
            set
            {
                if (statusImage != value)
                {
                    statusImage = value;
                    OnPropertyChanged("StatusImage");
                }
            }
        }

        /// <summary>
        /// IsSigned property, redifine get and set methods for the variable isSigned
        /// </summary>
        public bool IsSigned
        {
            get
            {
                return isSigned;
            }
            set
            {
                if (isSigned != value)
                {
                    isSigned = value;
                    OnPropertyChanged("IsSigned");
                }
            }
        }

        public Evaluation eval { get; set; }

        /// <summary>
        /// Check the status of the notation for a candidate to display the color indicator
        /// </summary>
        public void CheckStatus()
        {
            int numberOfNoted = 0;
            foreach (Criteria criteria in this.eval.Criterias)
            {
                if (!criteria.SelectedLevel.Equals(""))
                {
                    numberOfNoted++;
                }
            }
            if (numberOfNoted == 0)
            {
                this.StatusImage = "red.png";
                return;
            }
            if (numberOfNoted == this.eval.Criterias.Count)
            {
                this.StatusImage = "green.png";
                return;
            }
            this.StatusImage = "yellow.png";
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
        /// Clone the object and his children
        /// </summary>
        /// <returns>Object which represent a Candidate</returns>
        public object Clone()
        {
            Candidate clone = MemberwiseClone() as Candidate;

            clone.eval = eval.Clone() as Evaluation;

            return clone;
        }

    }
}