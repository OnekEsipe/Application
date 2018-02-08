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
        private String pathToLoginFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        //Etat de la tablette (en ligne / hors ligne) pour gérer mode de connexion
        public Boolean isOnline { get; set; }

        //Connexion + get all files name to download in http response
        public void UserConnexion()
        {

        }

    }
}