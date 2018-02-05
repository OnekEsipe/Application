using System;
using System.Collections.Generic;
using System.Text;

namespace Onek.data
{
    class Login
    {
        public const String PATH_TO_LOGIN_FILE = "";
        public const String SERVER_URL = "";

        public String login { get; set; }
        public String password { get; set; }
        public List<int> Events_id { get; set; }

        //Etat de la tablette (en ligne / hors ligne) pour gérer mode de connexion
    }
}
