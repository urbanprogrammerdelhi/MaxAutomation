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
    public class FsaReportData
    {
        public FsaReportHeader CurrentHeader => new FsaReportHeader();
        public FsaReportDetails CurrentDetails => new FsaReportDetails();
        public FsaReportFooter CurrentFooter => new FsaReportFooter();
        public List<FsaReportHeader> Header { get; set; }
        public List<FsaReportDetails> Details { get; set; }
        public List<FsaReportFooter> Footer { get; set; }

       
    }
}
