using Newtonsoft.Json;
using Onek.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Onek.data;
using System.Text;
using Xamarin.Forms.PlatformConfiguration;

namespace Onek
{

    public class JsonParser
    {
        private static String pathToLoginFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private static String jsonDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static String serverURL = "https://173.249.25.49/serveur/api/app/events/[id_event]/admin/export";

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
            /*List<Login> logins = LoadJson();
            foreach (Login login in logins)
            {
                if (login.login.Equals(loginUser))
                {
                    EventsToDownload.AddRange(login.Events_id);
                }
            }*/
            //Download and parse json files
            WebClient client = new WebClient();
            List<String> EventsJson = new List<string>();
            foreach (int id in EventsToDownload)
            {
                String downloadEventURL = serverURL.Replace("[id_event]", "" + id);
                //Download events data from server
                try
                {
                    String jsonString = client.DownloadString(downloadEventURL);
                    EventsJson.Add(jsonString);
                    //Save json into internal memory
                    String fileName = Path.Combine(jsonDataDirectory, id + "-event.json");
                    File.WriteAllText(fileName, jsonString);
                }
                catch(Exception e)
                {
                    //Handle http error
                }
            }
            //Test (à supprimer par la suite)

            /*EventsJson.Add("{" +
  "\"Id\":1," +
  "\"Name\":\"event 1\"," +
  "\"Begin\":\"2018-02-07T13:00:00\"," +
  "\"End\":\"2018-02-07T15:00:00\"," +
  "\"Parameters\":[]," +
  "\"Criterias\":[" +
    "{\"Id\":1," +
    "\"Name\":\"Critère n°1\"," +
    "\"Text\":\"Texte critère n°1\"," +
    "\"Category\":\"Catégorie de critère n°1\"," +
    "\"Descriptor\":[" +
      "{\"Text\":\"Très satisfaisant\"," +
      "\"Level\":\"A\"}," +
      "{\"Text\":\"Satisfaisant\"," +
      "\"Level\":\"B\"}," +
      "{\"Text\":\"Bof\"," +
      "\"Level\":\"C\"}," +
      "{\"Text\":\"Pas bien\"," +
      "\"Level\":\"D\"}" +
    "]" +
    "}," +
    "{\"Id\":2," +
    "\"Name\":\"Critère n°2\"," +
    "\"Text\":\"Texte critère n°2\"," +
    "\"Category\":\"Catégorie de critère n°2\"," +
    "\"Descriptor\":[" +
      "{\"Text\":\"Très satisfaisant\"," +
      "\"Level\":\"A\"}," +
      "{\"Text\":\"Satisfaisant\"," +
      "\"Level\":\"B\"}," +
      "{\"Text\":\"Bof\"," +
      "\"Level\":\"C\"}," +
      "{\"Text\":\"Pas bien\"," +
      "\"Level\":\"D\"}" +
    "]" +
    "}," +
    "{\"Id\":3," +
    "\"Name\":\"Critère n°3\"," +
    "\"Text\":\"Texte critère n°3\"," +
    "\"Category\":\"Catégorie de critère n°3\"," +
    "\"Descriptor\":[" +
      "{\"Text\":\"Très satisfaisant\"," +
      "\"Level\":\"A\"}," +
      "{\"Text\":\"Satisfaisant\"," +
      "\"Level\":\"B\"}," +
      "{\"Text\":\"Bof\"," +
      "\"Level\":\"C\"}," +
      "{\"Text\":\"Pas bien\"," +
      "\"Level\":\"D\"}" +
    "]" +
    "}" +
  "]," +
  "\"Jurys\":[" +
    "{\"Id\":1," +
      "\"LastName\":\"L'éponge\"," +
      "\"FirstName\":\"Bob\"," +
      "\"Candidates\":[" +
        "{\"Id\":1," +
        "\"FirstName\":\"Patrick\"," +
        "\"LastName\":\"Etoile\"}," +
        "{\"Id\":2," +
        "\"FirstName\":\"Carlo\"," +
        "\"LastName\":\"Poulpe\"}" +
      "]" +
    "}" +
  "]," +
  "\"Evaluations\":[]}");*/

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
        /// Generate JSON and send it to the server
        /// </summary>
        /// <param name="eval"></param>
        public static void SendJsonToServer(Evaluation eval)
        {
            //Generate json
            String json = JsonConvert.SerializeObject(eval);

            //Send json to server
            /*HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("url");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            SendToServer(httpWebRequest, json);

            //Check server response
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            while (!response.StatusCode.GetTypeCode().Equals(HttpStatusCode.OK))
            {
                //If response not ok send it again
                SendToServer(httpWebRequest, json);
            }*/
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