using PdfDemo.Dal;
using Sams.Extensions.Dal;
using Sams.Extensions.Model;
using Sams.Extensions.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Sams.Extensions.Dal
{


    public class BranchCodeData:BaseDataAccess, IBranchCodeData
    {
        public byte[] GetImageById(int id,bool forCheckListId=false)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string tableName = forCheckListId ? "MaxLifeChecklistImageMaster" : "MaxBupaChecklistImageMasterUpdated";
                string sql = "SELECT Image FROM "+ tableName+" WHERE ImageAutoId =" + id.ToString();
                using (SqlDataAdapter sda = new SqlDataAdapter(sql, con))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
                        return null;
                    return (byte[])dt.Rows[0]["Image"];

                }
            }
        }
        public  List<BranchDetails> FetchClientCode(string ClientCode, string FromDate, string ToDate)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))

            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "udp_GetClientCodeMaxReportNew";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@LocationAutoId", ClientCode));
                command.Parameters.Add(new SqlParameter("@FromDate", FromDate));
                command.Parameters.Add(new SqlParameter("@ToDate", ToDate));
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                var list = dt.ToList<BranchDetails>();
                int counter = 1;
                foreach (var lst in list)
                {
                    lst.SerialNumber = counter;
                    counter++;
                }
                return list;

            }
        }

        public  PdfReportViewModel GetReportValues(string Location, string Branch, string AuditDate)
        {
            try
            {
                PdfReportViewModel result = new PdfReportViewModel();
                result.AuditDate = AuditDate;
                result.Branch = Branch;
                result.Location = Location;
                using (SqlConnection connection = new SqlConnection(ConnectionString))

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = "udp_GetMaxReportValuesUpdated_New";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Clientcode", Branch));

                    command.Parameters.Add(new SqlParameter("@LocationautoId", Location));

                    command.Parameters.Add(new SqlParameter("@FromDate", AuditDate));

                    SqlDataAdapter adpt = new SqlDataAdapter();
                    adpt.SelectCommand = command;
                    DataSet ds = new DataSet();
                    adpt.Fill(ds);
                    if (ds != null && ds.Tables.Count > 1)
                    {
                        var header = ds.Tables[1].ToList<Reportheader>().FirstOrDefault();
                        header.AuditDate = AuditDate;
                        result.Header = header;
                        var detail = ds.Tables[0].ToList<ReportBody>();
                        result.MasterdetailList = detail.ToLookup(lkp => lkp.MainHeader);// + "," + lkp.HeaderIndex);
                        result.ColumnDetails = detail.FirstOrDefault();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;

        }

        public List<ImageModel> FetchCheckListImageList(string location,string branch,string auditDate,string checkListId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))

            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "[SP_FetchICheckListImageList]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@LocationAutoId", location));
                command.Parameters.Add(new SqlParameter("@FromDate", auditDate));
                command.Parameters.Add(new SqlParameter("@ClientCode", branch));
                command.Parameters.Add(new SqlParameter("@CheckListId", checkListId));

                
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                return dt.ToList<ImageModel>();
               

            }
        }

        public List<BranchDetails> FetchClientCodeV2(string ClientCode, string FromDate, string ToDate,string checkistType)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))

            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "udp_FetchBranchGroupDetails";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@LocationAutoId", ClientCode));
                command.Parameters.Add(new SqlParameter("@FromDate", FromDate));
                command.Parameters.Add(new SqlParameter("@ToDate", ToDate));
                command.Parameters.Add(new SqlParameter("@ChecklistType", checkistType));

                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                var list = dt.ToList<BranchDetails>();
                int counter = 1;
                foreach (var lst in list)
                {
                    lst.SerialNumber = counter;
                    counter++;
                }
                return list;

            }
        }

        public PdfReportViewModel GetReportValuesV2(string Location, string Branch, string AuditDate, string checkistType)
        {
            try
            {
                PdfReportViewModel result = new PdfReportViewModel();
                result.AuditDate = AuditDate;
                result.Branch = Branch;
                result.Location = Location;
                result.ChecklistType = checkistType;
                using (SqlConnection connection = new SqlConnection(ConnectionString))

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = "udp_GetMaxReportValuesUpdatedV2";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Clientcode", Branch));
                    command.Parameters.Add(new SqlParameter("@LocationautoId", Location));
                    command.Parameters.Add(new SqlParameter("@FromDate", AuditDate));
                    command.Parameters.Add(new SqlParameter("@ChecklistType", checkistType));


                    SqlDataAdapter adpt = new SqlDataAdapter();
                    adpt.SelectCommand = command;
                    DataSet ds = new DataSet();
                    adpt.Fill(ds);
                    if (ds != null && ds.Tables.Count > 1)
                    {
                        var header = ds.Tables[1].ToList<Reportheader>().FirstOrDefault();
                        header.AuditDate = AuditDate;
                        result.Header = header;
                        var detail = ds.Tables[0].ToList<ReportBody>();
                        result.MasterdetailList = detail.ToLookup(lkp => lkp.MainHeader);// + "," + lkp.HeaderIndex);
                        result.ColumnDetails = detail.FirstOrDefault();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public List<CheckListTypeModel> FetchChecklistTypes()
        {

                 using (SqlConnection connection = new SqlConnection(ConnectionString))

            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "udp_FetchMaxLifeCheckListTypes";
                command.CommandType = CommandType.StoredProcedure;
              
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                return dt.ToList<CheckListTypeModel>();
               

            }
        }
    }
}


 