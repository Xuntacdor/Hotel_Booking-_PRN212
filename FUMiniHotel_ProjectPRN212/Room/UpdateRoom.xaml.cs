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
using System.Windows.Shapes;

namespace FUMiniHotel_ProjectPRN212.Room
{
    /// <summary>
    /// Interaction logic for UpdateRoom.xaml
    /// </summary>
    public partial class UpdateRoom : Window
    {
        public BusinessObjects.Room UpdatedRoom { get; set; }
        private readonly int _roomId;
        private readonly RoomDAO _roomDao = new RoomDAO();
        private readonly RoomTypeDAO _roomTypeDao = new RoomTypeDAO();

        public UpdateRoom(int roomId)
        {
            InitializeComponent();
            _roomId = roomId;
            LoadRoomData();
            LoadRoomTypes();
        }

        private void LoadRoomData()
        {
            UpdatedRoom = _roomDao.GetRoomById(_roomId);
            if (UpdatedRoom != null)
            {
                txtRoomId.Text = UpdatedRoom.RoomId.ToString();
                txtRoomNumber.Text = UpdatedRoom.RoomNumber;
                txtDescription.Text = UpdatedRoom.Description;
                txtMaxCapacity.Text = UpdatedRoom.MaxCapacity.ToString();
                txtPricePerDay.Text = UpdatedRoom.PricePerDay.ToString();

                if (UpdatedRoom.Status == "Còn phòng")
                {
                    cbStatus.SelectedIndex = 0;
                }
                else if (UpdatedRoom.Status == "Hết phòng")
                {
                    cbStatus.SelectedIndex = 1;
                }
                else
                {
                    cbStatus.SelectedIndex = -1; 
                }

                cbRoomType.SelectedValue = UpdatedRoom.RoomTypeId;
            }
        }

        private void LoadRoomTypes()
        {
            var roomTypes = _roomTypeDao.GetAllRoomTypes();
            cbRoomType.ItemsSource = roomTypes;
            cbRoomType.DisplayMemberPath = "TypeName";
            cbRoomType.SelectedValuePath = "RoomTypeId";
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                UpdatedRoom.RoomNumber = txtRoomNumber.Text;
                UpdatedRoom.RoomTypeId = (int)cbRoomType.SelectedValue;
                UpdatedRoom.Description = txtDescription.Text;
                UpdatedRoom.MaxCapacity = int.Parse(txtMaxCapacity.Text);
                UpdatedRoom.PricePerDay = decimal.Parse(txtPricePerDay.Text);
                UpdatedRoom.Status = (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
