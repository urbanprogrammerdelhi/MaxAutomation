using PdfDemo.Data;
using PdfDemo.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfDemo.Dal.DAL
{
    public interface IMasterData 
    {
        List<Location> FetchLocations(string companycode, string region);
        List<CompanyDetails> FetchCompanies();
    }
   

 
}
