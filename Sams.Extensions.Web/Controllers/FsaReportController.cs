using iTextSharp.text;
using iTextSharp.text.pdf;
using Sams.Extensions.Business;
using Sams.Extensions.Dal;
using Sams.Extensions.Logger;
using Sams.Extensions.Model;
using Sams.Extensions.Utility;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sams.Extensions.Web.Controllers
{
    [CustomAuthorizeAttribute]
   public class FsaReportController : Controller
    {
        private readonly IMasterBusiness _master;
        private readonly ILoggerManager _loggerManager;
        private readonly IGroupLReportBusiness _groupLReportBusiness;
        public FsaReportController(IMasterBusiness master, ILoggerManager loggerManager, IGroupLReportBusiness groupLReportBusiness)
        {
            _master = master;
            _loggerManager = loggerManager;
            _groupLReportBusiness = groupLReportBusiness;
        }
        // GET: Branch
        public ActionResult Index()
        {
            _loggerManager.LogError($"Logging Test");

            try
            {
                var vm = FsaViewModel.DefaultInstance;
                if (TempData["FsaData"] != null)
                {
                    vm = (FsaViewModel)TempData["FsaData"];
                }              
                var currentCompany = (Session["UserInfo"] as Account).Company;
                var locations = _master.FetchLocations(currentCompany, "All");
                vm.Locations = locations.ToSelectList("LocationAutoId", "Locationdesc");
                
               
                DateTime? fromDate;
                DateTime? toDate;
                FsaConstants.CalculateFromToDate(vm.CurrentYear, vm.CurrentQuarter, out fromDate, out toDate);

                //if (!string.IsNullOrEmpty(vm.CurrentLocation))
                //{
                    var clients = _master.FetchClients(vm.CurrentLocation, fromDate, toDate);
                    vm.Clients = clients.ToSelectList("ClientCode", "ClientName");
               // }
                vm.ReportData = _groupLReportBusiness.FsaDetails(new FsaSearchModel { ClientCode = vm.CurrentClient, FromDate = fromDate, ToDate = toDate });
                return View(vm);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Exception in Index {ex}");
                throw;
            }
        }
        [HttpPost]
        public ActionResult Search()
        {
            var vm = FsaViewModel.DefaultInstance;
            vm.CurrentLocation = Request.Form["CurrentLocation"].ParseToText();
            vm.CurrentClient = Request.Form["CurrentClient"].ParseToText();
            vm.CurrentYear = Request.Form["CurrentYear"].ParseToText();
            vm.CurrentQuarter = Request.Form["CurrentQuarter"].ParseToText();
            TempData["FsaData"] = vm;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult FetchLocations(string CompanyCode,string RegionCode)
        {
            var locations = _master.FetchLocations(CompanyCode, RegionCode);
            var locationList = locations.ToSelectList("LocationAutoId", "Locationdesc");
            return Json(locationList, JsonRequestBehavior.AllowGet);
        }      
     
        [HttpPost]
        public ActionResult FetchClients(string locationCode, string selectedYear, string selectedQuarter)
        {
            DateTime? fromDate;
            DateTime? toDate;
            FsaConstants.CalculateFromToDate(selectedYear, selectedQuarter, out fromDate, out toDate);

            var locations = _master.FetchClients(locationCode, fromDate, toDate);
            var locationList = locations.ToSelectList("ClientCode", "ClientName");
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
                        return Search();
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
                var vm = FsaViewModel.DefaultInstance;
                vm.CurrentLocation = Request.Form["CurrentLocation"].ParseToText();
                vm.CurrentClient = Request.Form["CurrentClient"].ParseToText();
                vm.CurrentYear = Request.Form["CurrentYear"].ParseToText();
                vm.CurrentQuarter = Request.Form["CurrentQuarter"].ParseToText();

                SelectPdf.PdfDocument mainDocument = new SelectPdf.PdfDocument();
               
                DateTime? fromDate;
                DateTime? toDate;
                FsaConstants.CalculateFromToDate(vm.CurrentYear, vm.CurrentQuarter, out fromDate, out toDate);
                var reports = _groupLReportBusiness.GenerateFsaReportDetails(new FsaSearchModel {ClientCode=vm.CurrentClient,FromDate=fromDate,ToDate=toDate });

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
                FileResult fileResult = new FileContentResult(pdfBytes, "application/pdf")
                {
                    FileDownloadName = $"{vm.CurrentClient}_{DateTime.Now.ToString("ddmmyyyyhhmmss")}.pdf"
                };
                fileResult.ExecuteResult(this.ControllerContext);
                return fileResult;

            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Exception in Index {ex}");
                return View("Error");
            }
        }



        public ActionResult SetImage(byte[] image)
        {
            if (image != null)
            {
                return File(image, "image/jpg");
            }
            else
            {
                string path = Server.MapPath("~/Images/NoImagesFound.jpg");

                //Read the File data into Byte Array.
                byte[] bytes = System.IO.File.ReadAllBytes(path);

                //Send the File to Download.
                return File(bytes, "image/jpg");
            }
        }
    }
  
}