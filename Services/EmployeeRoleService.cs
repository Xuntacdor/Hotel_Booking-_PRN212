using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmployeeRoleService : IEmployeeRoleService
    {
        private readonly IEmployeeRoleRepository repository;
        public EmployeeRoleService()
        {
            repository = new EmployeeRoleRepository();
        }
        public List<EmployeeRole> GetEmployeeRoles() => repository.GetEmployeeRoles();

    }
}
