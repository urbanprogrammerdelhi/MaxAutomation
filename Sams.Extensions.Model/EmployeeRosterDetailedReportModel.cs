using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
    public class EmployeeRosterReportModel
    {
        public EmployeeRosterReports ReportType { get; set; }
        public string[] Header { get; set; }
        public Dictionary<string, DataTable> ReportData { get; set; }


    }
   
}
