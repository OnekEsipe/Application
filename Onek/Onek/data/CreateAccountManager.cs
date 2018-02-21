using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Onek.data
{
    class CreateAccountManager
    {
        public String Login { get; set; }
        public String Lastname { get; set; }
        public String Firstname { get; set; }
        public String Mail { get; set; }
        public String Password { get; set; }

        /// <summary>
        /// Check if password contains at least 6 characters and at least one uppercase letter
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Boolean CheckPassword(String password)
        {
            if (password.Length < 6 || new Regex("[A-Z]").Matches(password).Count < 1)
                return false;
            return true;
            
        }

        public static Boolean CheckMail(String mail)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(mail);
                return true;
            }
            catch(FormatException e)
            {
                return false;
            }
        }
    }
}
