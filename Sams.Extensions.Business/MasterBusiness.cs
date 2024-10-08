﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sams.Extensions.Dal;
using Sams.Extensions.Model;


namespace Sams.Extensions.Business
{
    public class MasterBusiness : IMasterBusiness
    {
        readonly IMasterData _masterData;
        public MasterBusiness(IMasterData masterData)
        {
            _masterData = masterData;
        }

        public List<ClientModel> FetchClients(string locationId,DateTime? fromDate, DateTime? toDate)
        {
            return _masterData.FetchClients(locationId, fromDate, toDate);
        }

        public List<CompanyDetails> FetchCompanyDetails()
        {
            return _masterData.FetchCompanies();
        }

        public List<Location> FetchLocations(string companyCode, string region)
        {
            return _masterData.FetchLocations(companyCode, region);
        }

        public List<Region> FetchRegions(string companycode)
        {
            return _masterData.FetchRegions(companycode);
        }

        public List<ReportDataModel> FetchReports(string companycode)
        {
            return _masterData.FetchReports(companycode);
        }

        public List<SiteModel> FetchSites(string locationId, string clientCode, string companyCode)
        {
            return _masterData.FetchSites(locationId, clientCode, companyCode);
        }

        public List<string> ListOfPosts()
        {
            return _masterData.ListOfPosts();
        }
    }
}
