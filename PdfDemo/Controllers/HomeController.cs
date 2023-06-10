using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PdfDemo
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var data = DataAccessLayer.FetchClientCode("215", "29-May-2023", "31-May-2023");
            return View(data);
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