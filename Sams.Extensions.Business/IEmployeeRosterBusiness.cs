using Sams.Extensions.Model;
using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Business
{
   public interface IEmployeeRosterBusiness
    {
        int SaveEmployeeData(EmployeeRosterHeader employee);
        string SaveEmployeeRosterData(EmployeeRosterDetails employeeRoster);
        Dictionary<string, DataTable> FetchEmployeeRosterData(ViewEmployeeRosterSearchModel viewEmployeeRosterSearchModel);
    }
}
