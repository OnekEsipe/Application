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
    class LoginManager
    {
        public String Login { get; set; }
        public String Password { get; set; }

        public String GenerateLoginJson()
        {
            return JsonConvert.SerializeObject(this);
        }

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
