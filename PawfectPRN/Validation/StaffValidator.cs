using System;
using System.Linq;
using System.Text.RegularExpressions;
using PawfectPRN.Models;

namespace PawfectPRN.Validation
{
    public static class StaffValidator
    {
        public static bool ValidateStaff(Account staff, out string errorMessage, PawfectPrnContext context, bool isUpdate = false)
        {
            errorMessage = string.Empty;

            if (staff == null)
            {
                errorMessage = "Lỗi hệ thống: Không có dữ liệu nhân viên.";
                return false;
            }

            // Kiểm tra họ tên
            if (string.IsNullOrWhiteSpace(staff.FullName))
            {
                errorMessage = "Vui lòng nhập họ tên.";
                return false;
            }

            // Kiểm tra email hợp lệ
            if (string.IsNullOrWhiteSpace(staff.Email) || !Regex.IsMatch(staff.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errorMessage = "Email không hợp lệ.";
                return false;
            }

            // Kiểm tra email có bị trùng không (chỉ kiểm tra khi thêm mới)
            if (!isUpdate && context.Accounts.Any(a => a.Email.ToLower() == staff.Email.ToLower()))
            {
                errorMessage = "Email đã tồn tại! Vui lòng chọn email khác.";
                return false;
            }

            // Kiểm tra số điện thoại hợp lệ
            if (!string.IsNullOrWhiteSpace(staff.PhoneNumber) && !Regex.IsMatch(staff.PhoneNumber, @"^\d{10}$"))
            {
                errorMessage = "Số điện thoại phải có 10 chữ số.";
                return false;
            }

            // Kiểm tra địa chỉ
            if (string.IsNullOrWhiteSpace(staff.Address))
            {
                errorMessage = "Vui lòng nhập địa chỉ.";
                return false;
            }

            // Kiểm tra giới tính hợp lệ
            var validGenders = new[] { "male", "female", "other" };
            if (!validGenders.Contains(staff.Gender.ToLower()))
            {
                errorMessage = "Giới tính không hợp lệ.";
                return false;
            }

            // Kiểm tra mật khẩu (tối thiểu 6 ký tự)
            if (!isUpdate && (string.IsNullOrWhiteSpace(staff.Password) || staff.Password.Length < 6))
            {
                errorMessage = "Mật khẩu phải có ít nhất 6 ký tự.";
                return false;
            }

            return true;
        }
    }
}
