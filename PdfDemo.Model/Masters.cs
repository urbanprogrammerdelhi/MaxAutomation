using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfDemo.Model
{
    public class Location
    {
        public decimal LocationAutoId { get; set; }
        public string LocationCode { get; set; }
        public string Locationdesc { get; set; }
    }
    public class CompanyDetails
    {
        public string CompanyCode { get; set; }
        public string CompanyDesc { get; set; }
    }
    public class ImageModel
    {
        public int ImageAutoId { get; set; }
    }
}
