using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawfectPRN.Validation
{
    namespace PawfectPRN.Validation
    {
        internal class ProfileValidator
        {
            public static bool ValidatePasswordChange(string oldPassword, string newPassword, string confirmNewPassword, string actualHashedPassword, Func<string, string> hashFunction, out string errorMessage)
            {
                errorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
                {
                    errorMessage = "Please fill in all the required fields!";
                    return false;
                }

                if (newPassword != confirmNewPassword)
                {
                    errorMessage = "New password and confirmation do not match!";
                    return false;
                }

                string hashedOldPassword = hashFunction(oldPassword);
                if (hashedOldPassword != actualHashedPassword)
                {
                    errorMessage = "Incorrect old password!";
                    return false;
                }

                return true;
            }

            public static bool ValidateAddress(string address, out string errorMessage)
            {
                errorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(address))
                {
                    errorMessage = "Address cannot be empty!";
                    return false;
                }

                if (!address.Any(char.IsLetter))
                {
                    errorMessage = "Address must contain at least one letter!";
                    return false;
                }

                return true;
            }
        }
    }
}
