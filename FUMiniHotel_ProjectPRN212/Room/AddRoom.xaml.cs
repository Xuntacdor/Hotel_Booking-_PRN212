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
using DataAccessLayer;
namespace FUMiniHotel_ProjectPRN212.Room
{
    /// <summary>
    /// Interaction logic for AddRoom.xaml
    /// </summary>
    public partial class AddRoom : Window
    {
        public BusinessObjects.Room NewRoom { get; set; }
        private readonly RoomTypeDAO _roomTypeDao = new RoomTypeDAO();

        public AddRoom()
        {
            InitializeComponent();
            NewRoom = new BusinessObjects.Room();
            LoadRoomTypes();
        }

        private void LoadRoomTypes()
        {
            cbRoomType.ItemsSource = _roomTypeDao.GetAllRoomTypes();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                NewRoom.RoomNumber = txtRoomNumber.Text;
                NewRoom.RoomTypeId = (int)cbRoomType.SelectedValue;
                NewRoom.Description = txtDescription.Text;
                NewRoom.MaxCapacity = int.Parse(txtMaxCapacity.Text);
                NewRoom.PricePerDay = decimal.Parse(txtPricePerDay.Text);
                NewRoom.Status = (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text) ||
                cbRoomType.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtMaxCapacity.Text) ||
                string.IsNullOrWhiteSpace(txtPricePerDay.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(txtMaxCapacity.Text, out _))
            {
                MessageBox.Show("Sức chứa phải là số nguyên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!decimal.TryParse(txtPricePerDay.Text, out _))
            {
                MessageBox.Show("Giá phòng phải là số!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
