using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Onek.data
{
    /// <summary>
    /// Data class to store account creation information
    /// </summary>
    class CreateAccountManager
    {

        //Properties
        public String Login { get; set; }
        public String Lastname { get; set; }
        public String Firstname { get; set; }
        public String Mail { get; set; }
        public String Password { get; set; }

        /// <summary>
        /// Check if password contains at least 6 characters and at least one uppercase letter
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Boolean true if password check specification and false if not</returns>
        public static Boolean CheckPassword(String password)
        {
            if (password.Length < 6 || new Regex("[A-Z]").Matches(password).Count < 1)
                return false;
            return true;
            
        }

        /// <summary>
        /// Check if mail address is valid
        /// </summary>
        /// <param name="mail"></param>
        /// <returns>Boolean true if mail address is valid or false if not</returns>
        public static Boolean CheckMail(String mail)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(mail);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
