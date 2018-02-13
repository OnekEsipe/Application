using System;
using System.Collections.Generic;
using System.Text;

namespace Onek.utils
{
    class ApplicationConstants
    {

        public static String pathToLoginFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static String jsonDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static String serverEventURL = "https://173.249.25.49/serveur/api/app/events/[id_event]/admin/export";
    }
}
