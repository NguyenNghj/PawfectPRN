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
    }

}
