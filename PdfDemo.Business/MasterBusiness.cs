using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaxAutomation.Model;
using MaxAutomation.Dal.DAL;

namespace MaxAutomation.Business
{
    public class MasterBusiness : IMasterBusiness
    {
        readonly IMasterData _masterData;
        public MasterBusiness(IMasterData masterData)
        {
            _masterData = masterData;
        }

        public List<CompanyDetails> FetchCompanyDetails()
        {
            return _masterData.FetchCompanies();
        }

        public List<Location> FetchLocations(string companyCode, string region)
        {
            return _masterData.FetchLocations(companyCode, region);
        }
    }
}
