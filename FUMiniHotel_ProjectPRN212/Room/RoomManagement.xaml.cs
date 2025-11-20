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

namespace FUMiniHotel_ProjectPRN212.Room
{
    /// <summary>
    /// Interaction logic for RoomManagement.xaml
    /// </summary>
    public partial class RoomManagement : Page
    {
        private readonly RoomDAO _roomDao = new RoomDAO();
        private List<BusinessObjects.Room> _allRooms;
        private List<BusinessObjects.Room> _dateFilteredRooms;
        private bool _isAdmin = true; // Mặc định là Admin, sẽ được set từ parent window
        
        public RoomManagement(bool isAdmin = true)
        {
            _isAdmin = isAdmin;
            InitializeComponent();
            SetupUIForUserRole();
            
            // Đợi Page load xong mới load dữ liệu
            this.Loaded += RoomManagement_Loaded;
        }

        private void RoomManagement_Loaded(object sender, RoutedEventArgs e)
        {
            // Đảm bảo controls đã được khởi tạo trước khi load dữ liệu
            if (dgRooms != null)
            {
                LoadAllRooms();
            }
        }

        private void SetupUIForUserRole()
        {
            // Ẩn nút "Thêm phòng mới" và cột "Hành động" nếu không phải Admin
            if (!_isAdmin)
            {
                // Ẩn nút "Thêm phòng mới"
                if (AddButton != null)
                {
                    AddButton.Visibility = Visibility.Collapsed;
                }

                // Ẩn cột "Hành động" trong DataGrid sau khi load xong
                this.Loaded += (s, e) =>
                {
                    if (dgRooms != null && dgRooms.Columns.Count > 0)
                    {
                        var actionColumn = dgRooms.Columns.LastOrDefault() as DataGridTemplateColumn;
                        if (actionColumn != null && actionColumn.Header?.ToString() == "Hành động")
                        {
                            actionColumn.Visibility = Visibility.Collapsed;
                        }
                    }
                };
            }
        }

        private void LoadAllRooms()
        {
            try
            {
                _allRooms = _roomDao.GetAllRooms() ?? new List<BusinessObjects.Room>();
                _dateFilteredRooms = new List<BusinessObjects.Room>(_allRooms);
                UpdateRoomStatus();
                ApplyStatusFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                _allRooms = new List<BusinessObjects.Room>();
                _dateFilteredRooms = new List<BusinessObjects.Room>();
            }
        }

        private void UpdateRoomStatus()
        {
            if (_allRooms == null || _allRooms.Count == 0)
                return;

            try
            {
                var currentDate = DateTime.Today;
                var bookedRoomIds = _roomDao.GetBookedRoomIds(currentDate, currentDate.AddDays(1)) ?? new List<int?>();

                foreach (var room in _allRooms)
                {
                    if (room != null)
                    {
                        room.Status = bookedRoomIds.Contains(room.RoomId) ? "Hết phòng" : "Còn phòng";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật trạng thái phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyStatusFilter()
        {
            try
            {
                // Kiểm tra dgRooms đã được khởi tạo chưa
                if (dgRooms == null)
                {
                    return; // Chưa load xong, bỏ qua
                }

                if (_dateFilteredRooms == null)
                {
                    _dateFilteredRooms = new List<BusinessObjects.Room>();
                }

                var searchText = txtSearch?.Text?.ToLower() ?? string.Empty;
                var status = (cbStatus?.SelectedItem as ComboBoxItem)?.Content?.ToString();

                var filtered = _dateFilteredRooms.Where(r => r != null &&
            (string.IsNullOrWhiteSpace(searchText) ||
             (r.RoomNumber != null && r.RoomNumber.ToLower().Contains(searchText)) ||
             (r.Description != null && r.Description.ToLower().Contains(searchText))) &&
            (status == "Tất cả" || string.IsNullOrEmpty(status) ||
             (r.Status != null && r.Status.Equals(status, StringComparison.OrdinalIgnoreCase)))
        ).ToList();

                dgRooms.ItemsSource = filtered;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyDateFilter()
        {
            try
            {
                if (_allRooms == null)
                {
                    _allRooms = new List<BusinessObjects.Room>();
                }

                if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
                {
                    _dateFilteredRooms = new List<BusinessObjects.Room>(_allRooms);
                    ApplyStatusFilter();
                    return;
                }

                var startDate = dpStartDate.SelectedDate.Value;
                var endDate = dpEndDate.SelectedDate.Value;

                var bookedRoomIds = _roomDao.GetBookedRoomIds(startDate, endDate) ?? new List<int?>();
                _dateFilteredRooms = _allRooms.Where(r => r != null && !bookedRoomIds.Contains(r.RoomId)).ToList();
                
                foreach (var room in _dateFilteredRooms)
                {
                    if (room != null)
                    {
                        room.Status = "Còn phòng";
                    }
                }

                ApplyStatusFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc theo ngày: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchByDate_Click(object sender, RoutedEventArgs e)
        {
            ApplyDateFilter();
        }

        private void ClearDateFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dpStartDate.SelectedDate = null;
                dpEndDate.SelectedDate = null;
                
                if (_allRooms == null)
                {
                    _allRooms = new List<BusinessObjects.Room>();
                }
                
                _dateFilteredRooms = new List<BusinessObjects.Room>(_allRooms);
                UpdateRoomStatus();
                ApplyStatusFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa bộ lọc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyStatusFilter();
        }

        private void cbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyStatusFilter();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddRoom();
            if (addWindow.ShowDialog() == true)
            {
                _roomDao.AddRoom(addWindow.NewRoom);
                LoadAllRooms();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgRooms == null) return;
            
            var selected = dgRooms.SelectedItem as BusinessObjects.Room;
            if (selected != null)
            {
                var editWindow = new UpdateRoom(selected.RoomId);
                if (editWindow.ShowDialog() == true)
                {
                    _roomDao.UpdateRoom(editWindow.UpdatedRoom);
                    LoadAllRooms();
                }
            }
        }
        
        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            if (dgRooms == null) return;
            
            var selected = dgRooms.SelectedItem as BusinessObjects.Room;
            if (selected != null)
            {
                if (MessageBox.Show("Xác nhận xóa phòng?", "Xác nhận",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _roomDao.DeleteRoom(selected.RoomId);
                    LoadAllRooms();
                }
            }
        }
 
    }
}
