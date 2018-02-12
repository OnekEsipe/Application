using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Onek.data
{
    public class Criteria : INotifyPropertyChanged, ICloneable
    {
        public int Id { get; set; }
        public String Text { get; set; }
        public String Category { get; set; }
        private String comment = "Zone de commentaire"; // default comment
        public ObservableCollection<Descriptor> Descriptor { get; set; } = new ObservableCollection<data.Descriptor>();
        private String selectedLevel = "";
        public Descriptor SelectedDescriptor { get;  set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public String Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                OnPropertyChanged("Comment");
            }
        }

        public String SelectedLevel
        {
            get { return selectedLevel; }
            set
            {
                selectedLevel = value;
                OnPropertyChanged("SelectedLevel");
            }
        }

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

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}