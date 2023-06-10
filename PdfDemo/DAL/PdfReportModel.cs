using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Pdfdemo
{

    public class PdfReportViewModel
    {
        public string Branch { get; set; }
        public string AuditDate { get; set; }
        public string Location { get; set; }
        public Reportheader Header { get; set; }
        public ILookup<string, ReportBody> MasterdetailList { get; set; }
        public ReportBody ColumnDetails { get; set; }


    }
  
    public class Reportheader
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string FOName { get; set; }


    }
    public class ReportBody
    {
        [Display(Name = "S.No.")]
        public int ChecklistId { get; set; }
        public string MainHeader { get; set; }
        [Display(Name = "Question")]
        public string SubHeader { get; set; }
        public int ImageAutoId { get; set; }
        public string BranchCode { get; set; }
        [Display(Name = "Response")]
        public string Text { get; set; }
        [Display(Name = "Comments")]

        public string Remarks { get; set; }
        public int HeaderIndex { get; set; }
        public string Photo { get; set; }


    }
    public class BranchDetails
    {
        public int SerialNumber { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string AuditTime { get; set; }

    }
 
}