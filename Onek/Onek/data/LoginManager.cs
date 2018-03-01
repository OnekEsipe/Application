using Newtonsoft.Json;
using Onek.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Onek.data
{
    /// <summary>
    /// Class to store login information and send authentificaion request
    /// </summary>
    class LoginManager
    {
        //Properties
        public String Login { get; set; }
        public String Password { get; set; }

        /// <summary>
        /// Generate a json containing login information that will be sent to the server
        /// </summary>
        /// <returns>String, the json login information</returns>
        public String GenerateLoginJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Send a request to the server to authenticate a user
        /// </summary>
        /// <param name="jsonLogin">String json containing login information</param>
        /// <returns>HttpWebResponse, the response of the server</returns>
        public HttpWebResponse SendAuthenticationRequest(String jsonLogin)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ApplicationConstants.serverLoginURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                streamWriter.Write(jsonLogin);
                streamWriter.Flush();
                streamWriter.Close();
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                return response;
            }
            catch (Exception e)
            {
                WebException webException = e as WebException;
                return webException.Response as HttpWebResponse; 
            }
        }
    }
}
