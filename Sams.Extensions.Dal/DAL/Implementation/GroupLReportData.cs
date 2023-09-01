using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfDemo.Dal;
using Sams.Extensions.Model;
using Sams.Extensions.Utility;

namespace Sams.Extensions.Data
{
    public class GroupLReportData : BaseDataAccess, IGroupLReportData
    {
        private readonly Dictionary<GroupLReports, Func<GroupLReportSearchModel, DataTable>> GroupLDashboards = new Dictionary<GroupLReports, Func<GroupLReportSearchModel, DataTable>>();
        private readonly Dictionary<GroupLReports, Func<DataTable, List<string>>> GroupLDashboardReports = new Dictionary<GroupLReports, Func<DataTable, List<string>>>();

        private readonly Dictionary<GroupLReports, List<string[]>> GroupLRequiredFields = new Dictionary<GroupLReports, List<string[]>>();

        public GroupLReportData()
        {
            GroupLDashboards.Add(global::GroupLReports.PhotoDashboard, GeneratePhotoDashboard);
            GroupLDashboards.Add(global::GroupLReports.CheckListDashboard, GenerateChecklistDashboard);
            GroupLDashboards.Add(global::GroupLReports.RegisterDashboard, GenerateRegisterDashboard);
            GroupLRequiredFields.Add(global::GroupLReports.CheckListDashboard, new List<string[]> { ConfigurationFields.CheckListReportRequiredFields, ConfigurationFields.ChecListReportComparisionFields });
            GroupLRequiredFields.Add(global::GroupLReports.RegisterDashboard, new List<string[]> { ConfigurationFields.RegisterReportRequiredFields, ConfigurationFields.RegisterReportComparisionFields });
            GroupLRequiredFields.Add(global::GroupLReports.PhotoDashboard, new List<string[]> { ConfigurationFields.PhotoDashboardRequiredFields, ConfigurationFields.PhotoDashboardRequiredFields });
            GroupLDashboardReports.Add(global::GroupLReports.PhotoDashboard, GeneratePhotoDashboardReport);
            GroupLDashboardReports.Add(global::GroupLReports.CheckListDashboard, GenerateCheckListDashboardReport);
            GroupLDashboardReports.Add(global::GroupLReports.RegisterDashboard, GenerateRegisterDashboardReport);

        }

        public GroupLReportDataSet GenerateDashboard(GroupLReportSearchModel searchModel)
        {
            try
            {
                var currentMethod = GroupLDashboards[searchModel.CurrentReport];
                var dt = currentMethod.Invoke(searchModel);
                return new GroupLReportDataSet { ReportData = dt, CurrentReport = searchModel.CurrentReport, RequiredFields = GroupLRequiredFields[searchModel.CurrentReport][0], ComparisionFields = GroupLRequiredFields[searchModel.CurrentReport][1] };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

       

        private DataTable GenerateChecklistDashboard(GroupLReportSearchModel searchModel)
        {
            try
            {
                
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = "udp_GetChecklistDetailGroupL";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@LocationAutoId", searchModel.LocationAutoId));
                    command.Parameters.Add(new SqlParameter("@FromDate", searchModel.ReportDate));
                    command.Parameters.Add(new SqlParameter("@ToDate", searchModel.ReportDate));
                    command.Parameters.Add(new SqlParameter("@EmployeeNumber", searchModel.EmployeeNumber));
                    SqlDataAdapter adpt = new SqlDataAdapter();
                    adpt.SelectCommand = command;
                    var dt = new DataTable();
                    adpt.Fill(dt);
                    return dt;
                  
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        List<string> IGroupLReportData.GenerateDashboardReport(GroupLReportSearchModel searchModel)
        {
            try
            {
                var currentMethod = GroupLDashboards[searchModel.CurrentReport];
                var dt = currentMethod.Invoke(searchModel);
                var reportMetod = GroupLDashboardReports[searchModel.CurrentReport];
                var report = reportMetod.Invoke(dt);
                return report;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private DataTable GeneratePhotoDashboard(GroupLReportSearchModel searchModel)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = "udp_GetEmployeeAttendanceMIlkBasket";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@LocationAutoId", searchModel.LocationAutoId));
                    command.Parameters.Add(new SqlParameter("@FromDate", searchModel.ReportDate));
                    command.Parameters.Add(new SqlParameter("@ToDate", searchModel.ReportDate));
                    command.Parameters.Add(new SqlParameter("@EmployeeNumber", searchModel.EmployeeNumber));
                    SqlDataAdapter adpt = new SqlDataAdapter();
                    adpt.SelectCommand = command;
                    var dt = new DataTable();
                    adpt.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private DataTable GenerateRegisterDashboard(GroupLReportSearchModel searchModel)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = "udp_GetRegisterDetailGroupL";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@LocationAutoId", searchModel.LocationAutoId));
                    command.Parameters.Add(new SqlParameter("@FromDate", searchModel.ReportDate));
                    command.Parameters.Add(new SqlParameter("@ToDate", searchModel.ReportDate));
                    command.Parameters.Add(new SqlParameter("@EmployeeNumber", searchModel.EmployeeNumber));
                    SqlDataAdapter adpt = new SqlDataAdapter();
                    adpt.SelectCommand = command;
                    var dt = new DataTable();
                    adpt.Fill(dt);
                    return dt;
                   

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private List<string> GeneratePhotoDashboardReport(DataTable dt)
        {
            try
            {
                List<string> result = new List<string>();
                var photoDashboardData = dt.ToList<PhotoDashboardModel>().ToList();
                var splitedPhotoDashBoard = photoDashboardData.Split(15).ToList();
                int counter = 1;
                foreach (var photoDashBoard in splitedPhotoDashBoard)
                {
                    foreach (var item in photoDashBoard)
                    {
                        item.SlNo = counter;
                        counter++;
                    }
                }
              
                foreach (var item in splitedPhotoDashBoard)
                {
                    result.Add(item.ToList().GenerateHtmlTable(ConfigurationFields.PhotoDashboardRequiredFields));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private List<string> GenerateCheckListDashboardReport(DataTable dt)
        {
            try
            {
                List<String> result = new List<string>();
                var checkListData = dt.ToList<CheckListReportModel>();
                var splitCheckList = checkListData.Split(10);
                foreach (var checkList in splitCheckList)
                {
                    result.Add(checkList.ToList().GenerateHtmlTableV2(ConfigurationFields.CheckListReportRequiredFields, ConfigurationFields.ChecListReportComparisionFields));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private List<string> GenerateRegisterDashboardReport(DataTable dt)
        {
            try
            {
                List<String> result = new List<string>();
                var list = dt.ToList<RegisterDashboardModel>();
                var splitCheckList = list.Split(10);
                int counter = 1;

                foreach (var checkList in splitCheckList)
                {
                    foreach (var item in checkList)
                    {
                        item.SlNo = counter;
                        counter++;
                    }
                }
                foreach (var checkList in splitCheckList)
                {
                    result.Add(checkList.ToList().GenerateHtmlTableV2(ConfigurationFields.RegisterReportRequiredFields, ConfigurationFields.RegisterReportComparisionFields));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
