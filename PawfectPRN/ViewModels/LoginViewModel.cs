using FirstCode.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using PawfectPRN.Models;
using FirstCode.Helper;
using PawfectPRN.Views.Admin;
using PawfectPRN.Views.Customer;

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

        //public void Login(object obj)
        //{
        //    //if (_username == "admin" && _password == "123")
        //    //{
        //    //    var adminView = new Admin();
        //    //    adminView.Show();

        //    //    Application.Current.Windows[0]?.Close();
        //    //    return;
        //    //}
        //    using (var context = new PawfectprnContext())
        //    {
        //        var user = context.Accounts.FirstOrDefault(u => u.Email == _username && u.Password == _password);
        //        if (user != null)
        //        {
        //            var mainWindow = new MainWindow();
        //            mainWindow.Show();
        //            Application.Current.Windows[0]?.Close();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}

        public void Login(object obj)
        {
            using (var context = new PawfectprnContext())
            {
                var user = context.Accounts.FirstOrDefault(u => u.Email == _username && u.Password == _password);
                if (user != null)
                {
                    Window nextWindow;

                    switch (user.RoleName.ToLower())
                    {
                        case "admin":
                            nextWindow = new Admin(); // Cửa sổ dành cho Admin
                            break;
                        case "customer":
                            nextWindow = new Customer(); // Cửa sổ dành cho User
                            break;
                        default:
                            MessageBox.Show("Role không hợp lệ.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                    }

                    nextWindow.Show();
                    Application.Current.Windows[0]?.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public ICommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

    }
}
