using Newtonsoft.Json;
using Onek.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Onek.utils;
using Plugin.Connectivity;
using System.Text;

namespace Onek
{

    public class JsonParser
    {

        /// <summary>
        /// Download json events, deserialize json and write them in internal memory
        /// </summary>
        /// <param name="user">User object containing user information</param>
        /// <returns>Lis<Event>List containing user information</Event></returns>
        public static List<Event> DeserializeJson(User user)
        {
            List<Event> events = new List<Event>();
            if (CrossConnectivity.Current.IsConnected)
            {
                //Download and parse json files
                List<String> EventsJson = new List<string>();
                foreach (int id in user.Events_id)
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
        /// <param name="url">String server url</param>
        /// <returns>String, event json downloaded from server</returns>
        public static String DownloadEventJson(String url)
        {
            WebClient client = new WebClient();
            return client.DownloadString(url);
        }


        /// <summary>
        /// Load json containing login information from internal memory and parse it 
        /// </summary>
        /// <returns>List<User>List containing user information stored in internal memory</User></returns>
        public static List<User> LoadLoginJson()
        {
            if (File.Exists(ApplicationConstants.pathToJsonAccountFile))
            {
                String jsonString = File.ReadAllText(ApplicationConstants.pathToJsonAccountFile);
                List<User> logins = JsonConvert.DeserializeObject<List<User>>(jsonString);
                return logins;
            }
            return null;
        }


        /// <summary>
        /// Send json to the server
        /// </summary>
        /// <param name="json">String, json to send to the server</param>
        public static void SendJsonToServer(String json)
        {
            //Send json to server
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ApplicationConstants.serverEvaluationURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                SendToServer(httpWebRequest, json);

                //Check server response
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            }catch(Exception e)
            {
                WebException webException = e as WebException;
            }
        }

        /// <summary>
        /// Generate Json evaluation
        /// </summary>
        /// <param name="evaluation">Evaluation object to convert in json</param>
        /// <returns>String, json containing evaluation information</returns>
        public static String GenerateJsonEval(Evaluation evaluation)
        {
            return JsonConvert.SerializeObject(evaluation);
        }

        /// <summary>
        /// Deserialize json evaluation
        /// </summary>
        /// <param name="json">String json evaluation to deserialize</param>
        /// <returns>Evaluation object deserialized from json evaluation</returns>
        public static Evaluation DeserializeJsonEvaluation(String json)
        {
            Evaluation evalDeserialized = JsonConvert.DeserializeObject<Evaluation>(json);
            return evalDeserialized;
        }

        /// <summary>
        /// Write json evaluation in internal memory
        /// </summary>
        /// <param name="json">String</param>
        /// <param name="idCandidate">int</param>
        /// <param name="idJury">int</param>
        /// <param name="idEvent">int</param>
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
        /// <param name="idCandidate">int</param>
        /// <param name="idJury">int</param>
        /// <param name="idEvent">int</param>
        /// <returns>String, a json containing evaluation information</returns>
        public static String ReadJsonFromInternalMemeory(int idCandidate, int idJury, int idEvent)
        {
            String fileName = Path.Combine(ApplicationConstants.jsonDataDirectory, idCandidate
                + "-" + idJury + "-" + idEvent + "-evaluation.json");
            return File.ReadAllText(fileName);
        }


        /// <summary>
        /// Send json String to the server
        /// </summary>
        /// <param name="httpWebRequest">HttpWebRequest</param>
        /// <param name="json">String json</param>
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
        /// <param name="json">String, json containing user information</param>
        /// <returns>List<User>List containing user data</User></returns>
        public static List<User> DeserializeJsonAccount(String json)
        {
            return JsonConvert.DeserializeObject<List<User>>(json);
        }

        /// <summary>
        /// Save users in the json account file in the internal memory. (Used for offline login)
        /// </summary>
        /// <param name="usersToAdd">List<User>, list containing User objects</User></param>
        public static void SaveJsonAccountInMemory(List<User> usersToAdd)
        {
            if (!File.Exists(ApplicationConstants.pathToJsonAccountFile))
            {
                File.WriteAllText(ApplicationConstants.pathToJsonAccountFile, JsonConvert.SerializeObject(usersToAdd));
            }
            //If file exists check if user is in the file and update user's information
            else
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
                return;
            }
        }

        /// <summary>
        /// Download a list containing the ids of events associated with a user
        /// </summary>
        /// <param name="user">User, object containing user data</param>
        /// <returns>List<int>, list containing events ids</int></returns>
        public static List<int> GetEventsIdToDownload(User user)
        {
            List<int> events_id = new List<int>();
            try
            {
                HttpWebRequest httpWebRequest = WebRequest.Create(ApplicationConstants.serverUserEventsId) as HttpWebRequest;
                String json = "{ \"Login\":\"" + user.Login + "\", \"Password\":\"" + user.Password + "\" }";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                SendToServer(httpWebRequest, json);

                //Check server response
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                if(response != null && response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    String jsonResponse = reader.ReadToEnd();
                    User u = JsonConvert.DeserializeObject<User>(jsonResponse);
                    events_id = u.Events_id;//dict["Events_id"];
                }
            }catch(Exception e)
            {
                HttpWebResponse response = (e as WebException).Response as HttpWebResponse;
                if(response != null && !response.StatusCode.Equals(HttpStatusCode.Forbidden))
                {
                    return null;
                }
            }
            return events_id;
        }

    }
}