using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BusinessObjects;

namespace FUMiniHotel_ProjectPRN212.Customer
{
    /// <summary>
    /// Interaction logic for CustomerDashboard.xaml
    /// </summary>
    public partial class CustomerDashboard : Window
    {

        private readonly CustomerService customerService;
        private readonly UserService userService;
        private int _loggedInCustomerId;
        public CustomerDashboard(int id)
        {
            InitializeComponent();
            _loggedInCustomerId = id;
            customerService = new CustomerService();
            userService = new UserService();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.Home());
            LoadCustomerInfo();
        }
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }
        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có muốn đăng xuất không ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Login login = new Login();
                this.Close();
                login.Show();
            }
        }
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.Home());
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            BusinessObjects.Customer currentCustomer = customerService.GetCustomerById(_loggedInCustomerId);

            if (currentCustomer != null)
            {
                UncheckAllButtons();
                ((ToggleButton)sender).IsChecked = true;
                MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.Profile.Profile(currentCustomer));
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin khách hàng hoặc người dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BookingsButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            
        }

        private void RoomsButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.Pages.CustomerBookingPage());
        }

        private void ChatBotButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.ChatBot.ChatBotPage(_loggedInCustomerId));
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {

            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            MessageBoxResult result = MessageBox.Show("Bạn có muốn đăng xuất không ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Login login = new Login();
                this.Close();
                login.Show();
            }
        }
        private void UncheckAllButtons()
        {
            HomeButton.IsChecked = false;
            ProfileButton.IsChecked = false;
            RoomsButton.IsChecked = false;
            BookingsButton.IsChecked = false;
            ChangePasswordButton.IsChecked = false;
            ChatBotButton.IsChecked = false;
            LogoutButton.IsChecked = false;
        }
        private void LoadCustomerInfo()
        {
            BusinessObjects.Customer currentCustomer = customerService.GetCustomerById(_loggedInCustomerId);

            if (currentCustomer != null)
            {
                txtCustomerName.Text = $"Xin chào, {currentCustomer.FullName}";
            }
            else
            {
                txtCustomerName.Text = "Xin chào!";
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
