using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository customerRepository;
        public CustomerService()
        {
            customerRepository = new CustomerRepository();
        }

        public void Add(Customer NewCustomer) => customerRepository.Add(NewCustomer);

        public void Delete(int customerId) => customerRepository.Delete(customerId);

        public Customer GetCustomerByEmail(string email) => customerRepository.GetCustomerByEmail(email);

        public Customer? GetCustomerById(int id) => customerRepository.GetCustomerById(id);

        public List<Customer> GetCustomers() => customerRepository.GetCustomers();

        public void Update(Customer customer) => customerRepository.Update(customer);

        public bool UpdateCustomer(Customer updatedCustomer)=> customerRepository.UpdateCustomer(updatedCustomer);
    }
}
