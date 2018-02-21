using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace Onek.utils
{
    class ApplicationConstants
    {
        //Directory to save application json files
        public static String pathToPersonnalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static String pathToJsonAccountFile = Path.Combine(pathToPersonnalFolder, "account.json");
        public static String jsonDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static String pathToJsonToSend = Path.Combine(jsonDataDirectory, "ToSend");
        //Server routes
        public static String serverEventURL = "https://" + URL + "/api/app/events/[id_event]/[login_user]/export";
        public static String serverLoginURL = "https://" + URL + "/api/app/login";
        public static String serverEvaluationURL = "https://" + URL + "/api/app/evaluation";
        public static String serverRegisterEventURL = "https://" + URL + "/api/app/events/code";
        public static String serverCreateAccountURL = "https://" + URL + "/api/app/createjury";
        public static String serverResetPasswordURL = "https://" + URL + "/api/app/password/reset";
        public static String serverChangePasswordURL = "https://" + URL + "/api/app/password/modify";
        //Server URL
        private static String url;
        private const String defaultURL = "173.249.25.49/serveur";
        //Directory to save configuration file
        private static String configDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static String URL 
        {
            get { return url; }
            set 
            {
                url = value;
                URLChanged();
                WriteConfigFile();
            }
        }

        /// <summary>
        /// Read the config.json file to load and set the server URL
        /// </summary>
        public static void ReadConfigFile()
        {
            String configFileName = "Config.json";
            String configFile = Path.Combine(configDir, configFileName);
            if (File.Exists(configFile))
            {
                String jsonConfig = File.ReadAllText(configFile);
                Dictionary<String, String> dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonConfig);
                URL = dict["ServerURL"];
            }
            else
            {
                URL = defaultURL;
            }
        }

        /// <summary>
        /// Write the new URL configuration in the config.json file
        /// </summary>
        private static void WriteConfigFile()
        {
            String configFileName = "Config.json";
            String configFile = Path.Combine(configDir, configFileName);
            String jsonConfig = "{\"ServerURL\":\"" + URL + "\"}";
            if (!File.Exists(configFile))
                File.Create(configFile);
            File.WriteAllText(configFile, jsonConfig);
        }

        /// <summary>
        /// Change URL values when the URL of the server change
        /// </summary>
        private static void URLChanged()
        {
            serverEventURL = "https://" + URL + "/api/app/events/[id_event]/[login_user]/export";
            serverEvaluationURL = "https://" + URL + "/api/app/evaluation";
            serverLoginURL = "https://" + URL + "/api/app/login";
            serverRegisterEventURL = "https://" + URL + "/api/app/events/code";
            serverCreateAccountURL = "https://" + URL + "/api/app/createjury";
            serverResetPasswordURL = "https://" + URL + "/api/app/password/reset";
            serverChangePasswordURL = "https://" + URL + "/api/app/password/modify";
            
        }

    }
}
