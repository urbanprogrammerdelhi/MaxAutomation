﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfDemo.Business;
using PdfDemo.Data;
using PdfDemo.Logger;
using PdfDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PdfDemo.Controllers
{
    [CustomAuthorizeAttribute]
   public class BranchController : Controller
    {
        private readonly IMasterBusiness _master;
        private readonly ILoggerManager _loggerManager;
        private readonly DataAccessLayer _dataAccesLayer;
        public BranchController(IMasterBusiness master, ILoggerManager loggerManager,DataAccessLayer dataAccesLayer)
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
                var data = _dataAccesLayer.FetchClientCode(vm.CurrentLocation, vm.FromDate.ToString("dd-MMM-yyyy"), vm.ToDate.ToString("dd-MMM-yyyy"));
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
            //vm.CurrentCompany = Request.Form["CurrentCompany"].ToString();
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
            var data = _dataAccesLayer.GetReportValues(Location, Branch, AuditDate);
            return View(data);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Export()
        {
            string location = Request.Form["Location"].ToString();
            string branch = Request.Form["Branch"].ToString();
            string auditdate = Request.Form["AuditDate"].ToString();
            var data = _dataAccesLayer.GetReportValues(location, branch, auditdate);
            Document pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            PdfReportBuilder builder = new PdfReportBuilder(pdfDoc,_dataAccesLayer);
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
            return View();

        }

        public ActionResult RetrieveImage(int ImageId)
        {
            byte[] cover = _dataAccesLayer.GetImageById(ImageId);
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
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomAuthorizeAttribute()
        {
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return (httpContext.Session != null && httpContext.Session["UserInfo"] != null);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}