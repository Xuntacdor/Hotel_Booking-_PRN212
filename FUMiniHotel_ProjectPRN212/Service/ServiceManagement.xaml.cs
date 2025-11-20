using DataAccessLayer;
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

namespace FUMiniHotel_ProjectPRN212.Service
{
    /// <summary>
    /// Interaction logic for ServiceManagement.xaml
    /// </summary>
    public partial class ServiceManagement : Page
    {
        private readonly ServiceDAO _serviceDao = new ServiceDAO();
        public ServiceManagement()
        {
            InitializeComponent();
            LoadServices();
        }
        private void LoadServices()
        {
            dgServices.ItemsSource = _serviceDao.GetAllServices();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                dgServices.ItemsSource = _serviceDao.SearchServices(txtSearch.Text);
            }
            else
            {
                LoadServices();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //var addDialog = new AddServiceDialog();
            //if (addDialog.ShowDialog() == true)
            //{
            //    _serviceDao.AddService(addDialog.Service);
            //    LoadServices();
            //    MessageBox.Show("Thêm dịch vụ thành công!", "Thành công",
            //                  MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            //if (dgServices.SelectedItem is BusinessObjects.Service selectedService)
            //{
            //    var editDialog = new EditServiceDialog(selectedService);
            //    if (editDialog.ShowDialog() == true)
            //    {
            //        _serviceDao.UpdateService(editDialog.Service);
            //        LoadServices();
            //        MessageBox.Show("Cập nhật dịch vụ thành công!", "Thành công",
            //                      MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Vui lòng chọn một dịch vụ để sửa", "Cảnh báo",
            //                  MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgServices.SelectedItem is BusinessObjects.Service selectedService)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa dịch vụ này?",
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _serviceDao.DeleteService(selectedService.ServiceId);
                    LoadServices();
                    MessageBox.Show("Xóa dịch vụ thành công!", "Thành công",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dịch vụ để xóa", "Cảnh báo",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
