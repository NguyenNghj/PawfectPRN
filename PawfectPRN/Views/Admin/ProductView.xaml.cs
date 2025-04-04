﻿
using PawfectPRN.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PawfectPRN.Views.Admin
{
    public partial class ProductView : Page
    {
        public ProductView()
        {
            InitializeComponent();
            this.DataContext = new ProductViewModel();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // Chỉ cho phép số
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
