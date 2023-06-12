using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfDemo.Business;
using PdfDemo.Data;
using PdfDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PdfDemo
{
   
     
    public class HomeController : Controller
    {
        private readonly IMasterBusiness _master;
        public HomeController(IMasterBusiness master)
        {
            _master = master;
        }
        public ActionResult Index()
        {
            try
            {
                var vm = MaxAuditViewModel.DefaultInstance;
                if (TempData["MaxData"]!=null)
                {
                    vm = (MaxAuditViewModel)TempData["MaxData"];
                }                   
                var companies = _master.FetchCompanyDetails();
                vm.Companies = companies.ToSelectList("CompanyCode", "CompanyDesc");
                if(!string.IsNullOrEmpty(vm.CurrentCompany))
                {
                    var locations = _master.FetchLocations(vm.CurrentCompany, "All");
                    vm.Locations = locations.ToSelectList("LocationAutoId", "Locationdesc");
                }
                var data = DataAccessLayer.FetchClientCode(vm.CurrentLocation, vm.FromDate.ToString("dd-MMM-yyyy"), vm.ToDate.ToString("dd-MMM-yyyy"));
                vm.BranchDetails = data;
                return View(vm);
            }
            catch (Exception ex)
            {
                throw;
            }
          
        }
        [HttpPost]
        public ActionResult Search()
        {
            var vm = MaxAuditViewModel.DefaultInstance;
            vm.CurrentCompany = Request.Form["CurrentCompany"].ToString();
            vm.CurrentLocation = Request.Form["CurrentLocation"].ToString();
            vm.FromDate = Convert.ToDateTime(Request.Form["Fromdate"]);
            vm.ToDate = Convert.ToDateTime(Request.Form["ToDate"]);
            TempData["MaxData"] = vm;

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult FetchLocations(string CompanyCode)
        {
            var locations = _master.FetchLocations(CompanyCode, "All");
            var locationList = locations.ToSelectList("LocationAutoId", "Locationdesc");
            return Json(locationList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(string Branch, string AuditDate, string Location)
        {
            var data = DataAccessLayer.GetReportValues(Location, Branch, AuditDate);
            return View(data);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Export()
        {
            string location = Request.Form["Location"].ToString();
            string branch = Request.Form["Branch"].ToString();
            string auditdate = Request.Form["AuditDate"].ToString();
            var data = DataAccessLayer.GetReportValues(location, branch, auditdate);
            Document pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            PdfReportBuilder builder = new PdfReportBuilder(pdfDoc);
            builder.CreateHeader(data.Header);
            builder.CreateDetails(data.MasterdetailList);
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", $"attachment;filename={branch.Trim()}.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
            return View();

        }

        public ActionResult RetrieveImage(int ImageId)
        {
            byte[] cover = DataAccessLayer.GetImageById(ImageId);
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
       
    }
}