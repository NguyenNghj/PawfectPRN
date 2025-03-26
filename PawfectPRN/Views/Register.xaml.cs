using PawfectPRN.ViewModels;
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

namespace PawfectPRN.Views
{
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel();
        }
        public void LoginTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close(); ;
        }
    }
}
