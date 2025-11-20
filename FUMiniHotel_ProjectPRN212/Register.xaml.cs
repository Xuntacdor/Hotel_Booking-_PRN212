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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private readonly IUserService userService;
        public Register()
        {
            InitializeComponent();
            userService = new UserService();    
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string fullname = txtFullName.Text;
            string telephone = txtTelephone.Text;
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            DateTime? birthday = dpBirthday.SelectedDate;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(telephone) || string.IsNullOrEmpty(password)
                || (string.IsNullOrEmpty(confirmPassword) || birthday == null)) 
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin !", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (password != confirmPassword) 
            {
                MessageBox.Show("Mật khẩu và xác nhận mật khẩu không giống nhau !", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (userService.CheckEmailExists(email))
            {
                MessageBox.Show("Email đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            bool registerResult = userService.Register(email,"Customer","Active", fullname, telephone, password,birthday);
            if (registerResult) 
            {
                MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                Login login = new Login();
                login.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Đăng ký thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBackToLogin_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
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
