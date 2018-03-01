using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Onek.data
{
    /// <summary>
    /// Data class to store Criteria information
    /// </summary>
    public class Criteria : INotifyPropertyChanged, ICloneable
    {
        //Variables
        private String comment;
        private String selectedLevel;
        //Properties
        public int Id { get; set; }
        public String Text { get; set; }
        public String Category { get; set; }
        public ObservableCollection<Descriptor> Descriptor { get; set; } = new ObservableCollection<data.Descriptor>();
        public Descriptor SelectedDescriptor { get;  set; }
        public DateTime LastModification { get; set; }
        public bool IsModified { get; set; } = false;

        public String Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                LastModification = DateTime.Now;
                IsModified = true;
                OnPropertyChanged("Comment");
            }
        }

        public String SelectedLevel
        {
            get { return selectedLevel; }
            set
            {
                selectedLevel = value;
                LastModification = DateTime.Now;
                IsModified = true;
                OnPropertyChanged("SelectedLevel");
            }
        }

        //INotifyPropertyChanged interface implementation

        /// <summary>
        /// Event triggered when a property is changed, used to refresh values of observableCollections
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
                handler(this, e);
        }

        protected void OnPropertyChanged(String propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        //IClonable interface implementation

        /// <summary>
        /// Clone the object
        /// </summary>
        /// <param name="propertyName">Object which represent a Criteria</param>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}