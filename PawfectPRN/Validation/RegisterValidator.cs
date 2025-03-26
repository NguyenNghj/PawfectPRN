using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PawfectPRN.Validation
{
    internal class RegisterValidator
    {
        public static string Validate(string fullName, string email, string phoneNumber, string password, string confirmPassword, string gender)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "Please enter your full name.";
            }
            if (string.IsNullOrWhiteSpace(email) || !ValidateEmail(email))
            {
                return "Please enter a valid email.";
            }
            if (string.IsNullOrWhiteSpace(phoneNumber) || !ValidatePhoneNumber(phoneNumber))
            {
                return "Please enter a valid phone number (10-11 digits).";
            }
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                return "Password must be at least 6 characters long.";
            }
            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                return "Please confirm your password.";
            }
            if (password != confirmPassword)
            {
                return "Password confirmation does not match.";
            }
            if (string.IsNullOrWhiteSpace(gender))
            {
                return "Please select your gender.";
            }

            return null; 
        }

        private static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, "^[\\w-\\.+]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
        }

        private static bool ValidatePhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, "^\\d{10,11}$");
        }
    }

}
