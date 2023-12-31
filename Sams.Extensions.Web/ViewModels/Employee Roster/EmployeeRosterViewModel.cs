﻿using Sams.Extensions.Model;
using Sams.Extensions.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Sams.Extensions.Web
{

    public class EmployeeRosterViewModel
    {
        public  List<SelectListItem> Posts { get; set; }
        public string SelectedPost { get; set; }
        public string CurrentReport { get; set; }
        public string CurrentYear { get; set; }
        public string CurrentMonth { get; set; }
        public IList<SelectListItem> Years { get; set; }
        public IList<SelectListItem> Months { get; set; }
        public IList<SelectListItem> Locations { get; set; }
        public IList<SelectListItem> Companies { get; set; }
        public string CurrentCompany { get; set; }
        public string CurrentLocation { get; set; } 
        public IList<EmployeeRosterModel> EmployeeRosterDetails { get; set; }
        public EmployeeRosterModel CurrentEmployeeRosterModel { get; set; }
        public bool ShowExportToExcelButton { get; set; }
        public IList<SelectListItem> Reports { get; set; }
        public static EmployeeRosterViewModel DefaultInstance
        {
            get
            {

                EmployeeRosterViewModel instance = new EmployeeRosterViewModel
                {
                    Companies = new List<SelectListItem>(),
                    CurrentCompany = string.Empty,
                    CurrentEmployeeRosterModel = new EmployeeRosterModel(),
                    CurrentLocation = string.Empty,
                    EmployeeRosterDetails = new List<EmployeeRosterModel>(),
                    Locations = new List<SelectListItem>(),
                    Months = new List<SelectListItem>(),
                    CurrentMonth = string.Empty,
                    CurrentYear = string.Empty,
                    Years = new List<SelectListItem>(),
                    ReportData=new EmployeeRosterReportModel(),
                    //EmployeeRosterData=new Dictionary<string, DataTable>(),
                    ShowExportToExcelButton=false,
                    Reports=new List<SelectListItem>(),
                    CurrentReport=string.Empty,
                    SelectedPost=string.Empty,
                    Posts=new List<SelectListItem>()

                };
                int fromYear = ConfigurationFields.FromYear;
                int toYear = ConfigurationFields.ToYear;
                for (int i = fromYear; i <= toYear; i++)
                {
                    instance.Years.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
                var months = ConfigurationFields.Months;
                foreach (var month in months)
                {
                    instance.Months.Add(new SelectListItem { Text = month, Value = ((int)Enum.Parse(typeof(Months), month)).ToString() });
                }
                foreach(var item in CommonUtility.EmployeeeRosterReports)
                {
                    instance.Reports.Add(new SelectListItem { Value=item.Key.ToString(),Text=item.Value.ToString()});
                }
                return instance;


            }
        }
        public EmployeeRosterReportModel ReportData { get; set; }
        [AllowHtml]
        public string GridHtml { get; set; }


    }
}