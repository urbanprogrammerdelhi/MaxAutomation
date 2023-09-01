using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
    public class RegisterDashboardModel
    {
        public int SlNo { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ClientCode { get; set; }
        public string VisitorName { get; set; }
        public string Purpose { get; set; }
        public string Mobile { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public byte[] EmployeeImage { get; set; }
    }
}
