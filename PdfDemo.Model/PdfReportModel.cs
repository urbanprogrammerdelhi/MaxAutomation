
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace PdfDemo.Model
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
        [Display(Name = "Branch Code")]
        public string BranchCode { get; set; }
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }
        [Display(Name = "Field Officer Name")]
        public string FOName { get; set; }
        [Display(Name = "Audit Date")]
        public string AuditDate { get; set; }
        [Display(Name ="SPOC Name")]
        public string SpocName { get; set; }
        [Display(Name = "SPOC Number")]
        public string SpocNumber { get; set; }
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
        [Display(Name ="Sl No.")]
        public int SerialNumber { get; set; }
        [Display(Name = "Branch Code")]
        public string BranchCode { get; set; }
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }
        [Display(Name = "Audit Time")]
        public string AuditTime { get; set; }
        public string LocationCode { get; set; }

    }
 
}