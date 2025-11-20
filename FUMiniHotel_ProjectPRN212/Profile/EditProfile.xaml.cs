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
using System.Windows.Shapes;

namespace FUMiniHotel_ProjectPRN212.Profile
{
    /// <summary>
    /// Interaction logic for EditProfile.xaml
    /// </summary>
    public partial class EditProfile : Window
    {
        private BusinessObjects.Customer _customer;
        public EditProfile(BusinessObjects.Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            LoadProfiles();
        }
        private void LoadProfiles()
        {
            txtFullName.Text = _customer.FullName;
            txtPhone.Text = _customer.Telephone;
            txtEmail.Text = _customer.Email;
            dpBirthday.SelectedDate = _customer.Birthday;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string fullname = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            DateTime? birthday = dpBirthday.SelectedDate;
            if (string.IsNullOrEmpty(fullname) || string.IsNullOrWhiteSpace(phone) || birthday == null ) 
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidName(fullname))
            {
                MessageBox.Show("Tên không được chứa ký tự đặc biệt.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _customer.FullName = fullname;
            _customer.Birthday = birthday;
            _customer.Telephone = phone;

            MessageBox.Show("Cập nhật thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
        private bool IsValidName(string input) => Regex.IsMatch(input, @"^[\p{L}\s]+$");

        private bool IsValidPhone(string phone) => Regex.IsMatch(phone, @"^\d{10,12}$");
    }
}
