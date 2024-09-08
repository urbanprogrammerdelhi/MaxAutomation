using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Utility
{
    public class CommonUtility
    {


        public static readonly Dictionary<EmployeeRosterReports, string> EmployeeeRosterReports = new Dictionary<EmployeeRosterReports, string>
        {
            { EmployeeRosterReports.EmployeeRosterConsolidated,"Consolidated Report" },
            {EmployeeRosterReports.EmployeeRosterDetailed,"Detailed Report" }

        };
        public static readonly string rosterDataInitials = "S.No,Employee Code,Employee Name,Designation";

        //public static string GetPageAbsolutePath
        //{
        //    get
        //    {
        //        return System.Web.HttpContext.Current.Request.Url.AbsolutePath;
        //    }
        //}
        
        public static string GenerateDayHeader(int month, int year)
        {
            try
            {
                var daysInMonth = DateTime.DaysInMonth(year, month);
                return string.Join(",", Enumerable.Range(1, daysInMonth).Select(n => n.ParseToText()));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static string GenerateRosterDataHeader(int month, int year)
        {
            try
            {
                var daysInMonth = DateTime.DaysInMonth(year, month);

                return rosterDataInitials + "," + string.Join(",", Enumerable.Repeat<string>("Shift,Post", daysInMonth));
            }
            catch (Exception ex) { throw; }
        }

        public static List<string> EmployeeRosterConsolidatedReportHeader(int year,int month)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            List<string> ReportHeader = new List<string>();
            for (int i = 1; i <= daysInMonth; i++)
            {
                ReportHeader.Add(new DateTime(year, month, i).ToString("dd"));
            }
            return ReportHeader;
        }
        public static List<string[]> EmployeeRosterConsolidatedRequiredFields(int year, int month)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            List<string[]> RequiredFields = new List<string[]>();
            for (int i = 1; i <= daysInMonth; i++)
            {
                var field = new DateTime(year, month, i).ToString("dd-MMM-yyyy");
                RequiredFields.Add(new string[] { $"{field}_Shift", $"{field}_Post" });
            }
            return RequiredFields;
        }
    }
    public enum Months
    {
        Jan = 1,
        Feb,
        Mar,
        Apr,
        May,
        Jun,
        Jul,
        Aug,
        Sep,
        Oct,
        Nov,
        Dec,
    }
    public enum Floors
    {
        Floor1 = 1,
        Floor2,
        Floor3,
        Floor4,
        Floor5,
        Floor6,
    }

}
