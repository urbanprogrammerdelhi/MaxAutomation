using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Data
{
    public interface IGroupLReportData
    {
        List<GroupLReportDataSet> GenerateDashboard(GroupLReportSearchModel searchModel);
        List<string> GenerateDashboardReport(GroupLReportSearchModel searchModel);
        List<string> GenerateFsaReportDetails(GroupLReportSearchModel searchModel);
        //GroupLReportDataSet GenerateFsaReport(GroupLReportSearchModel searchModel);
    }
}
