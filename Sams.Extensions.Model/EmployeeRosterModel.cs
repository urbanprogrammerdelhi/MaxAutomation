using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{

    public class EmployeeRosterModel
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Day { get; set; }
        public string LocationId { get; set; }
        public string LoggedInPerson { get; set; }
        public string Floor { get; set; }
        public string AttendanceStatus { get; set; }
    }
    public class EmployeeRosterHeader
    {
       
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string LoggedInPerson { get; set; }
        public List<EmployeeRosterDetails> RosterDetails { get; set; }
       
    }
    public class EmployeeRosterDetails
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string EmployeeId { get; set; }
        public string Day { get; set; }
        public string LocationId { get; set; }
        public string Post { get; set; }
        public string Shift { get; set; }
        public string LoggedInUser { get; set; }
    }
    public class EmployeeRosterSearchModel
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Location { get; set; }
        public string Floor { get; set; }
    }


    
}



