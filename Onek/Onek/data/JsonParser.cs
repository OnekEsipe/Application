using Newtonsoft.Json;
using Onek.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Onek.utils;
using Plugin.Connectivity;

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
            List<Event> events = new List<Event>();
            if (CrossConnectivity.Current.IsConnected)
            {
                //List json to download in login.json (à retirer de cette méthode)
                List<int> EventsToDownload = new List<int>();
                EventsToDownload.AddRange(user.Events_id);

                //Download and parse json files
                List<String> EventsJson = new List<string>();
                foreach (int id in EventsToDownload)
                {
                    String downloadEventURL = ApplicationConstants.serverEventURL.Replace("[id_event]", "" + id).Replace("[login_user]", "" + user.Login);
                    //Download events data from server
                    try
                    {
                        String jsonString = DownloadEventJson(downloadEventURL);
                        EventsJson.Add(jsonString);
                        //Save json into internal memory
                        String fileName = Path.Combine(ApplicationConstants.jsonDataDirectory, id + "-event.json");
                        File.WriteAllText(fileName, jsonString);
                    }
                    catch (Exception e)
                    {
                        //Handle http error
                    }
                }

                //Deserialize json
                foreach (String json in EventsJson)
                {
                    Event eventDeserialized = JsonConvert.DeserializeObject<Event>(json);
                    if (eventDeserialized != null)
                    {
                        events.Add(eventDeserialized);
                    }
                }
            }
            else
            {
                foreach (int eventId in user.Events_id)
                {
                    String file = Path.Combine(ApplicationConstants.jsonDataDirectory,
                            eventId + "-event.json");
                    if (File.Exists(file))
                    {
                        String json = File.ReadAllText(file);
                        Event e = JsonConvert.DeserializeObject<Event>(json);
                        if (e != null)
                        {
                            events.Add(e);
                        }
                    }
                }
            }
            return events;
        }

        /// <summary>
        /// Download event json
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static String DownloadEventJson(String url)
        {
            WebClient client = new WebClient();
            return client.DownloadString(url);
        }


        /// <summary>
        /// Load json containing login information and parse it 
        /// </summary>
        /// <returns>List<Login></Login></returns>
        public static List<User> LoadLoginJson()
        {
            //Simulate Json File in Folder
            /*List<User> loginList = new List<User>();
            loginList.Add(new User() { Id = 1, Login = "a", Password = "a", Events_id = { 1, 2, 28 } });
            loginList.Add(new User() { Id = 2, Login = "test", Password = "test", Events_id = { 1, 2, 28 } });
            loginList.Add(new User() { Id = 6, Login = "ff", Password = "ff", Events_id = { 1, 2, 3 } });
            string text = JsonConvert.SerializeObject(loginList);
                

            string documentsPathW = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePathW = Path.Combine(documentsPathW, "account.json");
            System.IO.File.WriteAllText(filePathW, text);

            string documentsPathR = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePathR = Path.Combine(documentsPathR, "account.json");*/
            if (File.Exists(ApplicationConstants.pathToJsonAccountFile))
            {
                String jsonString = File.ReadAllText(ApplicationConstants.pathToJsonAccountFile);
                List<User> logins = JsonConvert.DeserializeObject<List<User>>(jsonString);
                return logins;
            }
            return null;
        }


        /// <summary>
        /// Send json evaluation to the server
        /// </summary>
        /// <param name="eval"></param>
        public static void SendJsonToServer(String json)
        {
            //Send json to server
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ApplicationConstants.serverEvaluationURL);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            SendToServer(httpWebRequest, json);

            //Check server response
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            while (!response.StatusCode.Equals(HttpStatusCode.OK))
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
            String fileNameForSendingQueue = Path.Combine(ApplicationConstants.pathToJsonToSend, idCandidate
                + "-" + idJury + "-" + idEvent + "-evaluation.json");
            File.WriteAllText(fileName, json);
            //Add file in dir for sender
            if (!Directory.Exists(ApplicationConstants.pathToJsonToSend))
            {
                Directory.CreateDirectory(ApplicationConstants.pathToJsonToSend);
            }
            File.WriteAllText(fileNameForSendingQueue, json);
        }

        /// <summary>
        /// Read json eval from internal memory
        /// </summary>
        /// <param name="idCandidate"></param>
        /// <param name="idJury"></param>
        /// <param name="idEvent"></param>
        /// <returns></returns>
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
        public static void SendToServer(HttpWebRequest httpWebRequest, String json)
        {
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
        }

        /// <summary>
        /// Deserialize json account
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<User> DeserializeJsonAccount(String json)
        {
            return JsonConvert.DeserializeObject<List<User>>(json);
        }

        public static void SaveJsonAccountInMemory(List<User> usersToAdd)
        {
            if (File.Exists(ApplicationConstants.pathToJsonAccountFile))
            {
                String jsonAccount = File.ReadAllText(ApplicationConstants.pathToJsonAccountFile);
                List<User> currentJson = DeserializeJsonAccount(jsonAccount);
                usersToAdd.ForEach(u =>
                {
                    Boolean added = false;
                    for (int i = 0; i < currentJson.Count; i++)
                    {
                        if (currentJson[i].Id == u.Id)
                        {
                            currentJson.Remove(currentJson[i]);
                            currentJson.Insert(i, u);
                            added = true;
                        }
                    }
                    if (!added)
                        currentJson.Add(u);
                });
                File.WriteAllText(ApplicationConstants.pathToJsonAccountFile, JsonConvert.SerializeObject(currentJson));
                jsonAccount = File.ReadAllText(ApplicationConstants.pathToJsonAccountFile);
                return;
            }
            File.WriteAllText(ApplicationConstants.pathToJsonAccountFile, JsonConvert.SerializeObject(usersToAdd));
            String json = File.ReadAllText(ApplicationConstants.pathToJsonAccountFile);
        }

    }
}