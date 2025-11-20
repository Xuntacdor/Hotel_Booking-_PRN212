using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEmployeeService
    {
        void AddEmployee(Employee NewEmployee);
        void UpdateEmployee(Employee employee);
        void DeleteEmployee(int id);
        IQueryable<EmployeeDTO> GetEmployees();
        Employee? GetEmployeeById(int id);
        Employee? GetEmployeeByEmail(string email);
    }
}
