using BusinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public void Add(Customer NewCustomer) => CustomerDAO.Add(NewCustomer);

        public void Delete(int customerId) => CustomerDAO.Delete(customerId);


        public Customer GetCustomerByEmail(string email) => CustomerDAO.GetCustomerByEmail(email);

        public Customer? GetCustomerById(int id) => CustomerDAO.GetCustomerById(id);

        public List<Customer> GetCustomers() => CustomerDAO.GetCustomers();

        public void Update(Customer customer) => CustomerDAO.Update(customer);


        public bool UpdateCustomer(Customer updatedCustomer)=> CustomerDAO.UpdateCustomer(updatedCustomer);
    }
}
