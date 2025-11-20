using BusinessObjects;
using Services;
using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for EditEmployeeProfile.xaml
    /// </summary>
    public partial class EditEmployeeProfile : Window
    {
        private BusinessObjects.Employee _employee;
        private readonly EmployeeService employeeService;
        private readonly FuminiHotelProjectPrn212Context context;

        public EditEmployeeProfile(BusinessObjects.Employee employee)
        {
            InitializeComponent();
            _employee = employee;
            employeeService = new EmployeeService();
            context = new FuminiHotelProjectPrn212Context();
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            // Load lại employee với thông tin Role
            var employeeWithRole = context.Employees
                .Include(e => e.Role)
                .FirstOrDefault(e => e.EmployeeId == _employee.EmployeeId);

            if (employeeWithRole != null)
            {
                _employee = employeeWithRole;
            }

            txtFullName.Text = _employee.FullName ?? string.Empty;
            txtPhone.Text = _employee.Telephone ?? string.Empty;
            txtEmail.Text = _employee.Email ?? string.Empty;
            dpHireDate.SelectedDate = _employee.HireDate;
            
            // Các trường không được chỉnh sửa (chỉ hiển thị)
            txtRole.Text = _employee.Role?.RoleName ?? "N/A";
            txtSalary.Text = _employee.Salary?.ToString("N0") + " VND" ?? "N/A";
            txtStatus.Text = _employee.Status ?? "N/A";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string fullname = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();
            DateTime? hireDate = dpHireDate.SelectedDate;

            if (string.IsNullOrEmpty(fullname) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(email) || hireDate == null)
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

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Chỉ cập nhật các trường được phép chỉnh sửa
                // Giữ nguyên Role, Salary, Status
                _employee.FullName = fullname;
                _employee.Telephone = phone;
                _employee.Email = email;
                _employee.HireDate = hireDate.Value;

                // Cập nhật vào database
                employeeService.UpdateEmployee(_employee);

                MessageBox.Show("Cập nhật thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private bool IsValidName(string input) => Regex.IsMatch(input, @"^[\p{L}\s]+$");

        private bool IsValidPhone(string phone) => Regex.IsMatch(phone, @"^\d{10,12}$");

        private bool IsValidEmail(string email) => Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}

