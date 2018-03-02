using Onek.data;
using Plugin.Connectivity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Onek.utils
{
    /// <summary>
    /// Class to send json evaluation to the server
    /// </summary>
    public static class EvaluationSender
    {
        //Variables
        public static ConcurrentQueue<String> EvaluationsToSend { get; set; } = new ConcurrentQueue<string>();
        private static Boolean isOnline = CrossConnectivity.Current.IsConnected;
        private static HttpWebRequest httpWebRequest = WebRequest.Create(ApplicationConstants.serverEvaluationURL) as HttpWebRequest;

        /// <summary>
        /// Static constructor executed when one time when one variable or one method is called
        /// </summary>
        static EvaluationSender()
        {
            EvaluationsToSend = new ConcurrentQueue<string>();
            isOnline = CrossConnectivity.Current.IsConnected;
            httpWebRequest = WebRequest.Create(ApplicationConstants.serverEvaluationURL) as HttpWebRequest;
        }

        /// <summary>
        /// Load json evaluation saved in internal memory (called when application starts)
        /// </summary>
        public static void LoadJsons()
        {
            if (isOnline)
            {
                if (Directory.Exists(ApplicationConstants.pathToJsonToSend))
                {
                    foreach (String jsonPath in Directory.GetFiles(ApplicationConstants.pathToJsonToSend))
                    {
                        EvaluationsToSend.Enqueue(File.ReadAllText(jsonPath));
                    }
                }
            }
        }

        /// <summary>
        /// Send json evaluation to the server
        /// </summary>
        public static void SendJsonEvalToServer()
        {
            isOnline = CrossConnectivity.Current.IsConnected;
            if (isOnline)
            {
                foreach (String json in EvaluationsToSend)
                {
                    try
                    {
                        httpWebRequest = WebRequest.Create(ApplicationConstants.serverEvaluationURL) as HttpWebRequest;
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        StreamWriter writer = new StreamWriter(httpWebRequest.GetRequestStream());
                        writer.Write(json);
                        writer.Flush();
                        writer.Close();
                        HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
                        if (response.StatusCode.Equals(HttpStatusCode.OK) ||
                            response.StatusCode.Equals(HttpStatusCode.Conflict) ||
                            response.StatusCode.Equals(HttpStatusCode.BadRequest))
                        {
                            String delete = "";
                            EvaluationsToSend.TryDequeue(out delete);
                            Evaluation deletedEval = JsonParser.DeserializeJsonEvaluation(delete);
                            DeleteFile(deletedEval);
                            //if conflict get the latest from server
                            if (response.StatusCode.Equals(HttpStatusCode.Conflict))
                            {
                                DownloadLatestVersion(deletedEval);
                            }
                        }
                    }
                    catch (WebException e)
                    {
                        WebException webException = e as WebException;
                        HttpWebResponse response = e.Response as HttpWebResponse;
                        if(response != null && response.StatusCode.Equals(HttpStatusCode.BadRequest) ||
                            response.StatusCode.Equals(HttpStatusCode.Conflict) || 
                            response.StatusCode.Equals(HttpStatusCode.InternalServerError))
                        {
                            String delete = "";
                            EvaluationsToSend.TryDequeue(out delete);
                            Evaluation deletedEval = JsonParser.DeserializeJsonEvaluation(delete);
                            DeleteFile(deletedEval);
                            if (response.StatusCode.Equals(HttpStatusCode.Conflict))
                            {
                                DownloadLatestVersion(deletedEval);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add Evaluation json in sending queue
        /// </summary>
        /// <param name="jsonEvaluation">String json containing evaluation information</param>
        public static void AddEvaluationInQueue(String jsonEvaluation)
        {            
            EvaluationsToSend.Enqueue(jsonEvaluation);
        }

        /// <summary>
        /// Delete file sended
        /// </summary>
        /// <param name="deletedEval">Evaluation, the evaluation object to be deleted</param>
        private static void DeleteFile(Evaluation deletedEval)
        {
            
            if (deletedEval != null)
            {
                String toDelete = Path.Combine(ApplicationConstants.pathToJsonToSend,
                    deletedEval.IdCandidate + "-" + deletedEval.IdJury + "-" +
                    deletedEval.IdEvent + "-evaluation.json");
                if (File.Exists(toDelete))
                {
                    File.Delete(toDelete);
                }
            }
        }

        /// <summary>
        /// Download the latest version of event if the server return that the version of
        /// evaluation in database is newer.
        /// </summary>
        /// <param name="deletedEval">Evaluation, evaluation object to update</param>
        private static void DownloadLatestVersion(Evaluation deletedEval)
        {
            int idEvent = deletedEval.IdEvent;
            int idJury = deletedEval.IdJury;
            List<User> users = JsonParser.LoadLoginJson();
            User user = null;
            if (users != null)
            {
                foreach (User u in users)
                {
                    if (u.Id == idJury)
                    {
                        user = u;
                        break;
                    }
                }
                if (user != null)
                {
                    String eventURL = ApplicationConstants.serverEventURL
                        .Replace("[id_event]", "" + idEvent).Replace("[login_user]", "" + user.Login);
                    String jsonString = JsonParser.DownloadEventJson(eventURL);
                    String fileName = Path.Combine(ApplicationConstants.jsonDataDirectory, idEvent + "-event.json");
                    File.WriteAllText(fileName, jsonString);
                }
            }
        }
    }
}
