using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Services;
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

namespace FUMiniHotel_ProjectPRN212.Employee
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        public BusinessObjects.Employee NewEmployee { get; set; }
        private readonly FuminiHotelProjectPrn212Context context;
        private readonly EmployeeRoleService employeeRoleService;
        public AddEmployee()
        {
            employeeRoleService = new EmployeeRoleService();
            InitializeComponent();
            NewEmployee = new BusinessObjects.Employee();
            context = new FuminiHotelProjectPrn212Context();
            LoadRoles();
        }
        private void LoadRoles()
        {
            List<EmployeeRole> roles = employeeRoleService.GetEmployeeRoles();
            cbRole.ItemsSource = roles; 
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            string fullname = txtFullName.Text;
            string email = txtEmail.Text;
            string telephone = txtTelephone.Text;
            DateTime? hireDate = (DateTime?)dpHireDate.SelectedDate;
            string salary = txtSalary.Text;
            int? roleId = cbRole.SelectedValue as int?;
            string? status = (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
            string password = "password123";

            if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(telephone) || string.IsNullOrEmpty(salary) ||
                roleId == null || hireDate == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ tất cả các trường!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Regex.IsMatch(telephone, @"^\d+$"))
            {
                MessageBox.Show("Số điện thoại chỉ được chứa số!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (!decimal.TryParse(salary, out decimal salaryT) || salaryT < 0)
            {
                MessageBox.Show("Lương phải là một số hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var newUser = new User
                {
                    Email = email,
                    Password = "password123", 
                    Role = "Employee",
                    Status = "Active"
                };

                context.Users.Add(newUser);
                context.SaveChanges();


                NewEmployee.FullName = fullname;
                NewEmployee.Email = email;
                NewEmployee.HireDate = (DateTime)hireDate;
                NewEmployee.Telephone = telephone;
                NewEmployee.Salary = salaryT;
                NewEmployee.RoleId = roleId;
                NewEmployee.Status = status;
                NewEmployee.UserId = newUser.UserId;
                

                MessageBox.Show("Nhân viên được thêm thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
