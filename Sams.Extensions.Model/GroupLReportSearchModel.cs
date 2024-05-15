using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
   public class GroupLReportSearchModel
   {
        public string CurrentRegion { get; set; }
        public string LocationAutoId { get; set; }
        public string ReportDate { get; set; }
        public string EmployeeNumber { get; set; }
        public GroupLReports CurrentReport { get; set; }
        public string ClientCode { get; set; }
        public string SiteCode { get; set; }

    }
    public class FsaSearchModel
    {
        public string ClientCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

    }
}
