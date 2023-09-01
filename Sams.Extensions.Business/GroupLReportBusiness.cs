using Sams.Extensions.Data;
using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
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

        public GroupLReportDataSet GenerateDashboard(GroupLReportSearchModel searchModel)
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
