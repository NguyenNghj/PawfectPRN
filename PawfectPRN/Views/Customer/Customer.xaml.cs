using PawfectPRN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PawfectPRN.Views.Customer
{
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer : Window
    {
        private Account _account;

        public Customer(Account account)
        {
            InitializeComponent();
            _account = account; 
            MainFrame.Content = new ProfileView(_account);
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ProfileView(_account);
        }
        private void Logout_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Hiển thị hộp thoại xác nhận logout
            MessageBoxResult result = MessageBox.Show("Are you sure you want to logout?",
                                                      "Logout Confirmation",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Điều hướng về màn hình đăng nhập
                Login loginWindow = new Login();
                loginWindow.Show();

                // Đóng cửa sổ hiện tại
                this.Close();
            }
        }

        private void Booking_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new CustomerBookingView(_account);
        }
    }

}
