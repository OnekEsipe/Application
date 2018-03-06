using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Onek.utils
{
    /// <summary>
    /// Class which contains static variables and methods used accross the app
    /// </summary>
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
        public static String serverUserEventsId = "https://" + URL + "/api/app/jury/events/id";
        //Server URL
        private static String url;
        //URL bidon 
        private const String defaultURL = "addressIp/serveur";
        //Directory to save configuration file
        private static String configDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        /// <summary>
        /// Static constructor executed one time when one of the variables or methods is called for the first time
        /// </summary>
        static ApplicationConstants()
        {
            if (!Directory.Exists(pathToPersonnalFolder))
                Directory.CreateDirectory(pathToPersonnalFolder);
            if (!Directory.Exists(jsonDataDirectory))
                Directory.CreateDirectory(jsonDataDirectory);
            if (!Directory.Exists(configDir))
                Directory.CreateDirectory(configDir);
        }

        /// <summary>
        /// URL property (redifine get and set)
        /// </summary>
        public static String URL 
        {
            get { return url; }
            set 
            {
                if (value != null && !value.Equals(""))
                {
                    if (value.EndsWith("/"))
                        url = value.Substring(0, value.Length - 1);
                    else
                        url = value;
                    URLChanged();
                    WriteConfigFile();
                }
            }
        }

        /// <summary>
        /// DefaultURL property (redifine get and set)
        /// </summary>
        public static String DefaultURL
        {
            get { return defaultURL; }
            private set { }
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
            serverUserEventsId = "https://" + URL + "/api/app/jury/events/id";
        }

    }
}
