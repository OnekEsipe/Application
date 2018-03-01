using System;
using System.ComponentModel;

namespace Onek.data
{
    /// <summary>
    /// Data class to store descriptor information
    /// </summary>
    public class Descriptor : INotifyPropertyChanged
    {
        public String Text { get; set; }
        public String Level { get; set; }

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
    }


}