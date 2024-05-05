using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
    public class GroupLReportDataSet
    {
       public GroupLReports CurrentReport { get; set; }
       public DataTable ReportData { get; set; }
        public string[] RequiredFields { get; set; }
        public string[] ComparisionFields { get; set; }
       
    }
}
