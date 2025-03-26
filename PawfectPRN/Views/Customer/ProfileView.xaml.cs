using PawfectPRN.Models;
using PE_180897_NguyenTriNghi.ViewBaseModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PawfectPRN.Views.Customer
{
    public partial class ProfileView : Page
    {
        public ProfileView(Account account)
        {
            InitializeComponent();
            DataContext = new ProfileViewModel(account);
        }

        private void OldPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel viewModel)
            {
                viewModel.OldPassword = ((PasswordBox)sender).Password;
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel viewModel)
            {
                viewModel.NewPassword = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmNewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel viewModel)
            {
                viewModel.ConfirmNewPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
