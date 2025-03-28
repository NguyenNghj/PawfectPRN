﻿using FirstCode.Helper;
using FirstCode.ViewModels;
using Newtonsoft.Json;
using PawfectPRN.Models;
using PawfectPRN.Validation;
using PawfectPRN.Validation.PawfectPRN.Validation;
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
                using (var context = new PawfectPrnContext())
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

            using (var context = new PawfectPrnContext())
            {
                var accountToUpdate = context.Accounts.Find(selectitem.AccountId);
                if (accountToUpdate != null)
                {
                    // Validate address
                    string addressErrorMessage;
                    if (!ProfileValidator.ValidateAddress(textboxitem.Address, out addressErrorMessage))
                    {
                        MessageBox.Show(addressErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

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

                    MessageBox.Show("Information updated successfully!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void ChangePassword(object obj)
        {
            if (selectitem == null) return;

            using (var context = new PawfectPrnContext())
            {
                var accountToChangePassword = context.Accounts.Find(selectitem.AccountId);
                if (accountToChangePassword != null)
                {
                    string errorMessage;
                    if (!ProfileValidator.ValidatePasswordChange(OldPassword, NewPassword, ConfirmNewPassword, accountToChangePassword.Password, HashPassword, out errorMessage))
                    {
                        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    accountToChangePassword.Password = HashPassword(NewPassword);
                    context.SaveChanges();

                    MessageBox.Show("Password changed successfully!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);

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
