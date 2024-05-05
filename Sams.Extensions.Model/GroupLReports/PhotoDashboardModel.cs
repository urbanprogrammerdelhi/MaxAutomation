using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
    public class PhotoDashboardModel
    {
        
        public long SlNo { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public string ShiftDetails { get; set; }
        public string ClientName { get; set; }
        public string SiteName { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string GeoAddress { get; set; }
        public byte[] EmployeeImage { get; set; }

    }
}
