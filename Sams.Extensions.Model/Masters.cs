using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Model
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
    public class Region
    {
        public decimal RegionId { get; set; }
        public string RegionName { get; set; }
        public string CompanyCode { get; set; }
        public string RegionDescription { get; set; }
    }
}
