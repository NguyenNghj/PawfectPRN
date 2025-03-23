using FirstCode.Helper;
using FirstCode.ViewModels;
using Newtonsoft.Json;
using PawfectPRN.Models;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PE_180897_NguyenTriNghi.ViewBaseModel
{
    public class ProfileViewModel : BaseViewModel
    {
        public Account Account { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        public ProfileViewModel(Account account)
        {
            Account = account;
            Load();
            UpdateCommand = new RelayCommand(Update);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
            textboxitem = account;
        }

        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            return string.Join(" ", input.Split(' ')
                        .Where(word => !string.IsNullOrWhiteSpace(word))
                        .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
        }

        private Account _textboxitem;
        public Account textboxitem
        {
            get { return _textboxitem; }
            set
            {
                if (value != null)
                {
                    value.FullName = CapitalizeFirstLetter(value.FullName);
                    value.Address = CapitalizeFirstLetter(value.Address);
                    value.Gender = CapitalizeFirstLetter(value.Gender);
                }
                _textboxitem = value;
                OnPropertyChanged(nameof(textboxitem));
            }
        }

        private string _oldPassword;
        public string OldPassword
        {
            get { return _oldPassword; }
            set
            {
                _oldPassword = value;
                OnPropertyChanged(nameof(OldPassword));
            }
        }

        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        private string _confirmNewPassword;
        public string ConfirmNewPassword
        {
            get { return _confirmNewPassword; }
            set
            {
                _confirmNewPassword = value;
                OnPropertyChanged(nameof(ConfirmNewPassword));
            }
        }

        private Account _selectitem;
        public Account selectitem
        {
            get { return _selectitem; }
            set
            {
                _selectitem = value;
                OnPropertyChanged(nameof(selectitem));
                if (_selectitem != null)
                {
                    _textboxitem = JsonConvert.DeserializeObject<Account>(JsonConvert.SerializeObject(_selectitem));
                    OnPropertyChanged(nameof(textboxitem));
                }
            }
        }

        private void Load()
        {
            if (Account != null)
            {
                using (var context = new PawfectprnContext())
                {
                    var accountFromDb = context.Accounts.FirstOrDefault(a => a.AccountId == Account.AccountId);
                    if (accountFromDb != null)
                    {
                        selectitem = accountFromDb;
                        _textboxitem = JsonConvert.DeserializeObject<Account>(JsonConvert.SerializeObject(accountFromDb));
                        OnPropertyChanged(nameof(textboxitem));
                    }
                }
            }
        }

        private void Update(object obj)
        {
            if (selectitem == null) return;

            using (var context = new PawfectprnContext())
            {
                var accountToUpdate = context.Accounts.Find(selectitem.AccountId);
                if (accountToUpdate != null)
                {
                    accountToUpdate.FullName = CapitalizeFirstLetter(textboxitem.FullName);
                    accountToUpdate.Email = textboxitem.Email;
                    accountToUpdate.PhoneNumber = textboxitem.PhoneNumber;
                    accountToUpdate.Address = CapitalizeFirstLetter(textboxitem.Address);
                    accountToUpdate.Gender = CapitalizeFirstLetter(textboxitem.Gender);

                    context.SaveChanges();

                    selectitem.FullName = accountToUpdate.FullName;
                    selectitem.Email = accountToUpdate.Email;
                    selectitem.PhoneNumber = accountToUpdate.PhoneNumber;
                    selectitem.Address = accountToUpdate.Address;
                    selectitem.Gender = accountToUpdate.Gender;

                    MessageBox.Show("Thông tin đã được cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ChangePassword(object obj)
        {
            if (selectitem == null) return;

            if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewPassword != ConfirmNewPassword)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new PawfectprnContext())
            {
                var accountToChangePassword = context.Accounts.Find(selectitem.AccountId);
                if (accountToChangePassword != null)
                {
                    string hashedOldPassword = HashPassword(OldPassword);
                    if (accountToChangePassword.Password != hashedOldPassword)
                    {
                        MessageBox.Show("Mật khẩu cũ không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string hashedNewPassword = HashPassword(NewPassword);
                    accountToChangePassword.Password = hashedNewPassword;
                    context.SaveChanges();

                    MessageBox.Show("Mật khẩu đã được thay đổi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    OldPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmNewPassword = string.Empty;
                }
            }
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
    }
}
