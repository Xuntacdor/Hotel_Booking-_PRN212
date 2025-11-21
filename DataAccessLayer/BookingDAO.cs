using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class BookingDAO
    {
        private readonly FuminiHotelProjectPrn212Context _context = new();


        public List<BookingService> GetBookingServices(int bookingId)
        {

                return _context.BookingServices
                    .Include(bs => bs.Service)
                    .Where(bs => bs.BookingId == bookingId)
                    .ToList();

        }
        public List<BookingDetail> GetBookingDetails(int bookingId)
        {

                return _context.BookingDetails
                    .Include(bd => bd.Room)
                    .ThenInclude(r => r.RoomType)
                    .Where(bd => bd.BookingId == bookingId)
                    .ToList();
            
        }
        public List<Booking> GetAllBookings()
        {
            return _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }
        public List<Booking> GetAllBookingsWithDetails()
        {
            return _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Room)
                        .ThenInclude(r => r.RoomType)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }
        public bool IsRoomAvailable(int roomId, DateTime startDate, DateTime endDate)
        {
            using (var context = new FuminiHotelProjectPrn212Context())
            {
                return !context.BookingDetails.Any(bd =>
                    bd.RoomId == roomId &&
                    !(endDate <= bd.StartDate || startDate >= bd.EndDate));
            }
        }

        // Get available rooms by date range and type
        public List<Room> GetAvailableRooms(DateTime startDate, DateTime endDate, int? roomTypeId = null)
        {
            using (var context = new FuminiHotelProjectPrn212Context())
            {
                var query = context.Rooms
                    .Include(r => r.RoomType)
                    .Where(r => r.Status == "Available");

                if (roomTypeId.HasValue)
                {
                    query = query.Where(r => r.RoomTypeId == roomTypeId);
                }

                var bookedRoomIds = context.BookingDetails
                    .Where(bd => !(endDate <= bd.StartDate || startDate >= bd.EndDate))
                    .Select(bd => bd.RoomId)
                    .Distinct();

                return query.Where(r => !bookedRoomIds.Contains(r.RoomId)).ToList();
            }
        }

        // Create new booking
        public Booking CreateBooking(Booking booking, List<BookingDetail> roomBookings, List<BookingService> bookingServices)
        {
            // Validate input
            if (booking == null) throw new ArgumentNullException(nameof(booking));
            if (roomBookings == null || !roomBookings.Any())
                throw new ArgumentException("Ít nhất một phòng phải được chọn");

            using (var context = new FuminiHotelProjectPrn212Context())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. Kiểm tra phòng trống
                        foreach (var room in roomBookings)
                        {
                            if (context.BookingDetails.Any(b =>
                                b.RoomId == room.RoomId &&
                                b.StartDate < room.EndDate &&
                                b.EndDate > room.StartDate))
                            {
                                throw new Exception($"Phòng {room.RoomId} đã được đặt trong khoảng thời gian này");
                            }
                        }

                        // 2. Thêm booking trước để nhận BookingId
                        context.Bookings.Add(booking);
                        context.SaveChanges(); // Lúc này booking.BookingId đã được sinh tự động

                        // 3. Thêm room bookings
                        foreach (var room in roomBookings)
                        {
                            room.BookingId = booking.BookingId; // Sử dụng BookingId vừa sinh
                            context.BookingDetails.Add(room);
                        }

                        // 4. Thêm services (nếu có)
                        if (bookingServices != null && bookingServices.Any())
                        {
                            foreach (var service in bookingServices)
                            {
                                service.BookingId = booking.BookingId;
                                // KHÔNG gán service.ServiceId để DB tự sinh
                                context.BookingServices.Add(service);
                            }
                        }

                        context.SaveChanges();
                        transaction.Commit();
                        return booking;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Log error chi tiết hơn
                        Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
                        throw new Exception("Lỗi khi đặt phòng: " + ex.Message);
                    }
                }
            }
        }

        // Get customer bookings
        public List<Booking> GetCustomerBookings(int customerId)
        {
            using (var context = new FuminiHotelProjectPrn212Context())
            {
                return context.Bookings
                    .Include(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Room)
                        .ThenInclude(r => r.RoomType)
                    .Include(b => b.BookingServices)
                        .ThenInclude(bs => bs.Service)
                    .Where(b => b.CustomerId == customerId)
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();
            }
        }
    }
}
