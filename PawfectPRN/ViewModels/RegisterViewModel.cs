using FirstCode.Helper;
using PawfectPRN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using FirstCode.ViewModels;
using System.Windows.Navigation;
using PawfectPRN.Views;

namespace PawfectPRN.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        private string _gender;
        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand { get; set; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(Register);
        }

        private string HashPassword(string password)
        {
            string hashed = password;
            for (int i = 0; i < 5; i++)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(hashed);
                hashed = Convert.ToBase64String(bytes);
            }
            return hashed;
        }

        private bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, "^[\\w-\\.+]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
        }

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, "^\\d{10,11}$");
        }

        public void Register(object obj)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                MessageBox.Show("Vui lòng nhập họ và tên.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Email) || !ValidateEmail(Email))
            {
                MessageBox.Show("Vui lòng nhập email hợp lệ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(PhoneNumber) || !ValidatePhoneNumber(PhoneNumber))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại hợp lệ (10-11 số).", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("Vui lòng xác nhận mật khẩu.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Gender))
            {
                MessageBox.Show("Vui lòng chọn giới tính.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new PawfectprnContext())
            {
                if (context.Accounts.Any(a => a.Email == Email))
                {
                    MessageBox.Show("Email đã được đăng ký.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newUser = new Account
                {
                    FullName = FullName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Address = Address,
                    Gender = Gender.ToString().ToLower(),
                    Password = HashPassword(Password), // Hash mật khẩu 5 lần trước khi lưu
                    RoleName = "customer" // Mặc định role là customer
                };

                context.Accounts.Add(newUser);
                context.SaveChanges();

                MessageBox.Show("Đăng ký thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Chuyển về màn hình đăng nhập
                NavigateToLogin();
            }
        }
        private void NavigateToLogin()
        {
            // Mở cửa sổ đăng nhập
            Login loginWindow = new Login();
            loginWindow.Show();

            // Tìm cửa sổ hiện tại bằng cách lấy cửa sổ chính hoặc cửa sổ đang hoạt động
            Window currentWindow = Application.Current.Windows.OfType<Window>()
                                        .FirstOrDefault(w => w.DataContext == this);

            // Đóng cửa sổ hiện tại
            currentWindow?.Close();
        }
    }
}
