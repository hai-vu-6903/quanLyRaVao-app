using qlrv.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Wpf.Ui.Controls;
using UiMessageBox = Wpf.Ui.Controls.MessageBox;

namespace qlrv
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            StartClock();
        }

        private void StartClock()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += (s, e) =>
            {
                DateTime now = DateTime.Now;

                // Ví dụ: Thứ Ba - 07/01/2026
                navDate.Content = $"{GetVietnameseDay(now)} - {now:dd/MM/yyyy}";

                // Giờ
                navTime.Content = now.ToString("HH:mm:ss");
            };

            _timer.Start();
        }

        private string GetVietnameseDay(DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday => "Thứ Hai",
                DayOfWeek.Tuesday => "Thứ Ba",
                DayOfWeek.Wednesday => "Thứ Tư",
                DayOfWeek.Thursday => "Thứ Năm",
                DayOfWeek.Friday => "Thứ Sáu",
                DayOfWeek.Saturday => "Thứ Bảy",
                DayOfWeek.Sunday => "Chủ Nhật",
                _ => ""
            };
        }

        private void BtnQuanNhan_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.QuanNhanView());
        }

        private void BtnKhach_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.KhachView());
        }

        private void BtnLichSu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.LichSuView());
        }

        private void BtnBaoCao_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.BaoCaoView());
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Max_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            var messageBox = new UiMessageBox
            {
                Title = "Xác nhận",
                Content = "Bạn có chắc muốn thoát không?",
                PrimaryButtonText = "Đồng ý",
                CloseButtonText = "Hủy"
            };

            var result = await messageBox.ShowDialogAsync();

            if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
            {
                Application.Current.Shutdown();
            }
        }
    }
}