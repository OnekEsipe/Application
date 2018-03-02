using System;
using System.Collections.Generic;

namespace Onek.data
{
    /// <summary>
    /// Data class to store User information
    /// </summary>
    public class User
    {
        //Properties
        public int Id { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }
        public List<int> Events_id { get; set; } = new List<int>();
    }
}