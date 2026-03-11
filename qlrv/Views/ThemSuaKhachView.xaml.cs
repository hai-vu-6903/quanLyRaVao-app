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

namespace qlrv.Views
{
    /// <summary>
    /// Interaction logic for ThemSuaKhachView.xaml
    /// </summary>
    public partial class ThemSuaKhachView : UserControl
    {
        public ThemSuaKhachView()
        {
            InitializeComponent();
            //this.DataContext = new ViewModels.ThemSuaKhachViewModel();
        }

        private void CCCD_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.ThemSuaKhachViewModel vm && !string.IsNullOrEmpty(vm.CCCD))
            {
                vm.TimKhachTheoCCCD();
            }
        }
    }
}
