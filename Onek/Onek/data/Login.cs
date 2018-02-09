using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Onek.data
{
    public class User
    {
        public const String SERVER_URL = "";

        public int Id { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }
        public List<int> Events_id { get; set; } = new List<int>();
    }
}