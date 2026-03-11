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
using qlrv.ViewModels;

namespace qlrv.Views
{
    /// <summary>
    /// Interaction logic for ThemSuaQuanNhanView.xaml
    /// </summary>
    public partial class ThemSuaQuanNhanView : UserControl
    {
        public ThemSuaQuanNhanView()
        {
            InitializeComponent();

            //this.DataContext = new ThemSuaQuanNhanViewModel();
        }

        private void CCCD_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is ThemSuaQuanNhanViewModel vm && !string.IsNullOrEmpty(vm.CCCD))
            {
                vm.TimQuanNhanTheoCCCD();
            }
        }
    }
}
