using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Onek.TouchTracking
{
    /// <summary>
    /// Class use to deal the touch effect on signature : ovveride in ios and android code
    /// </summary>
    public class TouchEffect : RoutingEffect
    {
        public event TouchActionEventHandler TouchAction;

        public TouchEffect() : base("XamarinDocs.TouchEffect")
        {
        }

        public bool Capture { set; get; }

        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }
}

