using BusinessObjects;
using DataAccessLayer;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FUMiniHotel_ProjectPRN212.Profile
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        private User _user;
        private readonly UserService userService;
        public ChangePassword(User user)
        {
            InitializeComponent();
            _user = user;
            userService = new UserService();
            txtCurrentPassword.Password = _user.Password;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string oldPassword = txtCurrentPassword.Password;
                string newPassword = txtNewPassword.Password;
                string confirmPass = txtConfirmPassword.Password;

                if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPass))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (newPassword != confirmPass)
                {
                    MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                bool result = userService.ChangePassword(_user.UserId, oldPassword, newPassword);

                if (result)
                {
                    MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;

            if (toggleButton == btnToggleCurrentPassword)
            {
                TogglePassword(txtCurrentPassword, txtCurrentPasswordVisible);
            }
            else if (toggleButton == btnToggleNewPassword)
            {
                TogglePassword(txtNewPassword, txtNewPasswordVisible);
            }
            else if (toggleButton == btnToggleConfirmPassword)
            {
                TogglePassword(txtConfirmPassword, txtConfirmPasswordVisible);
            }
        }

        private void TogglePassword(PasswordBox passwordBox, TextBox textBox)
        {
            if (textBox.Visibility == Visibility.Collapsed)
            {
                textBox.Text = passwordBox.Password;
                textBox.Visibility = Visibility.Visible;
                passwordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                passwordBox.Password = textBox.Text;
                textBox.Visibility = Visibility.Collapsed;
                passwordBox.Visibility = Visibility.Visible;
            }
        }

    }
}
