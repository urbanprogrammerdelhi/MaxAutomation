using iTextSharp.text;
using iTextSharp.text.pdf;
using Sams.Extensions.Business;
using Sams.Extensions.Dal;
using Sams.Extensions.Logger;
using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sams.Extensions.Web.Controllers
{
    [CustomAuthorizeAttribute]
   public class BranchNewController : Controller
    {
        private readonly IMasterBusiness _master;
        private readonly ILoggerManager _loggerManager;
        private readonly IBranchCodeData _dataAccesLayer;
        public BranchNewController(IMasterBusiness master, ILoggerManager loggerManager,IBranchCodeData dataAccesLayer)
        {
            _master = master;
            _loggerManager = loggerManager;
            _dataAccesLayer = dataAccesLayer;
        }
        // GET: Branch
        public ActionResult Index()
        {
            _loggerManager.LogError($"Logging Test");

            try
            {
                var vm = MaxAuditViewModel.DefaultInstance;
                if (TempData["MaxData"] != null)
                {
                    vm = (MaxAuditViewModel)TempData["MaxData"];
                }
                var companies = _master.FetchCompanyDetails();
                vm.Companies = companies.ToSelectList("CompanyCode", "CompanyDesc");
                vm.CurrentCompany = (Session["UserInfo"] as Account).Company;
                if (!string.IsNullOrEmpty(vm.CurrentCompany))
                {
                    var locations = _master.FetchLocations(vm.CurrentCompany, "All");
                    vm.Locations = locations.ToSelectList("LocationAutoId", "Locationdesc");
                }
                var checkListTypes = _dataAccesLayer.FetchChecklistTypes();
                vm.ChecklistTypes= checkListTypes.ToSelectList("Id", "DisplayLabel");
                var selectedCheckList = checkListTypes.SingleOrDefault(chk => chk.Id.ToString() == vm.SelectedChecklistType);
                var data = _dataAccesLayer.FetchClientCodeV2(vm.CurrentLocation, vm.FromDate.ToString("dd-MMM-yyyy"), vm.ToDate.ToString("dd-MMM-yyyy"), selectedCheckList!=null ? selectedCheckList.CheckListType:"");
                vm.BranchDetails = data;
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
            var vm = MaxAuditViewModel.DefaultInstance;
            vm.SelectedChecklistType = Request.Form["SelectedChecklistType"].ToString();
            vm.CurrentLocation = Request.Form["CurrentLocation"].ToString();
            vm.FromDate = Convert.ToDateTime(Request.Form["Fromdate"]);
            vm.ToDate = Convert.ToDateTime(Request.Form["ToDate"]);
            TempData["MaxData"] = vm;
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
        public ActionResult FetchRegions(string CompanyCode)
        {
            var locations = _master.FetchRegions(CompanyCode);
            var locationList = locations.ToSelectList("RegionName", "RegiRegionDescriptiononName");
            return Json(locationList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(string Branch, string AuditDate, string Location,string ChecklistType)
        {
            var data = _dataAccesLayer.GetReportValuesV2(Location, Branch, AuditDate, ChecklistType);
            return View(data);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Export()
        {
            try
            {
                string location = Request.Form["Location"].ToString();
                string branch = Request.Form["Branch"].ToString();
                string auditdate = Request.Form["AuditDate"].ToString();
                string checkListType = Request.Form["CheckListType"].ToString();

                var data = _dataAccesLayer.GetReportValuesV2(location, branch, auditdate, checkListType);
                Document pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                PdfReportBuilder builder = new PdfReportBuilder(pdfDoc, _dataAccesLayer);
                builder.CreateHeader(data.Header);
                builder.CreateDetails(data.MasterdetailList);
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"attachment;filename={data.Header.BranchName.Trim()}.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();
                
            }
            catch(Exception ex)
            {
                _loggerManager.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return View();
        }

        public ActionResult RetrieveImage(int ImageId)
        {
            byte[] cover = _dataAccesLayer.GetImageById(ImageId,true);
            if (cover != null)
            {
                return File(cover, "image/jpg");
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
        public ActionResult ViewMore(string CheckListId,string AuditDate, string Location, string Branch)
        {
            var images = _dataAccesLayer.FetchCheckListImageList(Location, Branch, AuditDate, CheckListId,true);
            return View(images);
        }

    }
  
}