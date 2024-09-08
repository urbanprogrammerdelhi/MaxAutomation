using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
    public class FsaDetails
    {
        [Display(Name = "SlNo")]

        public string SerialNumer { get; set; }
        [Display(Name ="Year")]
        public string Year { get; set; }
        [Display(Name = "Quarter")]

        public string Quarter { get; set; }
        [Display(Name = "Zone")]
        public string Zone { get; set; }
        [Display(Name = "Client")]

        public string Client { get; set; }
        public string ClientCode { get; set; }
        [Display(Name = "Action")]
       
        public string Action { get; set; }
        public string LocationCode { get; set; }
        public decimal LocationAutoID { get; set; }
        public string RequiredDetails
        {
            get;set;
        }
    }
}
