using PawfectPRN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PawfectPRN.Validation
{
    public static class ProductValidator
    {
        // Validation cho Product
        public static bool ValidateProduct(Product product, out string errorMessage, PawfectPrnContext context = null)
        {
            errorMessage = string.Empty;

            // Kiểm tra Name
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                errorMessage = "Vui lòng nhập tên sản phẩm!";
                return false;
            }
            if (product.Name.Length > 100)
            {
                errorMessage = "Tên sản phẩm không được vượt quá 100 ký tự!";
                return false;
            }
            if (!Regex.IsMatch(product.Name, @"^[\p{L}0-9\s]+$"))
            {
                errorMessage = "Tên sản phẩm chỉ được chứa chữ cái, số và khoảng trắng!";
                return false;
            }

            // Kiểm tra CategoryId
            if (product.CategoryId == 0)
            {
                errorMessage = "Vui lòng chọn danh mục cho sản phẩm!";
                return false;
            }
            if (context != null && !context.Categories.Any(c => c.CategoryId == product.CategoryId))
            {
                errorMessage = "Danh mục không hợp lệ!";
                return false;
            }

            // Kiểm tra Price
            // Nếu Price là 0, có thể người dùng chưa nhập hoặc nhập sai (chữ cái/ký tự đặc biệt)
            if (product.Price == 0)
            {
                errorMessage = "Vui lòng nhập giá sản phẩm hợp lệ (lớn hơn 0)!";
                return false;
            }
            if (product.Price < 0)
            {
                errorMessage = "Giá sản phẩm không được nhỏ hơn 0!";
                return false;
            }
            if (product.Price > 50000000)
            {
                errorMessage = "Giá sản phẩm không được vượt quá 50 triệu!";
                return false;
            }

            // Kiểm tra StockQuantity
            // Nếu StockQuantity là 0, vẫn hợp lệ (có thể sản phẩm hết hàng), nhưng cần kiểm tra nhập sai
            if (product.StockQuantity < 0)
            {
                errorMessage = "Số lượng tồn kho không được nhỏ hơn 0!";
                return false;
            }
            if (product.StockQuantity > 10000)
            {
                errorMessage = "Số lượng tồn kho không được vượt quá 10K!";
                return false;
            }

            // Kiểm tra Description (tùy chọn)
            if (!string.IsNullOrEmpty(product.Description) && product.Description.Length > 500)
            {
                errorMessage = "Mô tả sản phẩm không được vượt quá 500 ký tự!";
                return false;
            }

            return true;
        }
    }
}