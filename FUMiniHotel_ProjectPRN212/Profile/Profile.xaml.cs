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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FUMiniHotel_ProjectPRN212.Profile
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    
    public partial class Profile : Page
    {
  
        private BusinessObjects.Customer _customer;
        private readonly CustomerService customerService;
        private readonly UserService userService;
        public Profile(BusinessObjects.Customer customer)
        {
            InitializeComponent();
            customerService = new CustomerService();
            userService = new UserService();
            _customer = customer;
            LoadCustomerData();
        }
        private void LoadCustomerData()
        {
            txtFullName.Text = _customer.FullName;
            txtPhone.Text = _customer.Telephone;
            txtEmail.Text = _customer.Email;
            txtBirthday.Text = _customer.Birthday?.ToString("dd/MM/yyyy") ?? "N/A";
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var editProfile = new EditProfile(_customer);
            if (editProfile.ShowDialog() == true)
            {
                customerService.UpdateCustomer(_customer);
                LoadCustomerData();
            }
        }

        //private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        //{
        //    ChangePassword changePassWindow = new ChangePassword(_user);
        //    changePassWindow.ShowDialog();
        //}
    }
}
