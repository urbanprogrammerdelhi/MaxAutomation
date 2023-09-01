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
    public interface IBranchCodeData
    {
        byte[] GetImageById(int id);

        List<BranchDetails> FetchClientCode(string ClientCode, string FromDate, string ToDate);

        PdfReportViewModel GetReportValues(string Location, string Branch, string AuditDate);
        List<ImageModel> FetchCheckListImageList(string location, string branch, string auditDate, string checkListId);
    }
   

 
}
