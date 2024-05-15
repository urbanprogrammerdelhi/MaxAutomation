using Sams.Extensions.Model;
using Sams.Extensions.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Sams.Extensions.Web
{
    public static class FsaConstants
    {

        private static readonly string[] FirstQuarter = new string[] {"1","2","3" };
        private static readonly string[] SecondQuarter = new string[] { "4", "5", "6" };
        private static readonly string[] ThirdQuarter = new string[] { "7", "8", "9" };
        private static readonly string[] FourthQuarter = new string[] { "10", "11", "12" };
        private static readonly Dictionary<string, string[]> QuarterDictionary = new Dictionary<string, string[]>
        {
            {"Jan-Mar",FirstQuarter },
            {"April-June",SecondQuarter },
            {"July-Sep",ThirdQuarter },
            {"Oct-Dec",FourthQuarter },
            
        };
        public static void  CalculateFromToDate(string year,string quarter,out DateTime? fromDate,out DateTime? toDate)
        {
            
            fromDate = toDate = null;
            if (string.IsNullOrEmpty(year)) return;
            if (string.IsNullOrEmpty(quarter)) return;
            var quarterList = QuarterDictionary[quarter];
            var daysInAMonth= DateTime.DaysInMonth(year.ParseInt(), quarterList[2].ParseInt());
            fromDate = new DateTime(year.ParseInt(), quarterList[0].ParseInt(), 1, 0, 0, 0);
            toDate = new DateTime(year.ParseInt(), quarterList[2].ParseInt(), daysInAMonth, 23, 59, 59);            
        }
         
        public static List<SelectListItem> Years
        {
            get
            {
                var years = new List<SelectListItem>();
                for(int i=2024;i<=DateTime.Now.Year;i++)
                {
                    years.Add(new SelectListItem {Text=i.ParseToText(),Value=i.ParseToText() });
                }
                return years;
            }
        }
        public static List<SelectListItem> Quarters
        {
            get
            {
                var quarterList = QuarterDictionary.Keys.ToList();
                var quarters = new List<SelectListItem>();
                for (int i = 0; i < quarterList.Count; i++)
                {
                    quarters.Add(new SelectListItem { Text = quarterList[i].ParseToText(), Value = quarterList[i].ParseToText() });
                }
                return quarters;
            }
        }

    }
    public class MaxAuditViewModel
    {
        public IList<SelectListItem> ChecklistTypes { get; set; }
        public IList<SelectListItem> Locations { get; set; }
        public IList<SelectListItem> Companies { get; set; }
        public string CurrentCompany { get; set; }
        public string CurrentLocation { get; set; }
        public IList<BranchDetails> BranchDetails { get; set; }
        public BranchDetails CurrentBranch { get; set; }
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
        public static MaxAuditViewModel DefaultInstance
        {
            get
            {
                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                return new MaxAuditViewModel
                {
                    BranchDetails = new List<BranchDetails>(),
                    Companies = new List<SelectListItem>(),
                    CurrentBranch = new BranchDetails(),
                    CurrentCompany = "Max",
                    CurrentLocation = string.Empty,
                    Locations = new List<SelectListItem>(),
                    FromDate= currentDate.AddDays(Convert.ToInt32(ConfigurationManager.AppSettings["FromDays"])),
                    ToDate = currentDate,
                    ChecklistTypes=new List<SelectListItem>(),
                    SelectedChecklistType=string.Empty

                };
            }
        }
        public string SelectedChecklistType { get; set; }
    }

    public class FsaViewModel
    {
        public bool CanExportReport { get { return (ReportData != null && ReportData.Header != null && ReportData.Header.Count > 0 && ReportData.Details != null && ReportData.Details.Count > 0 && ReportData.Footer != null && ReportData.Footer.Count > 0);  } }
        public FsaReportData ReportData { get; set; }

        public IList<SelectListItem> Locations { get; set; }
        public IList<SelectListItem> Clients { get; set; }
        public IList<SelectListItem> Years { get; set; }
        public IList<SelectListItem> Quarters { get; set; }
        public string CurrentLocation { get; set; }
        public string CurrentClient { get; set; }
        public string CurrentYear { get; set; }
        public string CurrentQuarter { get; set; }


     
        public static FsaViewModel DefaultInstance
        {
            get
            {
                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                return new FsaViewModel
                {
                    ReportData = new FsaReportData(),
                    CurrentLocation = string.Empty,
                    Locations = new List<SelectListItem>(),
                    Clients= new List<SelectListItem>(),
                    CurrentClient=string.Empty,
                    CurrentQuarter= string.Empty,
                    CurrentYear=DateTime.Now.Year.ToString(),
                    Quarters= FsaConstants.Quarters,
                    Years= FsaConstants.Years

                };
            }
        }
    }


}