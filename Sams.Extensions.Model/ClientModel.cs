using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
{
    public class ClientModel
    {
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string LocationCode { get; set; }
        public string LocationDesc { get; set; }
        public decimal LocationAutoID { get; set; }
    }
    public class SiteModel
    {
        public string AsmtName { get; set; }
        public string AsmtId { get; set; }
    }
}
