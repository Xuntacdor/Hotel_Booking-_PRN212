using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class EmployeeRoleDAO
    {
        public static List<EmployeeRole> GetEmployeeRoles()
        {
            FuminiHotelProjectPrn212Context context = new FuminiHotelProjectPrn212Context();
            return context.EmployeeRoles.ToList();
        }
    }
}
