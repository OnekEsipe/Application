using System;
using Xamarin.Forms;

namespace Onek.TouchTracking
{
    /// <summary>
    /// Args sent when touching
    /// </summary>
    public class TouchActionEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor of TouchActionArgs
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="type">type of the action</param>
        /// <param name="location">point on touched screen</param>
        /// <param name="isInContact">true if touching, false if releasing</param>
        public TouchActionEventArgs(long id, TouchActionType type, Point location, bool isInContact)
        {
            Id = id;
            Type = type;
            Location = location;
            IsInContact = isInContact;
        }

        public long Id { private set; get; }

        public TouchActionType Type { private set; get; }

        public Point Location { private set; get; }

        public bool IsInContact { private set; get; }
    }
}