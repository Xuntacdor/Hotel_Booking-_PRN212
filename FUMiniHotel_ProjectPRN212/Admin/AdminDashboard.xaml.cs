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

namespace FUMiniHotel_ProjectPRN212.Admin
{
    /// <summary>
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            Loaded += AdminDashboard_Loaded;
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
        private void AdminDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.Home());
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

        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.Employee.EmployeeManagement());
        }

        private void RoomsButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.Room.RoomManagement());
        }

        private void BookingsButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.Booking.BookingManagement());
        }

        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.Service.ServiceManagement());
        }

        private void CustomersButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.AdminCustomer.CustomerManagement());
        }

        private void InvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.Invoice.InvoiceManagement());
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

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            ((ToggleButton)sender).IsChecked = true;
            MainContent.NavigationService.Navigate(new FUMiniHotel_ProjectPRN212.Home());
        }
        private void UncheckAllButtons()
        {
            HomeButton.IsChecked = false;
            EmployeesButton.IsChecked = false;
            RoomsButton.IsChecked = false;
            BookingsButton.IsChecked = false;
            ServicesButton.IsChecked = false;
            CustomersButton.IsChecked = false;
            InvoicesButton.IsChecked = false;
            LogoutButton.IsChecked = false;
        }

    }
}
