using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        public EmployeeService()
        {
            employeeRepository = new EmployeeRepository();
        }

        public void AddEmployee(Employee NewEmployee) => employeeRepository.AddEmployee(NewEmployee);
 

        public void DeleteEmployee(int id) => employeeRepository.DeleteEmployee(id);
     

        public Employee? GetEmployeeById(int id) => employeeRepository.GetEmployeeById(id);

        public Employee? GetEmployeeByEmail(string email) => employeeRepository.GetEmployeeByEmail(email);

        public IQueryable<EmployeeDTO> GetEmployees() => employeeRepository.GetEmployees();


        public void UpdateEmployee(Employee employee) => employeeRepository.UpdateEmployee(employee);
     
    }
}
