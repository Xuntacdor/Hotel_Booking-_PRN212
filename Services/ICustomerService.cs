using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICustomerService
    {
        Customer? GetCustomerById(int id);
        List<Customer> GetCustomers();
        Customer GetCustomerByEmail(string email);
        bool UpdateCustomer(Customer updatedCustomer);
        void Add(Customer NewCustomer);
        void Update(Customer customer);
        void Delete(int customerId);
    }
}
