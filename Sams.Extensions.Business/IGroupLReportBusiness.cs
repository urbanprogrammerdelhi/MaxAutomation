using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Business
{
   public interface IGroupLReportBusiness
    {
        GroupLReportDataSet GenerateDashboard(GroupLReportSearchModel searchModel);
        List<string> GenerateDashboardReport(GroupLReportSearchModel searchModel);




    }
}
