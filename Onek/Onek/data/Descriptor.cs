using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Onek.data
{
    public class Descriptor : INotifyPropertyChanged
    {
        public String Text { get; set; }
        public String Level { get; set; }
        private Color backgroundColor = Color.LightBlue;
        private Color textColor = Color.Black;

        public event PropertyChangedEventHandler PropertyChanged;

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                
                backgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        public Color TextColor
        {
            get { return textColor; }
            set
            {

                textColor = value;
                OnPropertyChanged("TextColor");
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
    }


}