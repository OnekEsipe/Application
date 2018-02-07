using Newtonsoft.Json;
using Onek.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Xamarin.Forms.PlatformConfiguration;

namespace Onek
{

    class JsonParser
    {
        private static String pathToLoginFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private static String jsonDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Deserialize json
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static List<Event> DeserializeJson(String loginUser)
        {
            //List json to download in login.json (à retirer de cette méthode)
            List<int> EventsToDownload = new List<int>();
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
                //Download events data from server
                String jsonString = client.DownloadString(Login.SERVER_URL + id + "-*");
                //EventsJson.Add(jsonString);
                //Save json into internal memory
                String fileName = Path.Combine(jsonDataDirectory, id + "-event.json");
                File.WriteAllText(fileName, jsonString);
            }
            //Test (à supprimer par la suite)
            
            EventsJson.Add("{"+
  "\"Id\":1,"+
  "\"Name\":\"event 1\","+
  "\"Begin\":\"2018-02-07T13:00:00\","+
  "\"End\":\"2018-02-07T15:00:00\","+
  "\"Parameters\":[],"+
  "\"Criterias\":["+
    "{\"Id\":1,"+
    "\"Name\":\"Critère n°1\","+
    "\"Text\":\"Texte critère n°1\","+
    "\"Category\":\"Catégorie de critère n°1\","+
    "\"Descriptor\":["+
      "{\"Text\":\"Très satisfaisant\","+
      "\"Level\":\"A\"},"+
      "{\"Text\":\"Satisfaisant\"," +
      "\"Level\":\"B\"}," +
      "{\"Text\":\"Bof\"," +
      "\"Level\":\"C\"},"+
      "{\"Text\":\"Pas bien\"," +
      "\"Level\":\"D\"}" +
    "]" +
    "},"+
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
  "\"Jurys\":["+
    "{\"Id\":1,"+
      "\"LastName\":\"L'éponge\","+
      "\"FirstName\":\"Bob\","+
      "\"Candidates\":["+
        "{\"Id\":1,"+
        "\"FirstName\":\"Patrick\","+
        "\"LastName\":\"Etoile\"},"+
        "{\"Id\":2,"+
        "\"FirstName\":\"Carlo\","+
        "\"LastName\":\"Poulpe\"}"+
      "]"+
    "}"+
  "],"+
  "\"Evaluations\":[]}");

            //Deserialize json
            List<Event> events = new List<Event>();
            foreach(String json in EventsJson)
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
        private static List<Login> LoadJson()
        {
            StreamReader Reader = new StreamReader(pathToLoginFile);
            String json = Reader.ReadToEnd();
            List<Login> logins = JsonConvert.DeserializeObject<List<Login>>(json);
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
            }
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
