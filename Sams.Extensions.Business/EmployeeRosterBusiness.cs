using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sams.Extensions.Dal;
using Sams.Extensions.Model;

namespace Sams.Extensions.Business
{
    public class EmployeeRosterBusiness : IEmployeeRosterBusiness
    {
        private readonly IEmployeeRoster _employeeRoster;
        public EmployeeRosterBusiness(IEmployeeRoster employeeRoster)
        {
            _employeeRoster = employeeRoster;
        }
        public Dictionary<string, DataTable> FetchEmployeeRosterData(ViewEmployeeRosterSearchModel viewEmployeeRosterSearchModel)
        {
            return _employeeRoster.FetchEmployeeRosterData(viewEmployeeRosterSearchModel);
        }

        public int SaveEmployeeData(EmployeeRosterHeader employee)
        {
            return _employeeRoster.SaveEmployeeData(employee);
        }

        public string SaveEmployeeRosterData(EmployeeRosterDetails employeeRoster)
        {
            return _employeeRoster.SaveEmployeeRosterData(employeeRoster);
        }
    }
}
