using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FUMiniHotel_ProjectPRN212.Room
{
    public partial class RoomManagement : Page
    {
        private readonly RoomDAO _roomDao = new RoomDAO();
        private List<BusinessObjects.Room> _allRooms;
        private List<BusinessObjects.Room> _dateFilteredRooms;

        public RoomManagement()
        {
            InitializeComponent();
            LoadAllRooms();
        }

        private void LoadAllRooms()
        {
            _allRooms = _roomDao.GetAllRooms()
                        ?? new List<BusinessObjects.Room>();

            // No status override
            _dateFilteredRooms = _allRooms.ToList();

            ApplyStatusFilter();
        }

        private void ApplyStatusFilter()
        {
            try
            {
                if (_dateFilteredRooms == null || _allRooms == null)
                    return;

                var searchText = txtSearch.Text.ToLower();
                var status = (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

                var filtered = _dateFilteredRooms.Where(r => r != null &&
                    (string.IsNullOrWhiteSpace(searchText) ||
                        (r.RoomNumber?.ToLower().Contains(searchText) ?? false) ||
                        (r.Description?.ToLower().Contains(searchText) ?? false)) &&
                    (status == "Tất cả" || string.IsNullOrEmpty(status) ||
                        (r.Status != null && r.Status.Equals(status, StringComparison.OrdinalIgnoreCase)))
                ).ToList();

                dgRooms.ItemsSource = filtered;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void ApplyDateFilter()
        {
            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                _dateFilteredRooms = new List<BusinessObjects.Room>(_allRooms);
                ApplyStatusFilter();
                return;
            }

            var startDate = dpStartDate.SelectedDate.Value;
            var endDate = dpEndDate.SelectedDate.Value;

            // Chỉ tìm phòng trống theo date, KHÔNG đụng tới Status của phòng
            var bookedRoomIds = _roomDao.GetBookedRoomIds(startDate, endDate);

            _dateFilteredRooms = _allRooms
                .Where(r => !bookedRoomIds.Contains(r.RoomId))
                .ToList();

            ApplyStatusFilter();
        }

        private void SearchByDate_Click(object sender, RoutedEventArgs e)
        {
            ApplyDateFilter();
        }

        private void ClearDateFilter_Click(object sender, RoutedEventArgs e)
        {
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;

            _dateFilteredRooms = new List<BusinessObjects.Room>(_allRooms);

            ApplyStatusFilter();
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
            var button = sender as Button;
            var room = button.DataContext as BusinessObjects.Room; // Lấy đúng room của row

            if (room == null) return;

            var editWindow = new UpdateRoom(room.RoomId);

            if (editWindow.ShowDialog() == true)
            {
                _roomDao.UpdateRoom(editWindow.UpdatedRoom);
                LoadAllRooms();       // ✔ Reload UI
                dgRooms.Items.Refresh(); // ✔ Cập nhật lại DataGrid
            }
        }

        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
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
