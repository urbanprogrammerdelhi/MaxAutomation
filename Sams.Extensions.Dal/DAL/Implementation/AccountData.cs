using PdfDemo.Dal;
using Sams.Extensions.Model;
using Sams.Extensions.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Dal
{
    public class AccountData :BaseDataAccess, IAccountData
    {
        public Model.Account AccountByCredentials(string userName, string password)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "udp_ValidateUsers";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@userName", userName));
                command.Parameters.Add(new SqlParameter("@password", password));
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    return dt.Rows[0].ConvertFromDataRow<Account>();

                    return null;
            }
        }

        public IList<Model.Account> ListOfAccounts()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "udp_ValidateUsers";
                command.CommandType = CommandType.StoredProcedure;
               
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    return dt.ToList<Account>();

                return null;
            }
        }
    }
}
