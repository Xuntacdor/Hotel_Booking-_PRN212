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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
namespace FUMiniHotel_ProjectPRN212.Employee
{
    /// <summary>
    /// Interaction logic for EmployeeManagement.xaml
    /// </summary>
    public partial class EmployeeManagement : Page
    {
        private readonly IEmployeeService employeeService;
        private readonly FuminiHotelProjectPrn212Context context;
        public EmployeeManagement()
        {
            InitializeComponent();
            employeeService = new EmployeeService();
            context = new FuminiHotelProjectPrn212Context();
            LoadEmployees();
        }
        private void LoadEmployees()
        {
            try
            {
                var employees = context.Employees
                    .Join(context.EmployeeRoles,
                          e => e.RoleId, 
                          r => r.RoleId,
                          (e, r) => new 
                          {
                              e.EmployeeId,
                              e.FullName,
                              RoleName = r.RoleName, 
                              e.Email,
                              e.Telephone,
                              e.HireDate,
                              e.Status,
                              e.Salary
                          }).ToList();

                dgEmployees.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi khi tai du lieu nhan vien: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEmployee();
                if(dialog.ShowDialog() == true)
            {
                employeeService.AddEmployee(dialog.NewEmployee);
                dgEmployees.ItemsSource = employeeService.GetEmployees().ToList();
            }
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgEmployees.SelectedItem as dynamic;
            if (selectedItem != null)
            {
                var dialog = new UpdateEmployee(selectedItem.EmployeeId);
                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    var updatedEmployee = employeeService.GetEmployeeById(selectedItem.EmployeeId);
                    if (updatedEmployee != null)
                    {
                        employeeService.UpdateEmployee(updatedEmployee);
                        LoadEmployees();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgEmployees.SelectedItem as dynamic;

            if (selectedItem != null)
            {
                int employeeId = selectedItem.EmployeeId;

                MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn xóa nhân viên này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var employee = context.Employees.Find(employeeId);
                    if (employee != null)
                    {
                        employeeService.DeleteEmployee(employee.EmployeeId);
                        LoadEmployees(); 
                        MessageBox.Show("Nhân viên đã bị xóa!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Hãy chọn một nhân viên để xóa!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = employeeService.GetEmployees().AsQueryable();
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                search = search.Where(s => s.FullName.ToLower().Contains(txtSearch.Text.ToLower()));            
            }
            dgEmployees.ItemsSource = search.ToList();
        }
    }
}
