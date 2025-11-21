using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class RoomTypeDAO
    {
        private readonly FuminiHotelProjectPrn212Context _context = new();

        // Lấy tất cả loại phòng
        public List<RoomType> GetAllRoomTypes()
        {
            return _context.RoomTypes
                .OrderBy(rt => rt.TypeName)
                .ToList();
        }

        // Lấy loại phòng theo ID
        public RoomType GetRoomTypeById(int roomTypeId)
        {
            return _context.RoomTypes
                .FirstOrDefault(rt => rt.RoomTypeId == roomTypeId);
        }

        // Thêm loại phòng mới
        public void AddRoomType(RoomType roomType)
        {
            try
            {
                _context.RoomTypes.Add(roomType);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi thêm loại phòng: " + ex.InnerException?.Message);
            }
        }

        // Cập nhật loại phòng
        public void UpdateRoomType(RoomType roomType)
        {
            try
            {
                var existing = _context.RoomTypes.Find(roomType.RoomTypeId);
                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(roomType);
                    _context.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi cập nhật loại phòng: " + ex.InnerException?.Message);
            }
        }

        // Xóa loại phòng
        public void DeleteRoomType(int roomTypeId)
        {
            try
            {
                var roomType = _context.RoomTypes.Find(roomTypeId);
                if (roomType != null)
                {
                    // Kiểm tra ràng buộc khóa ngoại trước khi xóa
                    if (_context.Rooms.Any(r => r.RoomTypeId == roomTypeId))
                    {
                        throw new Exception("Không thể xóa vì có phòng thuộc loại này");
                    }

                    _context.RoomTypes.Remove(roomType);
                    _context.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi xóa loại phòng: " + ex.InnerException?.Message);
            }
        }

        // Tìm kiếm loại phòng theo tên
        public List<RoomType> SearchRoomTypes(string keyword)
        {
            return _context.RoomTypes
                .Where(rt => rt.TypeName.Contains(keyword) ||
                             rt.Description.Contains(keyword))
                .ToList();
        }
    }
}
