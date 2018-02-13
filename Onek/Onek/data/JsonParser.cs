using Newtonsoft.Json;
using Onek.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Onek.data;
using System.Text;
using Xamarin.Forms.PlatformConfiguration;
using Onek.utils;

namespace Onek
{

    public class JsonParser
    {

        /// <summary>
        /// Deserialize json
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static List<Event> DeserializeJson(User user)
        {
            //List json to download in login.json (à retirer de cette méthode)
            List<int> EventsToDownload = new List<int>();
            EventsToDownload.AddRange(user.Events_id);
            EventsToDownload.Add(3);

            //Download and parse json files
            WebClient client = new WebClient();
            List<String> EventsJson = new List<string>();
            foreach (int id in EventsToDownload)
            {
                String downloadEventURL = ApplicationConstants.serverEventURL.Replace("[id_event]", "" + id);
                //Download events data from server
                try
                {
                    String jsonString = client.DownloadString(downloadEventURL);
                    EventsJson.Add(jsonString);
                    //Save json into internal memory
                    String fileName = Path.Combine(ApplicationConstants.jsonDataDirectory, id + "-event.json");
                    File.WriteAllText(fileName, jsonString);
                }
                catch(Exception e)
                {
                    //Handle http error
                }
            }

            //Deserialize json
            List<Event> events = new List<Event>();
            foreach (String json in EventsJson)
            {
                Event eventDeserialized = JsonConvert.DeserializeObject<Event>(json);
                if (eventDeserialized != null)
                {
                    events.Add(eventDeserialized);
                }
            }
            return events;
        }


        /// <summary>
        /// Load json containing login information and parse it 
        /// </summary>
        /// <returns>List<Login></Login></returns>
        public static List<User> LoadLoginJson()
        {
            //Simulate Json File in Folder
            List<User> loginList = new List<User>();
            loginList.Add(new User() { Id = 1, Login = "a", Password = "a", Events_id = { 1, 2, 28 } });
            loginList.Add(new User() { Id = 2, Login = "test", Password = "test", Events_id = { 1, 2, 28 } });
            string text = JsonConvert.SerializeObject(loginList);
                

            string documentsPathW = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePathW = Path.Combine(documentsPathW, "account.json");
            System.IO.File.WriteAllText(filePathW, text);

            string documentsPathR = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePathR = Path.Combine(documentsPathR, "account.json");
            String jsonString  = System.IO.File.ReadAllText(filePathR);
            List<User> logins = JsonConvert.DeserializeObject<List<User>>(jsonString);
            return logins;
        }


        /// <summary>
        /// Send json evaluation to the server
        /// </summary>
        /// <param name="eval"></param>
        public static void SendJsonToServer(String json)
        {
            //Send json to server
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("url");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            SendToServer(httpWebRequest, json);

            //Check server response
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            while (!response.StatusCode.GetTypeCode().Equals(HttpStatusCode.OK))
            {
                //If response not ok send it again
                SendToServer(httpWebRequest, json);
                response = (HttpWebResponse)httpWebRequest.GetResponse();
            }
        }

        /// <summary>
        /// Generate Json evaluation
        /// </summary>
        /// <param name="evaluation"></param>
        /// <returns></returns>
        public static String GenerateJsonEval(Evaluation evaluation)
        {
            return JsonConvert.SerializeObject(evaluation);
        }

        /// <summary>
        /// Deserialize json evaluation
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Evaluation DeserializeJsonEvaluation(String json)
        {
            Evaluation evalDeserialized = JsonConvert.DeserializeObject<Evaluation>(json);
            return evalDeserialized;
        }

        /// <summary>
        /// Write json evaluation in internal memory
        /// </summary>
        /// <param name="json"></param>
        /// <param name="idCandidate"></param>
        /// <param name="idJury"></param>
        /// <param name="idEvent"></param>
        public static void WriteJsonInInternalMemory(String json, int idCandidate, int idJury, int idEvent)
        {
            String fileName = Path.Combine(ApplicationConstants.jsonDataDirectory, idCandidate 
                + "-" + idJury + "-" + idEvent + "-evaluation.json");
            File.WriteAllText(fileName, json);
        }

        public static String ReadJsonFromInternalMemeory(int idCandidate, int idJury, int idEvent)
        {
            String fileName = Path.Combine(ApplicationConstants.jsonDataDirectory, idCandidate
                + "-" + idJury + "-" + idEvent + "-evaluation.json");
            return File.ReadAllText(fileName);
        }


        /// <summary>
        /// Send json String to the server
        /// </summary>
        /// <param name="httpWebRequest"></param>
        /// <param name="json"></param>
        private static void SendToServer(HttpWebRequest httpWebRequest, String json)
        {
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
        }

    }
}