using PawfectPRN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawfectPRN.Validation
{
    public class StaffBookingValidator
    {
        public static bool ValidateBooking(PethotelBooking booking, DateTime? bookingDate, DateTime? bookingTime, DateTime? checkoutDate, DateTime? checkoutTime, List<string> validStatuses, out string errorMessage, PawfectPrnContext context = null)
        {
            errorMessage = string.Empty;

            // Check AccountId
            if (booking.AccountId == 0)
            {
                errorMessage = "Please select a customer (Account)!";
                return false;
            }
            if (context != null && !context.Accounts.Any(a => a.AccountId == booking.AccountId))
            {
                errorMessage = "Invalid customer (Account)!";
                return false;
            }

            // Check PethotelId
            if (booking.PethotelId == 0)
            {
                errorMessage = "Please select a pet hotel!";
                return false;
            }
            if (context != null && !context.PetHotels.Any(p => p.PethotelId == booking.PethotelId))
            {
                errorMessage = "Invalid pet hotel!";
                return false;
            }

            // Check BookingDate and BookingTime
            if (!bookingDate.HasValue)
            {
                errorMessage = "Booking date is required!";
                return false;
            }
            if (!bookingTime.HasValue)
            {
                errorMessage = "Booking time is required!";
                return false;
            }

            // Check CheckoutDate and CheckoutTime
            if (!checkoutDate.HasValue)
            {
                errorMessage = "Checkout date is required!";
                return false;
            }
            if (!checkoutTime.HasValue)
            {
                errorMessage = "Checkout time is required!";
                return false;
            }

            // Combine date and time to validate logic
            var bookingDateTime = bookingDate.Value.Date + bookingTime.Value.TimeOfDay;
            var checkoutDateTime = checkoutDate.Value.Date + checkoutTime.Value.TimeOfDay;

            // Check if CheckoutDate is later than or equal to BookingDate
            if (checkoutDateTime < bookingDateTime)
            {
                errorMessage = "Checkout date and time must be later than or equal to booking date and time!";
                return false;
            }

            // Check duration (optional: limit to 30 days)
            var days = (checkoutDateTime - bookingDateTime).TotalDays;
            if (days > 30)
            {
                errorMessage = "Booking duration cannot exceed 30 days!";
                return false;
            }

            // Check Status
            if (string.IsNullOrWhiteSpace(booking.Status))
            {
                errorMessage = "Status is required!";
                return false;
            }
            if (!validStatuses.Contains(booking.Status))
            {
                errorMessage = $"Status must be one of the following: {string.Join(", ", validStatuses)}!";
                return false;
            }

            // Check Price (if already calculated)
            if (booking.Price.HasValue)
            {
                if (booking.Price < 0)
                {
                    errorMessage = "Price cannot be less than 0!";
                    return false;
                }
                if (booking.Price > 100000000) // Giới hạn giá tối đa (ví dụ: 100 triệu)
                {
                    errorMessage = "Price must not exceed 100 million!";
                    return false;
                }
            }

            // Check ServiceDetails (optional)
            if (!string.IsNullOrEmpty(booking.ServiceDetails) && booking.ServiceDetails.Length > 500)
            {
                errorMessage = "Service details must not exceed 500 characters!";
                return false;
            }

            return true;
        }
    }
}
