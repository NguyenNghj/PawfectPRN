using PawfectPRN.Models;
using System;

namespace PawfectPRN.Validation
{
    public class CustomerBookingValidator
    {
        public (bool IsValid, string ErrorMessage) ValidateAdd(PethotelBooking booking, PetHotel petHotel)
        {
            if (booking == null)
            {
                return (false, "Booking information is missing!");
            }

            if (booking.PethotelId == 0)
            {
                return (false, "Please select a Pet Room!");
            }

            if (booking.BookingDate == null)
            {
                return (false, "Booking Date must be provided!");
            }

            if (booking.CheckoutDate == null || booking.CheckoutDate == default)
            {
                return (false, "Checkout Date must be provided!");
            }

            if (booking.CheckoutDate <= booking.BookingDate)
            {
                return (false, "Checkout Date must be later than Booking Date!");
            }

            if (petHotel == null)
            {
                return (false, "Selected Pet Hotel not found!");
            }

            return (true, string.Empty);
        }

        public (bool IsValid, string ErrorMessage) ValidateUpdate(PethotelBooking selectedItem, PethotelBooking textboxItem, PetHotel petHotel)
        {
            if (selectedItem == null)
            {
                return (false, "Please select a booking to update!");
            }

            if (textboxItem == null)
            {
                return (false, "Booking information is missing!");
            }

            if (textboxItem.PethotelId == 0)
            {
                return (false, "Please select a Pet Hotel!");
            }

            if (textboxItem.BookingDate == null)
            {
                return (false, "Booking Date must be provided!");
            }

            if (textboxItem.CheckoutDate == null)
            {
                return (false, "Checkout Date must be provided!");
            }

            if (textboxItem.CheckoutDate <= textboxItem.BookingDate)
            {
                return (false, "Checkout Date must be later than Booking Date!");
            }

            if (petHotel == null)
            {
                return (false, "Selected Pet Hotel not found!");
            }

            return (true, string.Empty);
        }
    }
}