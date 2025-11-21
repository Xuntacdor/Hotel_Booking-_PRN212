using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class RoomDAO
    {
        private readonly FuminiHotelProjectPrn212Context _context = new();

        public List<Room> GetAllRooms()
        {
            return _context.Rooms
                .Include(r => r.RoomType)
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        public Room GetRoomById(int roomId)
        {
            return _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefault(r => r.RoomId == roomId);
        }

        public void AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
        }

        public void UpdateRoom(Room room)
        {
            try
            {
                using (var _context = new FuminiHotelProjectPrn212Context())
                {
                    var existing = _context.Rooms.Find(room.RoomId);
                    if (existing != null)
                    {
                        _context.Entry(existing).CurrentValues.SetValues(room);
                        _context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi cập nhật khách hàng: " + ex.InnerException?.Message);
            }
        }
 

        public void DeleteRoom(int roomId)
        {
            var room = _context.Rooms.Find(roomId);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
            }
        }

        public List<Room> SearchAvailableRooms(DateTime startDate, DateTime endDate)
        {
            var bookedRoomIds = _context.BookingDetails
                .Where(bd => !(endDate <= bd.StartDate || startDate >= bd.EndDate))
                .Select(bd => bd.RoomId)
                .Distinct()
                .ToList();

            return _context.Rooms
                .Where(r => !bookedRoomIds.Contains(r.RoomId) && r.Status == "Active")
                .Include(r => r.RoomType)
                .ToList();
        }

        public List<Room> SearchRooms(string keyword, string status)
        {
            var query = _context.Rooms
                .Include(r => r.RoomType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(r => r.RoomNumber.Contains(keyword) ||
                                       r.Description.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            return query.ToList();
        }
        public List<int?> GetBookedRoomIds(DateTime startDate, DateTime endDate)
        {
            return _context.BookingDetails
                .Where(bd => !(endDate <= bd.StartDate || startDate >= bd.EndDate))
                .Select(bd => bd.RoomId)
                .Distinct()
                .ToList();
        }
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
    }
}
