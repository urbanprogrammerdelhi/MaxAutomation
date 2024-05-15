using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Business
{
   public interface IGroupLReportBusiness
    {
        List<GroupLReportDataSet> GenerateDashboard(GroupLReportSearchModel searchModel);
        List<string> GenerateDashboardReport(GroupLReportSearchModel searchModel);
        List<string> GenerateFsaReportDetails(FsaSearchModel searchModel);
        FsaReportData FsaDetails(FsaSearchModel searchModel);

        //GroupLReportDataSet<DataSet> GenerateFsaReport(GroupLReportSearchModel searchModel);




    }
}
