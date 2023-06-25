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
    public interface IBranchCodeData
    {
        byte[] GetImageById(int id);

        List<BranchDetails> FetchClientCode(string ClientCode, string FromDate, string ToDate);

        PdfReportViewModel GetReportValues(string Location, string Branch, string AuditDate);
    }
   

 
}
