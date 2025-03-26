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

namespace PawfectPRN.Views.Admin
{
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
            MainFrame.Content = new ProductView();
        }


        private void Product_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ProductView();
        }

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new CategoryView();
        }
        private void Staff_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new StaffView();
        }

        private void PetHotel_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PetHotelView();
        }

        private void Logout_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show("Are you sure you want to logout?",
                                                      "Logout Confirmation",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Login loginWindow = new Login();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}
