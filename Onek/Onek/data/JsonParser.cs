using Newtonsoft.Json;
using Onek.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Onek
{

    class JsonParser
    {
        private String pathToLoginFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public List<Event> DeserializeJson(String loginUser)
        {
            //list json to download in login.json
            List<Login> logins = LoadJson();
            List<int> EventsToDownload = new List<int>();
            foreach (Login login in logins)
            {
                if (login.login.Equals(loginUser))
                {
                    EventsToDownload.AddRange(login.Events_id);
                }
            }
            //download and parse json files
            WebClient client = new WebClient();
            List<String> EventsJson = new List<string>();
            foreach (int id in EventsToDownload)
            {
                //Download events data from server
                //EventsJson.Add(client.DownloadString(Login.SERVER_URL + id + "-*"));
            }
            List<Event> events = new List<Event>();
            foreach(String json in EventsJson)
            {
                events.AddRange(JsonConvert.DeserializeObject<List<Event>>(json));
            }
            return events;
        }


        /// <summary>
        /// Load json containing login information and parse it 
        /// </summary>
        /// <returns>List<Login></Login></returns>
        private List<Login> LoadJson()
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
        public void SendJsonToServer(Evaluation eval)
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
        private void SendToServer(HttpWebRequest httpWebRequest, String json)
        {
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
        }

    }
}
