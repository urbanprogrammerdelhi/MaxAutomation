using Sams.Extensions.Data;
using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Business
{
   public class GroupLReportBusiness:IGroupLReportBusiness

    {
        private readonly IGroupLReportData _reportData;
        public GroupLReportBusiness(IGroupLReportData reportData)
        {
            _reportData = reportData;
        }

        public FsaReportData FsaDetails(FsaSearchModel searchModel)
        {
            try
            {
                return _reportData.FsaDetails(searchModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<GroupLReportDataSet> GenerateDashboard(GroupLReportSearchModel searchModel)
        {
            try
            {
                return _reportData.GenerateDashboard(searchModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public GroupLReportDataSet<DataSet> GenerateFsaReport(GroupLReportSearchModel searchModel)
        //{
        //    try
        //    {
        //        return _reportData.GenerateFsaReport(searchModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public List<string> GenerateFsaReportDetails(FsaSearchModel searchModel)
        {
            try
            {
                return _reportData.GenerateFsaReportDetails(searchModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

      

        List<string> IGroupLReportBusiness.GenerateDashboardReport(GroupLReportSearchModel searchModel)
        {
            try
            {
                return _reportData.GenerateDashboardReport(searchModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
