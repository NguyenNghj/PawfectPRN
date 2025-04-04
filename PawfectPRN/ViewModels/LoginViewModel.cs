﻿using FirstCode.ViewModels;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using PawfectPRN.Models;
using PawfectPRN.Views.Admin;
using PawfectPRN.Views.Customer;
using FirstCode.Helper;
using PawfectPRN.Views.Staff;
using PawfectPRN.Validation;

namespace PawfectPRN.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
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

        public ICommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
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

        public void Login(object obj)
        {
            string validationMessage = LoginValidator.Validate(Username, Password);
            if (validationMessage != null)
            {
                MessageBox.Show(validationMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new PawfectPrnContext())
            {
                string hashedPassword = HashPassword(_password); 

                var user = context.Accounts.FirstOrDefault(u => u.Email == _username && u.Password == hashedPassword);
                if (user != null)
                {
                    Window nextWindow;

                    switch (user.RoleName.ToLower())
                    {
                        case "admin":
                            nextWindow = new Admin();
                            break;
                        case "customer":
                            nextWindow = new Customer(user);
                            break;
                        case "staff":
                            nextWindow = new Staff(user);
                            break;
                        default:
                            MessageBox.Show("Invalid role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                    }

                    nextWindow.Show();
                    CloseCurrentWindow();
                }
                else
                {
                    MessageBox.Show("Incorrect email or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CloseCurrentWindow()
        {
            Window? currentWindow = Application.Current.Windows.OfType<Window>()
                                      .FirstOrDefault(w => w.DataContext == this);
            currentWindow?.Close();
        }
    }
}
