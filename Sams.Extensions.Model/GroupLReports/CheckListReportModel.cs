using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
    public class CheckListReportModel
    {

        public int AutoID { get; set; }

        public int HeaderAutoID { get; set; }
        public string SiteName { get; set; }
        public string ChecklistHeader { get; set; }
        public string ChecklistName { get; set; }
        public string ChecklistStatus { get; set; }
        public string CompletionTime { get; set; }
        public string Remarks { get; set; }
        public byte[] ChecklistImage { get; set; }


    }
}
