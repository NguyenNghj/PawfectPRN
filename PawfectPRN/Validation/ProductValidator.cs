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
        // Validation for Product
        public static bool ValidateProduct(Product product, out string errorMessage, PawfectPrnContext context = null)
        {
            errorMessage = string.Empty;

            // Check Name
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                errorMessage = "Please enter a product name!";
                return false;
            }
            if (product.Name.Length > 100)
            {
                errorMessage = "Product name must not exceed 100 characters!";
                return false;
            }
            if (!Regex.IsMatch(product.Name, @"^[\p{L}0-9\s]+$"))
            {
                errorMessage = "Product name can only contain letters, numbers, and spaces!";
                return false;
            }

            // Check CategoryId
            if (product.CategoryId == 0)
            {
                errorMessage = "Please select a category for the product!";
                return false;
            }
            if (context != null && !context.Categories.Any(c => c.CategoryId == product.CategoryId))
            {
                errorMessage = "Invalid category!";
                return false;
            }

            // Check Price
            // If Price is 0, user might not have entered it or entered invalid characters
            if (product.Price == 0)
            {
                errorMessage = "Please enter a valid product price (greater than 0)!";
                return false;
            }
            if (product.Price < 0)
            {
                errorMessage = "Product price cannot be less than 0!";
                return false;
            }
            if (product.Price > 50000000)
            {
                errorMessage = "Product price must not exceed 50 million!";
                return false;
            }

            // Check StockQuantity
            // StockQuantity of 0 is valid (product might be out of stock), but check for invalid input
            if (product.StockQuantity < 0)
            {
                errorMessage = "Stock quantity cannot be less than 0!";
                return false;
            }
            if (product.StockQuantity > 10000)
            {
                errorMessage = "Stock quantity must not exceed 10K!";
                return false;
            }

            // Check Description (optional)
            if (!string.IsNullOrEmpty(product.Description) && product.Description.Length > 500)
            {
                errorMessage = "Product description must not exceed 500 characters!";
                return false;
            }

            return true;
        }
    }
}