using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Onek.data
{
    public class Login
    {
        public const String SERVER_URL = "";

        public String login { get; set; }
        public String password { get; set; }
        public List<int> Events_id { get; set; } = new List<int>();
    }
}