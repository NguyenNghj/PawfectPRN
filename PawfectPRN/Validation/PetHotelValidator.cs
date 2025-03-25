using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawfectPRN.Models;

namespace PawfectPRN.Validation
{
    public static class PetHotelValidator
    {
        public static bool ValidatePetHotel(PetHotel hotel, out string errorMessage, PawfectPrnContext context)
        {
            errorMessage = string.Empty;

            if (hotel == null)
            {
                errorMessage = "Lỗi hệ thống: Không có dữ liệu khách sạn.";
                return false;
            }

            // Kiểm tra tên khách sạn
            if (string.IsNullOrWhiteSpace(hotel.PethotelName))
            {
                errorMessage = "Vui lòng nhập tên khách sạn.";
                return false;
            }

            // Kiểm tra trùng lặp tên khách sạn, nhưng bỏ qua khách sạn hiện tại
            if (context.PetHotels.Any(p => p.PethotelName.ToLower() == hotel.PethotelName.ToLower() && p.PethotelId != hotel.PethotelId))
            {
                errorMessage = "Tên khách sạn đã tồn tại! Vui lòng chọn tên khác.";
                return false;
            }


            // Kiểm tra Price hợp lệ (phải là số và lớn hơn 0)
            if (hotel.Price <= 0)
            {
                errorMessage = "Giá phải lớn hơn 0.";
                return false;
            }

            // Kiểm tra trạng thái hợp l

            return true;
        }
    }
}
