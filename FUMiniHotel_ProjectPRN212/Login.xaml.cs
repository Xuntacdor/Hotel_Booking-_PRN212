using BusinessObjects;
using FUMiniHotel_ProjectPRN212.Admin;
using FUMiniHotel_ProjectPRN212.Customer;
using Services;
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

namespace FUMiniHotel_ProjectPRN212
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly IUserService userService;
        private readonly ICustomerService customerService;
        public Login()
        {
            customerService = new CustomerService();
            InitializeComponent();
            userService = new UserService();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPass.Password;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập email và mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            bool loginResult = userService.Login(email, password);
            if (loginResult) 
            {
                //ktra xem co trung voi tk cua admin khong
                bool isAdmin = userService.IsAdmin(email);
                if (isAdmin) 
                {
                    MessageBox.Show("Hello Admin!, Welcome to FU Mini Hotel Management", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Hide();
                    AdminDashboard adminDashboard = new AdminDashboard();   
                    adminDashboard.Show();
                    this.Close();
                }
                var customer = customerService.GetCustomerByEmail(email);
                if (customer != null) 
                {
                    MessageBox.Show($"Hello {customer.FullName} , Welcome to FU Mini Hotel Management", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Hide();
                    CustomerDashboard customerDashboard = new CustomerDashboard(customer.CustomerId);
                    customerDashboard.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Email hoặc mật khẩu không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Close();
        }

        private void btnForgotPassword_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
