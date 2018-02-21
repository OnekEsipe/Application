using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Plugin.Connectivity;
using UIKit;

namespace Onek.iOS
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
            return Reachability.IsHostReachable(url);
            
        }
    }
}