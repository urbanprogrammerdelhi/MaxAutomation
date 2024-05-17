using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Business
{
    public interface IMasterBusiness
    {
        List<Location> FetchLocations(string companyCode, string region);
        List<CompanyDetails> FetchCompanyDetails();
        List<ReportDataModel> FetchReports(string companycode);
        List<Region> FetchRegions(string companycode);
        List<string> ListOfPosts();
        List<ClientModel> FetchClients(string locationId, DateTime? fromDate, DateTime? toDate);
        List<SiteModel> FetchSites(string locationId, string clientCode, string companyCode);


    }
}
