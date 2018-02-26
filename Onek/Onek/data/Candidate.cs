using Onek.data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Onek
{
    public class Candidate : INotifyPropertyChanged, ICloneable
    {
        public int Id { get; set; }
        private String statusImage;
        private bool isSigned;
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String FullName { get => FirstName + " " + LastName; }
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

        public object Clone()
        {
            Candidate clone = MemberwiseClone() as Candidate;

            clone.eval = eval.Clone() as Evaluation;

            return clone;
        }

    }
}