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
using PawfectPRN.ViewModels;

namespace PawfectPRN.Views.Admin
{
    /// <summary>
    /// Interaction logic for PetHotelView.xaml
    /// </summary>
    public partial class PetHotelView : Page
    {
        public PetHotelView()
        {
            InitializeComponent();
            this.DataContext = new PetHotelViewModel();

        }
    }
}

