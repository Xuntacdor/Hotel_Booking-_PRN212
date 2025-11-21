using BusinessObjects;
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

namespace FUMiniHotel_ProjectPRN212.Employee
{
    /// <summary>
    /// Interaction logic for UpdateEmployee.xaml
    /// </summary>
    public partial class UpdateEmployee : Window
    {
        private int employeeId;
        private readonly EmployeeService employeeService;
        private readonly EmployeeRoleService employeeRoleService;
        public UpdateEmployee(int id)
        {
            InitializeComponent();
            employeeRoleService = new EmployeeRoleService();
            employeeService = new EmployeeService();
            employeeId = id;
            LoadRoles();
            LoadEmployee();
        }
        private void LoadEmployee()
        {
            var employee = employeeService.GetEmployeeById(employeeId);
            if (employee != null)
            {
                txtEmployeeId.Text = employee.EmployeeId.ToString();
                txtFullName.Text = employee.FullName;
                txtEmail.Text = employee.Email;
                txtTelephone.Text = employee.Telephone;
                dpHireDate.SelectedDate = employee.HireDate;
                txtSalary.Text = employee.Salary.ToString();
                cbRole.SelectedValue = employee.RoleId;
                cbStatus.SelectedIndex = employee.Status == "Active" ? 0 : 1;
            }
            else
            {
                MessageBox.Show("Không tìm thấy nhân viên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
        private void LoadRoles()
        {
            List<EmployeeRole> roles = employeeRoleService.GetEmployeeRoles();
            cbRole.ItemsSource = roles;
            cbRole.DisplayMemberPath = "RoleName";  
            cbRole.SelectedValuePath = "RoleId";   
        }


        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telephone = txtTelephone.Text.Trim();
            string salaryText = txtSalary.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(telephone) || string.IsNullOrWhiteSpace(salaryText))
            {
                MessageBox.Show("Tất cả các trường là bắt buộc.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidPhoneNumber(telephone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(salaryText, out decimal salary) || salary < 0)
            {
                MessageBox.Show("Lương phải là số hợp lệ và không âm.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            var employee = employeeService.GetEmployeeById(employeeId);
            if (employee != null)
            {
                employee.FullName = txtFullName.Text;
                employee.Email = txtEmail.Text;
                employee.Telephone = txtTelephone.Text;
                employee.HireDate = dpHireDate.SelectedDate ?? DateTime.Now;
                employee.Salary = decimal.TryParse(txtSalary.Text, out var salaryy) ? salaryy : 0;
                employee.RoleId = (int)cbRole.SelectedValue;
                employee.Status = cbStatus.SelectedIndex == 0 ? "Active" : "Inactive";

                employeeService.UpdateEmployee(employee);

                MessageBox.Show("Cập nhật nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            DialogResult = true;
            Close();
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{10,11}$");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
