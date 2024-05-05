using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Utility
{
    public class ConfigurationFields
    {
        #region APS Roster Employee functionality
        public static readonly string RosterEmployeesClassRequiredFields = ConfigurationManager.AppSettings["RosterEmployeesClassRequiredFields"];
        public static readonly int FromYear = ConfigurationManager.AppSettings["FromYear"].ParseInt();
        public static readonly int ToYear = ConfigurationManager.AppSettings["ToYear"].ParseInt();
        public static readonly string[] Months = Enum.GetNames(typeof(Months));
        public static readonly string[] RequiredExtensionsForEmployeeRoster = ConfigurationManager.AppSettings["RosterEmployeeRequiredExtensions"].Split(',');
        public static readonly string[] PhotoDashboardRequiredFields = ConfigurationManager.AppSettings["PhotoDashboardRequiredFields"].Split(',');
        public static readonly string[] Floors = Enum.GetNames(typeof(Floors));
        public static readonly string DefaultHeaderFormat = "<th style='background-color: #B8DBFD;border: 1px solid #ccc'>{0}</th>";
        public static readonly string BeginTableTag = "<table border='1' cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-family: Arial; font-size: 10pt;'>";
        public static readonly string EndTableTag = "</table>";
        public static readonly string BeginRowTag = "<tr>";
        public static readonly string EndRowTag = "</tr>";
        public static readonly string HtmlBeginTag = "<html>";
        public static readonly string HtmlEndTag = "</html>";
        public static readonly string BodyBeginTag = "<body>";
        public static readonly string BodyEndTag = "</body>";
        public static readonly string HeadBeginTag = "<head>";
        public static readonly string HeadEndTag = "</head>";
        public static readonly string ColumnFormat = "<td style='border: 1px solid #ccc;padding:5px;'>{0}</td>";
        public static readonly string ImageFormat = "<img height='75px' width='75px' src=\'data:image/jpg;base64,@Image ></img>";
        public static readonly string DefaultCellBeginFormat = "<td style='border: 1px solid #ccc;padding:5px;'>";
        public static readonly string DefaultCellEndingTag = "</td>";
        public static readonly string ImageStartingTag = "<img height='75px' width='75px' src=\'data:image/jpg;base64,";
        public static string[] CheckListReportRequiredFields = ConfigurationManager.AppSettings["ChecListReportRequiredFields"].Split(',');
        public static string[] ChecListReportComparisionFields = ConfigurationManager.AppSettings["ChecListReportComparisionFields"].Split(',');
        public static string[] RegisterReportRequiredFields = ConfigurationManager.AppSettings["RegisterReportRequiredFields"].Split(',');
        public static string[] RegisterReportComparisionFields = ConfigurationManager.AppSettings["RegisterReportComparisionFields"].Split(',');

        public static string[] FsaReportHeaderRequiredFields = ConfigurationManager.AppSettings["FsaReportHeaderRequiredFields"].Split(',');
        public static string[] FsaReportHeaderComparisionFields = ConfigurationManager.AppSettings["FsaReportHeaderComparisionFields"].Split(',');

        public static string[] FsaReportDetailsRequiredFields = ConfigurationManager.AppSettings["FsaReportDetailsRequiredFields"].Split(',');
        public static string[] FsaReportDetailsComparisionFields = ConfigurationManager.AppSettings["FsaReportDetailsComparisionFields"].Split(',');

        public static string[] FsaReportFooterRequiredFields = ConfigurationManager.AppSettings["FsaReportFooterRequiredFields"].Split(',');
        public static string[] FsaReportFooterComparisionFields = ConfigurationManager.AppSettings["FsaReportFooterComparisionFields"].Split(',');




        #endregion
    }
}
