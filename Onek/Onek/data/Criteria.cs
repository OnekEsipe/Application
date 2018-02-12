using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Onek.data
{
    public class Criteria : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public String Text { get; set; }
        public String Category { get; set; }
        private String comment = "Zone de commentaire"; // default comment
        public ObservableCollection<Descriptor> Descriptor { get; set; } = new ObservableCollection<data.Descriptor>();
        public int selectedDescriptorIndex = -1; // -1 pas de note
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

        public int SelectedDescriptorIndex
        {
            get { return selectedDescriptorIndex; }
            set
            {
                selectedDescriptorIndex = value;
                //OnPropertyChanged("SelectedDescriptorIndex");
            }
        }

        public int GetDescriptorIndex(Descriptor descriptor)
        {
            int index = 0;
            foreach(Descriptor d in Descriptor)
            {
                if (d.Equals(descriptor))
                {
                    return index;
                }
                index++;
            }
            return index;
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
    }
}