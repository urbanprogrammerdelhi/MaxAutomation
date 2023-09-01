using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
    public class CheckListReportModel
    {
                   
        public int CheckListID { get; set; }
        public string ChecklistDetail { get; set; }
        public string EmpId { get; set; }
        public string EmployeeName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string ClientCode { get; set; }
        public string Status { get; set; }
        public byte[] Image { get; set; }





    }
}
