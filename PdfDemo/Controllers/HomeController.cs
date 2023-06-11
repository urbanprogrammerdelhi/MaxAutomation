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
    static class MyExtensions
    {
        public static TRes Transform<TSrc, TRes>(this TSrc src, Func<TSrc, TRes> selector)
        {
            return selector(src);
        }
        public static List<SelectListItem> ToSelectList<T>(this List<T> list, string idPropertyName, string namePropertyName = "Name")
       where T : class, new()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            list.ForEach(item =>
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = item.GetType().GetProperty(namePropertyName).GetValue(item).ToString(),
                    Value = item.GetType().GetProperty(idPropertyName).GetValue(item).ToString()
                });
            });

            return selectListItems;
        }
    }
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
                var data = DataAccessLayer.FetchClientCode(vm.CurrentLocation,vm.FromDate.ToString(),vm.ToDate.ToString());
                vm.BranchDetails = data;
                return View(vm);
            }
            catch (Exception ex)
            {
                throw;
            }
            //var test = _master.FetchLocations("", "");
            //var data = DataAccessLayer.FetchClientCode("215", "29-May-2023", "31-May-2023");
            //return View(data);
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
            string location = "215";
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
    }
}