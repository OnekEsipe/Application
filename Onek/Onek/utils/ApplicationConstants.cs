using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Onek.utils
{
    class ApplicationConstants
    {
        public static String pathToPersonnalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static String pathToJsonAccountFile = Path.Combine(pathToPersonnalFolder, "account.json"); 
        public static String jsonDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static String serverEventURL = "https://173.249.25.49/serveur/api/app/events/[id_event]/[login_user]/export";
        public static String serverLoginURL = "https://173.249.25.49/serveur/api/app/login";
        public static String serverEvaluationURL = "https://173.249.25.49/serveur/api/app/evaluation";
        public static String pathToJsonToSend = Path.Combine(jsonDataDirectory, "ToSend");
    }
}
