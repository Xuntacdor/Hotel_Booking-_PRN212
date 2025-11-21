using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
namespace DataAccessLayer
{
    public class CustomerDAO
    {

        public static Customer? GetCustomerById(int id)
        {
            using (var _context = new FuminiHotelProjectPrn212Context())
            {
                return _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            }
        }
        public static List<Customer> GetCustomers()
        {
            using (var _context = new FuminiHotelProjectPrn212Context()) {
                return _context.Customers.ToList();
            }
        }
        public static Customer GetCustomerByEmail(string email)
        {
            using (var _context = new FuminiHotelProjectPrn212Context())
            {
                return _context.Customers.FirstOrDefault(c => c.Email == email);
            }
        }
        public static bool UpdateCustomer(Customer updatedCustomer)
        {
            bool isUpdated = false;

            try
            {
                using (var context = new FuminiHotelProjectPrn212Context())
                {
                    var customer = context.Customers.FirstOrDefault(c => c.CustomerId == updatedCustomer.CustomerId);
                    if (customer != null)
                    {
                        customer.FullName = updatedCustomer.FullName;
                        customer.Telephone = updatedCustomer.Telephone;
                        customer.Birthday = updatedCustomer.Birthday;

                        context.SaveChanges();
                        isUpdated = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
            }

            return isUpdated;
        }
    
     public static void Add(Customer NewCustomer)
        {
            try
            {
                using (var _context = new FuminiHotelProjectPrn212Context())
                {
                    _context.Customers.Add(NewCustomer);
                    _context.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi thêm khách hàng: " + ex.InnerException?.Message);
            }
        }

        // Cập nhật thông tin khách hàng
        public static void Update(Customer customer)
        {
            try
            {
                using (var _context = new FuminiHotelProjectPrn212Context())
                {
                    var existing = _context.Customers.Find(customer.CustomerId);
                    if (existing != null)
                    {
                        _context.Entry(existing).CurrentValues.SetValues(customer);
                        _context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi cập nhật khách hàng: " + ex.InnerException?.Message);
            }
        }

        // Xóa khách hàng
        public static void Delete(int customerId)
        {
            try
            {
                using (var _context = new FuminiHotelProjectPrn212Context())
                {
                    var customer = _context.Customers.Find(customerId);
                    if (customer != null)
                    {
                        // Kiểm tra ràng buộc khóa ngoại trước khi xóa
                        if (_context.Bookings.Any(b => b.CustomerId == customerId) ||
                            _context.Invoices.Any(i => i.CustomerId == customerId))
                        {
                            throw new Exception("Không thể xóa vì khách hàng có dữ liệu liên quan");
                        }

                        _context.Customers.Remove(customer);
                        _context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi xóa khách hàng: " + ex.InnerException?.Message);
            }
        }

    }
}
