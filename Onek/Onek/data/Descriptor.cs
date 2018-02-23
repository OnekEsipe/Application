using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Onek.data
{
    public class Descriptor : INotifyPropertyChanged
    {
        public String Text { get; set; }
        public String Level { get; set; }

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