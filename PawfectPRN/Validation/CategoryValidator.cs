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
        // Validation cho Category
        public static bool ValidateCategoryName(string categoryName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                errorMessage = "Vui lòng nhập tên danh mục!";
                return false;
            }

            if (categoryName.Length > 50)
            {
                errorMessage = "Tên danh mục không được vượt quá 50 ký tự!";
                return false;
            }

            if (!Regex.IsMatch(categoryName, @"^[\p{L}0-9\s]+$"))
            {
                errorMessage = "Tên danh mục chỉ được chứa chữ cái, số và khoảng trắng!";
                return false;
            }

            return true;
        }
    }
}
