using Newtonsoft.Json;
using Onek.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
                return (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
