using MaxAutomation.Data;
using MaxAutomation.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxAutomation.Dal.DAL
{
  
    public class MasterData:IMasterData
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public List<Location> FetchLocations(string companycode, string region)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "udp_GetBranchFromRegion";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@baseCompanyCode", companycode));
                command.Parameters.Add(new SqlParameter("@region", region));
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                return dt.ToList<Location>();
            }
        }
        public List<CompanyDetails> FetchCompanies()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "udp_GetCompanyDetails";
                command.CommandType = CommandType.StoredProcedure;
               
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                return dt.ToList<CompanyDetails>();
            }
        }

    }
}
