using BusinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FUMiniHotel_ProjectPRN212.Pages
{
    public partial class CustomerBookingPage : Page
    {
        private readonly BookingDAO _bookingDao = new BookingDAO();
        private readonly RoomDAO _roomDao = new RoomDAO();
        private readonly ServiceDAO _serviceDao = new ServiceDAO();

        private List<BusinessObjects.Room> _availableRooms = new List<BusinessObjects.Room>();
        private List<BookingDetail> _selectedRooms = new List<BookingDetail>();
        private List<BookingService> _selectedServices = new List<BookingService>();
        private List<BusinessObjects.Service> _availableServices = new List<BusinessObjects.Service>();

        public DateTime CheckInDate { get; set; } = DateTime.Today;
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        public CustomerBookingPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadAvailableServices();
        }

        private void LoadAvailableServices()
        {
            _availableServices = _serviceDao.GetAllServices();
            dgAvailableServices.ItemsSource = _availableServices;
        }

        private void BtnSearchRooms_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInDate >= CheckOutDate)
            {
                MessageBox.Show("Ngày trả phòng phải sau ngày nhận phòng", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _availableRooms = _roomDao.GetAvailableRooms(CheckInDate, CheckOutDate);
            dgAvailableRooms.ItemsSource = _availableRooms;
        }

        private void BtnAddRoom_Click(object sender, RoutedEventArgs e)
        {
            if (dgAvailableRooms.SelectedItem is BusinessObjects.Room selectedRoom)
            {
                // Check if room is already selected
                if (_selectedRooms.Any(r => r.RoomId == selectedRoom.RoomId))
                {
                    MessageBox.Show("Phòng này đã được chọn", "Thông báo",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                int days = (CheckOutDate - CheckInDate).Days;
                _selectedRooms.Add(new BookingDetail
                {
                    RoomId = selectedRoom.RoomId,
                    Room = selectedRoom,
                    StartDate = CheckInDate,
                    EndDate = CheckOutDate,
                    ActualPrice = selectedRoom.PricePerDay,
                });

                dgSelectedRooms.ItemsSource = _selectedRooms.ToList();
                CalculateTotal();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn phòng từ danh sách", "Thông báo",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CalculateTotal()
        {
            decimal roomsTotal = _selectedRooms.Sum(r =>
        r.ActualPrice * (decimal)(r.EndDate - r.StartDate).TotalDays);

            // Calculate services total
            decimal servicesTotal = _selectedServices.Sum(s => s.TotalPrice);

            // Display combined total
            txtTotalPrice.Text = $"{roomsTotal + servicesTotal:N0} VND";
        }

        private void BtnAddService_Click(object sender, RoutedEventArgs e)
        {
            if (dgAvailableServices.SelectedItem is BusinessObjects.Service selectedService)
            {
                if (int.TryParse(txtServiceQuantity.Text, out int quantity) && quantity > 0)
                {
                    _selectedServices.Add(new BookingService
                    {
                        ServiceId = selectedService.ServiceId,
                        Service = selectedService,
                        Quantity = quantity,
                        TotalPrice = selectedService.Price * quantity
                    });

                    dgSelectedServices.ItemsSource = _selectedServices.ToList();
                    CalculateTotal();
                }
                else
                {
                    MessageBox.Show("Số lượng không hợp lệ", "Lỗi",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnRemoveService_Click(object sender, RoutedEventArgs e)
        {
            if (dgSelectedServices.SelectedItem is BookingService selectedService)
            {
                _selectedServices.Remove(selectedService);
                dgSelectedServices.ItemsSource = _selectedServices.ToList();
                CalculateTotal();
            }
        }

        private void BtnBookNow_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRooms.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một phòng", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get customer ID from current session
            int customerId = GetCurrentCustomerId();

            if (customerId == 0)
            {
                MessageBox.Show("Vui lòng đăng nhập để đặt phòng", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Create booking object with basic info
                var booking = new BusinessObjects.Booking
                {
                    CustomerId = customerId,
                    BookingDate = DateTime.Now,
                    Status = "Confirmed" // Matching your DAO method
                };

                // Prepare room bookings (without TotalPrice)
                var roomBookings = _selectedRooms.Select(r => new BookingDetail
                {
                    RoomId = r.RoomId,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    ActualPrice = r.ActualPrice
                }).ToList();

                // Call DAO method that handles the total calculation
                var result = _bookingDao.CreateBooking(booking, roomBookings, _selectedServices);

                MessageBox.Show($"Đặt phòng thành công! Mã đặt phòng: {result.BookingId}", "Thành công",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                // Reset form
                _selectedRooms.Clear();
                _selectedServices.Clear();
                dgSelectedRooms.ItemsSource = null;
                dgSelectedServices.ItemsSource = null;
                txtTotalPrice.Text = "0 VND";
                _availableRooms.Clear();
                dgAvailableRooms.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đặt phòng: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetCurrentCustomerId()
        {
            // Implement logic to get current logged in customer ID
            // This would typically come from your authentication system
            return 1; // Placeholder - replace with actual implementation
        }
    }
}