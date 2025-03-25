using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawfectPRN.Validation
{
    internal class LoginValidator
    {
        public static string Validate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return "Please enter your email.";
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                return "Please enter your password.";
            }
            return null;
        }
    }

}
