using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Dal
{
    public interface IMasterData 
    {
        List<Location> FetchLocations(string companycode, string region);
        List<CompanyDetails> FetchCompanies();
        List<ReportDataModel> FetchReports(string companycode);
        List<Region> FetchRegions(string companycode);
        List<string> ListOfPosts();
        List<ClientModel> FetchClients(string locationId);
        List<SiteModel> FetchSites(string locationId,string clientCode,string companyCode);

    }



}
