using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
   
    public class ViewEmployeeRosterModel
    {
        public string EmployeeInformation { get; set; }
        public string RosterDay { get; set; }
        public string Shift { get; set; }
        public string Post { get; set; }

    }
    public class ViewEmployeeRosterSearchModel
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Location { get; set; }
    }
}
