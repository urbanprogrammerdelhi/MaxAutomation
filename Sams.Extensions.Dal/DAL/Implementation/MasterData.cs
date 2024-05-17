using Sams.Extensions.Model;
using Sams.Extensions.Utility;
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

    public class MasterData : IMasterData
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
        public List<ReportDataModel> FetchReports(string companycode)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "udp_SearchReportsByCompany";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@CompanyCode", companycode));
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                return dt.ToList<ReportDataModel>();
            }
        }

        public List<Region> FetchRegions(string companycode)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "Search_Region_By_Company";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@CompanyCode", companycode));
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                return dt.ToList<Region>();
            }
        }

        public List<string> ListOfPosts()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "UDP_Search_Post";
                command.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                return dt.ToList<EmployeeRosterPostModel>().Select(pst=>pst.Post).ToList();
            }
        }

        public List<ClientModel> FetchClients(string locationId, DateTime? fromDate , DateTime? toDate)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "udp_GetClientListGroupL_1";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@LocationAutoID", locationId));
                command.Parameters.Add(new SqlParameter("@FromDate", fromDate));
                command.Parameters.Add(new SqlParameter("@ToDate", toDate));
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                return dt.ToList<ClientModel>();
            }
        }

        public List<SiteModel> FetchSites(string locationId, string clientCode, string companyCode)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "udp_GetSiteListGroupLNew";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@LocationAutoID", locationId));
                command.Parameters.Add(new SqlParameter("@ClientCode", clientCode));
                command.Parameters.Add(new SqlParameter("@BaseCompanyCode", companyCode));

                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                return dt.ToList<SiteModel>();
            }
        }
    }
}
