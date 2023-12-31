﻿using ClosedXML.Excel;
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
using System.Threading;
using System.Data.OleDb;

namespace Sams.Extensions.Web.Controllers
{
    [CustomAuthorizeAttribute]
    public class EmployeeRosterController : Controller
    {

        private readonly IMasterBusiness _master;
        private readonly ILoggerManager _loggerManager;
        private readonly IEmployeeRosterBusiness _employeeRosterBusiness;
        public EmployeeRosterController(IMasterBusiness master, ILoggerManager loggerManager, IBranchCodeData dataAccesLayer, IEmployeeRosterBusiness employeeRosterBusiness)
        {
            _master = master;
            _loggerManager = loggerManager;
            _employeeRosterBusiness = employeeRosterBusiness;
        }
        // GET: Branch
        public ActionResult Index()
        {
            _loggerManager.LogError($"Logging Test");

            try
            {
                var vm = EmployeeRosterViewModel.DefaultInstance;
                if (TempData["UploadMessage"] != null)
                {
                    ViewBag.MessageInfo = (MessageInfo)TempData["UploadMessage"];
                }
                if (TempData["EmployeeRosterData"] != null)
                {
                    vm = (EmployeeRosterViewModel)TempData["EmployeeRosterData"];
                }
                var companies = _master.FetchCompanyDetails();
                vm.Companies = companies.ToSelectList("CompanyCode", "CompanyDesc");
                vm.CurrentCompany = (Session["UserInfo"] as Account).Company;
                if (!string.IsNullOrEmpty(vm.CurrentCompany))
                {
                    var locations = _master.FetchLocations(vm.CurrentCompany, "All");
                    vm.Locations = locations.ToSelectList("LocationAutoId", "Locationdesc");
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
        public ActionResult Upload(HttpPostedFileBase postedFile)
        {
            var loggedInUser = (Session["UserInfo"] as Account);
            try
            {
                var vm = EmployeeRosterViewModel.DefaultInstance;
                vm.CurrentYear = Request.Form["CurrentYear"].ParseToText();
                vm.CurrentMonth = Request.Form["CurrentMonth"].ParseToText();
                vm.CurrentLocation = Request.Form["CurrentLocation"].ParseToText();
                var daysInMonth = DateTime.DaysInMonth(vm.CurrentYear.ParseInt(), vm.CurrentMonth.ParseInt());
                List<string> secondHeaderArray = CommonUtility.rosterDataInitials.Split(',').ToList();
                TempData["EmployeeRosterData"] = vm;
                string uploadedFileFullPath;
                string uploadedMessage = UploadPostedFile(postedFile, out uploadedFileFullPath);
                if (!string.IsNullOrEmpty(uploadedMessage))
                {
                    TempData["UploadMessage"] = new MessageInfo { HasIssue = true, Message = uploadedMessage };
                    return RedirectToAction("Index");
                }
                //Open the Excel file using ClosedXML.
                using (XLWorkbook workBook = new XLWorkbook(uploadedFileFullPath))
                {

                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheets.FirstOrDefault();
                    if (workSheet == null || workSheet.Rows() == null || workSheet.Rows().Count() <= 0)
                    {
                        TempData["UploadMessage"] = new MessageInfo { HasIssue = true, Message = "The selected excel file is empty. Please select another valid file to continue.." };
                        return RedirectToAction("Index");
                    }
                    //Loop through the Worksheet rows.
                    int counter = 0;

                    string headerStatus = ValidateHeader(workSheet.Rows().Take(2).ToList(), vm.CurrentMonth.ParseInt(), vm.CurrentYear.ParseInt());
                    if (!string.IsNullOrEmpty(headerStatus))
                    {
                        TempData["UploadMessage"] = new MessageInfo { HasIssue = true, Message = headerStatus };
                        return RedirectToAction("Index");
                    }
                    var dataRows = workSheet.Rows().Skip(2).ToList();

                    foreach (ClosedXML.Excel.IXLRow row in dataRows)
                    {
                        try
                        {
                            var cells = row.Cells(false, XLCellsUsedOptions.All).ToList();
                            var cellCount = (daysInMonth * 2) + 4;
                            int shiftCellCount = 0;
                            EmployeeRosterHeader employeeRosterHeader = new EmployeeRosterHeader
                            {
                                Designation = cells[secondHeaderArray.LastIndexOf("Designation")].Value.ParseToText(),
                                EmployeeCode = cells[secondHeaderArray.LastIndexOf("Employee Code")].Value.ParseToText(),
                                EmployeeName = cells[secondHeaderArray.LastIndexOf("Employee Name")].Value.ParseToText(),
                                LoggedInPerson = loggedInUser.UserName,
                                RosterDetails = new List<EmployeeRosterDetails>()
                            };
                            var shiftCells = cells.Skip(4).ToList();

                            if (!string.IsNullOrEmpty(employeeRosterHeader.EmployeeCode))
                            {
                                var employeeId = _employeeRosterBusiness.SaveEmployeeData(employeeRosterHeader);

                                for (int i = 1; i <= daysInMonth; i++)
                                {

                                    var employeeRosterData = new EmployeeRosterDetails
                                    {
                                        LoggedInUser = loggedInUser.UserName,
                                        Year = vm.CurrentYear,
                                        Day = i.ParseToText(),
                                        EmployeeId = employeeId.ParseToText(),
                                        Post = shiftCells[shiftCellCount + 1] != null ? shiftCells[shiftCellCount + 1].Value.ParseToText() : string.Empty,
                                        LocationId = vm.CurrentLocation,
                                        Month = vm.CurrentMonth,
                                        Shift = shiftCells[shiftCellCount] != null ? shiftCells[shiftCellCount].Value.ParseToText() : string.Empty
                                    };
                                    var result = _employeeRosterBusiness.SaveEmployeeRosterData(employeeRosterData);
                                    shiftCellCount += 2;

                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        counter++;
                    }
                    if (counter > 0)
                    {
                        TempData["UploadMessage"] = new MessageInfo { HasIssue = false, Message = "Successfully uploaded employee roster plan." };
                        TempData["EmployeeRosterData"] = null;

                    }
                }
                return RedirectToAction("Index");
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
                return View();
                //if (TempData["EmployeeRosterData"] == null)
                //{
                //    return RedirectToAction("ViewEmployeeRoster");
                //}

                //var vm = (EmployeeRosterViewModel)TempData["EmployeeRosterData"];
                //var employeeData = _employeeRosterBusiness.FetchEmployeeRosterData(new ViewEmployeeRosterSearchModel { Location = vm.CurrentLocation, Month = vm.CurrentMonth, Year = vm.CurrentYear });
                //if (employeeData == null || employeeData.Count <= 0)
                //{
                //    return RedirectToAction("ViewEmployeeRoster");
                //}
                //StringBuilder sb = new StringBuilder();
                //sb.Append(ConfigurationFields.HtmlBeginTag).Append(ConfigurationFields.BodyBeginTag);
                //sb.Append(ConfigurationFields.BeginTableTag);

                //foreach (var floorData in employeeData)
                //{
                //    var keyArray = floorData.Key.Split(',');
                //    sb.Append(ConfigurationFields.BeginRowTag);

                //    sb.Append($"<td style='background-color:#D3D3D3;border: 1px solid #ccc' colspan={floorData.Value.Columns.Count}>{keyArray[0]}({keyArray[1]})</td>");

                //    sb.Append(ConfigurationFields.EndRowTag);

                //    sb.Append(ConfigurationFields.BeginRowTag);
                //    foreach (DataColumn dataColumn in floorData.Value.Columns)
                //    {
                //        sb.Append(string.Format(ConfigurationFields.DefaultHeaderFormat, dataColumn.ColumnName));
                //    }
                //    sb.Append(ConfigurationFields.EndRowTag);


                //    foreach (DataRow dr in floorData.Value.Rows)
                //    {
                //        sb.Append(ConfigurationFields.BeginRowTag);
                //        foreach (DataColumn dataColumn in floorData.Value.Columns)
                //        {
                //            sb.Append(string.Format(ConfigurationFields.ColumnFormat, dr[dataColumn].ParseToText()));
                //        }
                //        sb.Append(ConfigurationFields.EndRowTag);

                //    }


                //}
                //sb.Append(ConfigurationFields.EndTableTag).Append(ConfigurationFields.BodyEndTag).Append(ConfigurationFields.HtmlEndTag);
                //var test = sb.ToString();
                //return File(Encoding.ASCII.GetBytes(sb.ToString()), "application/vnd.ms-excel", "Grid.xls");
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Exception in Index {ex}");

                return View("Error");
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitRequest(string Submit)
        {
            try
            {

                EmployeeRosterViewModel vm = EmployeeRosterViewModel.DefaultInstance;
                var companies = _master.FetchCompanyDetails();
                vm.Companies = companies.ToSelectList("CompanyCode", "CompanyDesc");
                vm.CurrentCompany = (Session["UserInfo"] as Account).Company;
                vm.CurrentYear = Request.Form["CurrentYear"].ParseToText();
                vm.CurrentMonth = Request.Form["CurrentMonth"].ParseToText();
                vm.CurrentLocation = Request.Form["CurrentLocation"].ParseToText();
                vm.CurrentReport= Request.Form["CurrentReport"].ParseToText();
                vm.SelectedPost= Request.Form["SelectedPost"].ParseToText();
                var posts = _master.ListOfPosts();
                vm.Posts = posts.Select(n => new SelectListItem { Text = n, Value = n }).ToList();
                if (!string.IsNullOrEmpty(vm.CurrentCompany))
                {
                    var locations = _master.FetchLocations(vm.CurrentCompany, "All");
                    vm.Locations = locations.ToSelectList("LocationAutoId", "Locationdesc");
                }
                TempData["EmployeeRosterData"] = vm;
                switch (Submit)
                {
                    case "Export to Excel":
                        return (Export());
                    case "Submit":
                        return RedirectToAction("Reports");
                }
                return View();
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Exception in Index {ex}");
                return View("Error");
            }
        }
       

        static DataSet Parse(string fileName)
        {
            string connectionString = string.Format("provider=Microsoft.Jet.OLEDB.4.0; data source={0};Extended Properties=Excel 8.0;", fileName);


            DataSet data = new DataSet();

            foreach (var sheetName in GetExcelSheetNames(connectionString))
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    var dataTable = new DataTable();
                    string query = string.Format("SELECT * FROM [{0}]", sheetName);
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    adapter.Fill(dataTable);
                    data.Tables.Add(dataTable);
                }
            }

            return data;
        }
        static string[] GetExcelSheetNames(string connectionString)
        {
            OleDbConnection con = null;
            DataTable dt = null;
            con = new OleDbConnection(connectionString);
            con.Open();
            dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (dt == null)
            {
                return null;
            }

            String[] excelSheetNames = new String[dt.Rows.Count];
            int i = 0;

            foreach (DataRow row in dt.Rows)
            {
                excelSheetNames[i] = row["TABLE_NAME"].ToString();
                i++;
            }

            return excelSheetNames;
        }

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult Export(string GridHtml)
        //{  
        //    return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "Grid.xls");

        //}


        private string ValidateHeader(List<ClosedXML.Excel.IXLRow> headerRows, int month, int year)
        {
            try
            {
                if (headerRows == null)
                    return "Invalid excel format. Please contact administrator";
                var selectedMonth = new DateTime(year, month, 1).ToString("MMM yyyy");
                var topHeader = headerRows[0].ConcatinateCells();
                var requiredTopHeader = CommonUtility.GenerateDayHeader(month, year);
                if (topHeader != requiredTopHeader)
                    return $"Invalid format ! {selectedMonth} should have maximum {DateTime.DaysInMonth(year, month)} days of data.";
                var dataHeader = headerRows[1].ConcatinateCells();
                var requiredDataHeader = CommonUtility.GenerateRosterDataHeader(month, year);
                if (dataHeader != requiredDataHeader)
                    return "Invalid Roster format. Please contact administrator";
                return string.Empty;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"{ex}");

            }
            return "Invalid Roster format. Please contact administrator";
        }
       
        private string UploadPostedFile(HttpPostedFileBase postedFile,out string uploadedFileFullPath)
        {
            uploadedFileFullPath = string.Empty;

            try
            {
                if (postedFile == null || postedFile.InputStream == null)
                    return "Please select the required Exployee Rroster Data to continue the process." ;
                
                var uploadFolder = Server.MapPath("~/Upload/");
                var selectedFile = postedFile.FileName;
                var selectedfileName = Path.GetFileNameWithoutExtension(selectedFile);
                var extension = Path.GetExtension(selectedFile);
                if (!ConfigurationFields.RequiredExtensionsForEmployeeRoster.Contains(extension))
                  return $"Please select the required Exployee Rroster Data in 'xlsx format' to continue the process." ;

                string uploadFileName = selectedfileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                // Create the folder if it does not exist.
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                // Save the uploaded file to the server.
                uploadedFileFullPath = uploadFolder + uploadFileName;
                postedFile.SaveAs(uploadedFileFullPath);
                if (!System.IO.File.Exists(uploadedFileFullPath))
                return "The selected file could not be uploaded. Please try again." ;                  

                return string.Empty;
            }
            catch(Exception ex)
            {
                _loggerManager.LogError($"{ex}");
            }
            return "The selected file could not be uploaded. Please try again.";

        }

        public ActionResult Reports()
        {
            try
            {
                var vm = EmployeeRosterViewModel.DefaultInstance;
                if (TempData["EmployeeRosterData"] != null)
                {
                    vm = (EmployeeRosterViewModel)TempData["EmployeeRosterData"];
                }
                else
                {
                    var posts=_master.ListOfPosts();
                    vm.Posts = posts.Select(n => new SelectListItem { Text = n, Value = n }).ToList();
                    var companies = _master.FetchCompanyDetails();
                    vm.Companies = companies.ToSelectList("CompanyCode", "CompanyDesc");
                    vm.CurrentCompany = (Session["UserInfo"] as Account).Company;
                    if (!string.IsNullOrEmpty(vm.CurrentCompany))
                    {
                        var locations = _master.FetchLocations(vm.CurrentCompany, "All");
                        vm.Locations = locations.ToSelectList("LocationAutoId", "Locationdesc");
                    }
                }

                if (!string.IsNullOrEmpty(vm.CurrentYear) && !string.IsNullOrEmpty(vm.CurrentMonth) && !string.IsNullOrEmpty(vm.CurrentLocation))
                {
                    var employeeData = _employeeRosterBusiness.FetchEmployeeRosterDataReport(new ViewEmployeeRosterSearchModel { Location = vm.CurrentLocation, Month = vm.CurrentMonth, Year = vm.CurrentYear,ReportType= (EmployeeRosterReports)Enum.Parse(typeof(EmployeeRosterReports),vm.CurrentReport),Post=vm.SelectedPost });
                    vm.ReportData = employeeData;
                    //if (employeeData != null && employeeData.Count > 0)
                    //    vm.ShowExportToExcelButton = true;
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Exception in Index {ex}");
                return View("Error");
            }
        }
    }

}


