using PdfDemo.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PdfDemo
{
    public class MaxAuditViewModel
    {
        public IList<SelectListItem> Locations { get; set; }
        public IList<SelectListItem> Companies { get; set; }
        public string CurrentCompany { get; set; }
        public string CurrentLocation { get; set; }
        public IList<BranchDetails> BranchDetails { get; set; }
        public BranchDetails CurrentBranch { get; set; }
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
        public static MaxAuditViewModel DefaultInstance
        {
            get
            {
                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                return new MaxAuditViewModel
                {
                    BranchDetails = new List<BranchDetails>(),
                    Companies = new List<SelectListItem>(),
                    CurrentBranch = new BranchDetails(),
                    CurrentCompany = string.Empty,
                    CurrentLocation = string.Empty,
                    Locations = new List<SelectListItem>(),
                    FromDate= currentDate.AddDays(Convert.ToInt32(ConfigurationManager.AppSettings["FromDays"])),
                    ToDate = currentDate

                };
            }
        }
    }
}