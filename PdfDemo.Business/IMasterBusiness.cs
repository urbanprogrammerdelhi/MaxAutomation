using PdfDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfDemo.Business
{
    public interface IMasterBusiness
    {
        List<Location> FetchLocations(string companyCode, string region);
        List<CompanyDetails> FetchCompanyDetails();
    }
}
