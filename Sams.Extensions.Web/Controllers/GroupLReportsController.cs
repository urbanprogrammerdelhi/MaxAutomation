using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Sams.Extensions.Business;
using Sams.Extensions.Logger;
using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using iTextSharp.tool.xml;
using System.Text;
using Sams.Extensions.Dal;
using Sams.Extensions.Utility;
using Sams.Extensions.Model;
using SelectPdf;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Sams.Extensions.Web.Controllers
{
    [CustomAuthorizeAttribute]
    public class GroupLReportsController : Controller
    {
        private readonly IMasterBusiness _master;
        private readonly ILoggerManager _loggerManager;
        private readonly IGroupLReportBusiness _groupLReportBusiness;
        public GroupLReportsController(ILoggerManager loggerManager, IMasterBusiness master, IGroupLReportBusiness groupLReportBusiness)
        {
            _loggerManager = loggerManager;
            _master = master;
            _groupLReportBusiness = groupLReportBusiness;


        }
        // GET: Branch
        public ActionResult Index()
        {
            _loggerManager.LogError($"Logging Test");

            try
            {
                GroupLReportSearchModel searchModel = null;
                if (TempData["GroupLSearchData"]!=null)
                {
                    searchModel = (GroupLReportSearchModel)TempData["GroupLSearchData"];
                    TempData["GroupLSearchData"] = null;
                }
                if (TempData["Message"] != null)
                {
                    ViewBag.MessageInfo = (MessageInfo)TempData["Message"];
                }
                GroupLReportViewModel vm = new GroupLReportViewModel
                {
                    Companies = new List<SelectListItem>(),
                    CurrentCompany = (Session["UserInfo"] as Account).Company,
                    CurrentLocation = string.Empty,
                    CurrentRegion = string.Empty,
                    CurrentReport = string.Empty,
                    Locations = new List<SelectListItem>(),
                    Regions = new List<SelectListItem>(),
                    Reports = new List<SelectListItem>(),
                    ReportHeaders = new Dictionary<GroupLReports, string>(),
                    ReportHeader = "Reports",
                    ReportDate=DateTime.Now,
                    SiteCode=string.Empty,ClientCode=string.Empty,Sites=new List<SelectListItem>(),Clients=new List<SelectListItem>()
                };
               
                vm.ReportHeaders.Add(GroupLReports.PhotoDashboard, "Photo Dashboard Report");
                vm.ReportHeaders.Add(GroupLReports.RegisterDashboard, "Register Dashboard Report");
                vm.ReportHeaders.Add(GroupLReports.CheckListDashboard, "Checklist Dashboard Report");
                vm.ReportHeaders.Add(GroupLReports.FSAReport, "FSA Report");

                var reports = _master.FetchReports(vm.CurrentCompany);
                vm.Reports = reports.ToSelectList("ReportId", "ReportName");
                var companies = _master.FetchCompanyDetails();
                vm.Companies = companies.ToSelectList("CompanyCode", "CompanyDesc");
                if (!string.IsNullOrEmpty(vm.CurrentCompany))
                {
                    var regions = _master.FetchRegions(vm.CurrentCompany);
                    vm.Regions = regions.ToSelectList("RegionName", "RegionDescription");
                }
               
                if (searchModel!=null)
               {
                    vm.ReportDate = Convert.ToDateTime(searchModel.ReportDate);
                    vm.CurrentRegion = searchModel.CurrentRegion;
                    vm.CurrentLocation = searchModel.LocationAutoId;
                    var locations = _master.FetchLocations(vm.CurrentCompany, vm.CurrentRegion);
                    vm.Locations = locations.ToSelectList("LocationAutoId", "Locationdesc");
                    vm.CurrentReport = ((int)searchModel.CurrentReport).ParseToText();
                    vm.ClientCode = searchModel.ClientCode;
                    vm.SiteCode = searchModel.SiteCode;
                    if (!string.IsNullOrEmpty(vm.CurrentLocation))
                    {
                        var clients = _master.FetchClients(vm.CurrentLocation);
                        vm.Clients = clients.ToSelectList("ClientCode", "ClientName");
                    }
                    if (!string.IsNullOrEmpty(vm.ClientCode))
                    {
                        var sites = _master.FetchSites(vm.CurrentLocation, vm.ClientCode, vm.CurrentCompany);
                        vm.Sites = sites.ToSelectList("AsmtId", "AsmtName");
                    }
                    var reportData = _groupLReportBusiness.GenerateDashboard(searchModel);
                    if (reportData != null && reportData.Count>0)
                    {
                        vm.ReportData = reportData;
                        vm.CanExportReport = true;
                        vm.ReportHeader = vm.ReportHeaders[(GroupLReports)Enum.Parse(typeof(GroupLReports), vm.CurrentReport)];
                    }
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Exception in Index {ex}");
                return View("Error");
            }
        }
       
        [HttpPost]
        public ActionResult FetchLocations(string CompanyCode, string RegionCode)
        {
            var locations = _master.FetchLocations(CompanyCode, RegionCode);
            var locationList = locations.ToSelectList("LocationAutoId", "Locationdesc");
            return Json(locationList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FetchRegions(string CompanyCode)
        {
            var locations = _master.FetchRegions(CompanyCode);
            var locationList = locations.ToSelectList("RegionName", "RegionDescriptiononName");
            return Json(locationList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitRequest(string Submit)
        {
            try
            {


                switch (Submit)
                {
                    case "Export to Pdf":
                        return (Export());
                    case "Search":
                        GroupLReportSearchModel searchModel = new GroupLReportSearchModel
                        {
                            EmployeeNumber = "All",
                            LocationAutoId = Request.Form["CurrentLocation"].ParseToText(),
                            CurrentReport = (GroupLReports)Request.Form["CurrentReport"].ParseInt(),
                            ReportDate = Request.Form["ReportDate"].ParseToText(),
                            CurrentRegion = Request.Form["CurrentRegion"].ParseToText(),
                            ClientCode = Request.Form["ClientCode"].ParseToText(),
                            SiteCode = Request.Form["SiteCode"].ParseToText()


                        };
                        TempData["GroupLSearchData"] = searchModel;
                        return RedirectToAction("Index");
                }
                return View();
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Exception in Index {ex}");
                return View("Error");
            }
        }
      
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Export()
        {
            try
            {
                GroupLReportSearchModel searchModel = new GroupLReportSearchModel
                {
                    EmployeeNumber = "All",
                    LocationAutoId = Request.Form["CurrentLocation"].ParseToText(),
                    CurrentReport = (GroupLReports)Request.Form["CurrentReport"].ParseInt(),
                    ReportDate = Request.Form["ReportDate"].ParseToText(),
                    ClientCode = Request.Form["ClientCode"].ParseToText(),
                    SiteCode = Request.Form["SiteCode"].ParseToText()

                };
                SelectPdf.PdfDocument mainDocument = new SelectPdf.PdfDocument();
                #region Commented
                ////var report = Request.Form["CurrentReport"].ParseToText();
                ////var currentLocation = Request.Form["CurrentLocation"].ParseToText();
                ////var reportDate = Request.Form["ReportDate"].ParseToText();
                ////var htmlString = _groupLReportBusiness.GeneratePhotoDashboard(new GroupLReportSearchModel { EmployeeNumber = "", ReportDate = reportDate, LocationAutoId = "20546" });
                //var reports = _groupLReportBusiness.GenerateDashboardReport(new GroupLReportSearchModel { EmployeeNumber = "All", ReportDate = "07-07-2023 00:00", LocationAutoId = "1",CurrentReport=GroupLReports.CheckListDashboard });
                //var reports = _groupLReportBusiness.GenerateDashboardReport(new GroupLReportSearchModel { EmployeeNumber = "All", ReportDate = "07-06-2023 00:00", LocationAutoId = "1",CurrentReport=GroupLReports.RegisterDashboard });
                #endregion
                List<string> reports;
                if (searchModel.CurrentReport == GroupLReports.FSAReport)
                {
                    reports = _groupLReportBusiness.GenerateFsaReportDetails(searchModel);
                }
                else
                {
                    reports = _groupLReportBusiness.GenerateDashboardReport(searchModel);
                }
                if (reports == null || reports.Count <= 0)
                {
                    TempData["Message"] = new MessageInfo { HasIssue = false, Message = "No records found" };
                    return RedirectToAction("Index");
                }
                int counter = 1;
                foreach (var htmlString in reports)
                {
                    HtmlToPdf converter = new HtmlToPdf();

                    // set converter options
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                    converter.Options.WebPageWidth = 1024;
                    converter.Options.WebPageHeight = 768;

                    // create a new pdf document converting an url
                    //= converter.ConvertHtmlString(htmlString);
                    mainDocument.Append(converter.ConvertHtmlString(htmlString));

                    // close pdf document


                    // return resulted pdf document

                    counter++;
                }
                var pdfBytes = mainDocument.Save();
                mainDocument.Close();
                FileResult fileResult = new FileContentResult(pdfBytes, "application/pdf");
                fileResult.FileDownloadName = $"Document_Part{counter}.pdf";
                fileResult.ExecuteResult(this.ControllerContext);
                return fileResult;

            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Exception in Index {ex}");
                return View("Error");
            }
        }

        public ActionResult ExportToPdf(string location,string reportDate,string reportName)
        {
            try
            {
                GroupLReportSearchModel searchModel = new GroupLReportSearchModel
                {
                    EmployeeNumber = "All",
                    LocationAutoId = location,
                    CurrentReport = (GroupLReports)reportName.ParseInt(),
                    ReportDate = reportDate

                };
                SelectPdf.PdfDocument mainDocument = new SelectPdf.PdfDocument();
                #region Commented
                ////var report = Request.Form["CurrentReport"].ParseToText();
                ////var currentLocation = Request.Form["CurrentLocation"].ParseToText();
                ////var reportDate = Request.Form["ReportDate"].ParseToText();
                ////var htmlString = _groupLReportBusiness.GeneratePhotoDashboard(new GroupLReportSearchModel { EmployeeNumber = "", ReportDate = reportDate, LocationAutoId = "20546" });
                //var reports = _groupLReportBusiness.GenerateDashboardReport(new GroupLReportSearchModel { EmployeeNumber = "All", ReportDate = "07-07-2023 00:00", LocationAutoId = "1",CurrentReport=GroupLReports.CheckListDashboard });
                //var reports = _groupLReportBusiness.GenerateDashboardReport(new GroupLReportSearchModel { EmployeeNumber = "All", ReportDate = "07-06-2023 00:00", LocationAutoId = "1",CurrentReport=GroupLReports.RegisterDashboard });
                #endregion
                var reports = _groupLReportBusiness.GenerateDashboardReport(searchModel);
                if (reports == null || reports.Count <= 0)
                {
                    TempData["Message"] = new MessageInfo { HasIssue = false, Message = "No records found" };
                    return RedirectToAction("Index");
                }
                int counter = 1;
                foreach (var htmlString in reports)
                {
                    HtmlToPdf converter = new HtmlToPdf();

                    // set converter options
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                    converter.Options.WebPageWidth = 1024;
                    converter.Options.WebPageHeight = 768;

                    // create a new pdf document converting an url
                    //= converter.ConvertHtmlString(htmlString);
                    mainDocument.Append(converter.ConvertHtmlString(htmlString));

                    // close pdf document


                    // return resulted pdf document

                    counter++;
                }
                var pdfBytes = mainDocument.Save();
                mainDocument.Close();

                return Json(Convert.ToBase64String(pdfBytes),JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Exception in Index {ex}");
                return View("Error");
            }
        }



        [HttpPost]
        public ActionResult FetchClients(string locationCode)
        {
            var locations = _master.FetchClients(locationCode);
            var locationList = locations.ToSelectList("ClientCode", "ClientName");
            return Json(locationList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FetchSites(string locationCode,string clientCode,string companyCode)
        {
            var locations = _master.FetchSites(locationCode, clientCode, companyCode);
            var locationList = locations.ToSelectList("AsmtId", "AsmtName");
            return Json(locationList, JsonRequestBehavior.AllowGet);
        }

    }

}




