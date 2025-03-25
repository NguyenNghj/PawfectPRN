using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PawfectPRN.Validation
{
    public static class CategoryValidator
    {
        // Validation for Category
        public static bool ValidateCategoryName(string categoryName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                errorMessage = "Please enter a category name!";
                return false;
            }

            if (categoryName.Length > 50)
            {
                errorMessage = "Category name must not exceed 50 characters!";
                return false;
            }

            if (!Regex.IsMatch(categoryName, @"^[\p{L}0-9\s]+$"))
            {
                errorMessage = "Category name can only contain letters, numbers, and spaces!";
                return false;
            }

            return true;
        }
    }
}