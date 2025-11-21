using BusinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class EmployeeRoleRepository : IEmployeeRoleRepository
    {
        public List<EmployeeRole> GetEmployeeRoles() => EmployeeRoleDAO.GetEmployeeRoles();
    }
}
