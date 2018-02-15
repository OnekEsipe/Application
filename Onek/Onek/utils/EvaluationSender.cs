using Onek.data;
using Plugin.Connectivity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Onek.utils
{
    public class EvaluationSender
    {
        public static ConcurrentQueue<String> EvaluationsToSend { get; set; } = new ConcurrentQueue<string>();
        private static Boolean isOnline = CrossConnectivity.Current.IsConnected;
        private static HttpWebRequest httpWebRequest = WebRequest.Create(ApplicationConstants.serverEvaluationURL) as HttpWebRequest;

        public static void LoadJsons()
        {
            if (isOnline)
            {
                //Si des evaluations non envoyées sont enregistrées les charger
                if (Directory.Exists(ApplicationConstants.pathToJsonToSend))
                {
                    foreach (String jsonPath in Directory.GetFiles(ApplicationConstants.pathToJsonToSend))
                    {
                        EvaluationsToSend.Enqueue(File.ReadAllText(jsonPath));
                    }
                }
            }
        }

        public static void SendJsonEvalToServer()
        {
            isOnline = CrossConnectivity.Current.IsConnected;
            if (isOnline)
            {
                foreach (String json in EvaluationsToSend)
                {
                    try
                    {
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        StreamWriter writer = new StreamWriter(httpWebRequest.GetRequestStream());
                        writer.Write(json);
                        writer.Flush();
                        writer.Close();
                        HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
                        if (response.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            String delete = "";
                            EvaluationsToSend.TryDequeue(out delete);
                            Evaluation deletedEval = JsonParser.DeserializeJsonEvaluation(delete);
                            if (deletedEval != null)
                            {
                                String toDelete = Path.Combine(ApplicationConstants.pathToJsonToSend,
                                    deletedEval.IdCandidate + "-" + deletedEval.IdJury + "-" +
                                    deletedEval.IdEvent + "-evaluation.json");
                                //if evaluation json is sent delete it from internal memory
                                if (File.Exists(toDelete))
                                {
                                    File.Delete(toDelete);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //SI 400 supprimer json
                        Console.WriteLine("Fichier non envoyé");
                    }
                }
            }
        }

        public static void AddEvaluationInQueue(String jsonEvaluation)
        {            
            EvaluationsToSend.Enqueue(jsonEvaluation);
        }
    }
}
