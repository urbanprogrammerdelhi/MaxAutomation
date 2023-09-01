using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
    public class RegisterData
    {
        public int SLNo { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }
        public byte[] EmployeeImage { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime DutyDateTime { get; set; }
        public string ClusterNo { get; set; }
        public string LocationName { get; set; }
        public string ShiftDetails { get; set; }
    }
    public class SearchRegisterData
    {
       public string Location { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string EmployeeNumber { get; set; }
    }
}
