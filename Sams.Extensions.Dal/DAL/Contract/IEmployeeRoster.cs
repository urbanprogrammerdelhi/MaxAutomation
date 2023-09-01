using Sams.Extensions.Model;
using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Dal
{
    public interface IEmployeeRoster
    {
        int SaveEmployeeData(EmployeeRosterHeader employee);
        string SaveEmployeeRosterData(EmployeeRosterDetails employeeRoster);
        Dictionary<string, DataTable> FetchEmployeeRosterData(ViewEmployeeRosterSearchModel employeeRosterSearchModel);
    }
}
