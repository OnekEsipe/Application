using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace Onek.utils
{
    class ConnectionTester
    {
        /// <summary>
        /// Check if server can be contacted 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Boolean checkServerCommunication(String url)
        {
            try
            {
                Ping pinger = new Ping();
                return pinger.Send(url).Status == IPStatus.Success;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
