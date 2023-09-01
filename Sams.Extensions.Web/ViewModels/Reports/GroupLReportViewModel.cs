using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sams.Extensions.Web
{
    public class GroupLReportViewModel
    {
        public bool CanExportReport { get; set; }

        public IList<SelectListItem> Locations { get; set; }
        public IList<SelectListItem> Companies { get; set; }
        public IList<SelectListItem> Regions { get; set; }
        public string CurrentCompany { get; set; }
        public string CurrentLocation { get; set; }
        public string CurrentRegion { get; set; }
        public IList<SelectListItem> Reports { get; set; }
        public string CurrentReport { get; set; }
        public DateTime ReportDate { get; set; }
        public DataTable ReportData { get; set; }
        public string[] RequiredFields { get; set; }
        public string [] ComparisionFields { get; set; }
        public Dictionary<GroupLReports,string> ReportHeaders { get; set; }
        public string ReportHeader { get; set; }


    }
}