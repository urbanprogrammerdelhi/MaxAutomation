using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
    public class  FsaReportHeader
    {
    
        public string ClientDetails { get; set; }
        public string OfficeAddress { get; set; }
    }

    public class FsaReportDetails
    {

        public string ShowID { get; set; }
        public string Category { get; set; }
        public string Audit { get; set; }
        public string RequiredAction { get; set; }
        public string Pictures { get; set; }
    }

    public class FsaReportFooter
    {

        public string ChecklistID { get; set; }
        public string Category { get; set; }
        public string Qty { get; set; }
       
    }
    public class CustomReportDetails
    {
        public IEnumerable<string> HeaderRequiredFields { get; set; }
        public IEnumerable<string> HeaderCompareFields { get; set; }

        public IEnumerable<string> DetailRequiredFields { get; set; }
        public IEnumerable<string> DetailCompareFields { get; set; }

        public IEnumerable<string> FooterRequiredFields { get; set; }
        public IEnumerable<string> FooterCompareFields { get; set; }
    }
}
