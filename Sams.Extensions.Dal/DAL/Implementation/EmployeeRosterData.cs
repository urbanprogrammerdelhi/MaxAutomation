using Sams.Extensions.Utility;
using Sams.Extensions.Dal;
using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using PdfDemo.Dal;

namespace Sams.Extensions.Dal
{


    public class EmployeeRosterData :BaseDataAccess, IEmployeeRoster
    {

       

        public Dictionary<string, DataTable> FetchEmployeeRosterData(ViewEmployeeRosterSearchModel employeeRosterSearchModel)
        {
            Dictionary<string, DataTable> output = new Dictionary<string, DataTable>();
            //List<string> RequiredFields = new List<string>();
            //for (int i = 1; i <= 31; i++)
            //{
            //    var field = new DateTime(2020, 01, i).ToString("dd");
            //    RequiredFields.Add(field);
            //}
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "UDP_APS_Search_EmployeeRosterDetails";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("Year", employeeRosterSearchModel.Year));

                command.Parameters.Add(new SqlParameter("Month", employeeRosterSearchModel.Month));
                command.Parameters.Add(new SqlParameter("LocationId", employeeRosterSearchModel.Location));



                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                List<ViewEmployeeRosterModel> employeeRosters = dt.ToList<ViewEmployeeRosterModel>();
                var floors = employeeRosters.ToLookup(emp => emp.Post);

                foreach (var floor in floors)
                {
                    DataTable result = new DataTable();
                    result.Columns.Add("Employee Code");
                    result.Columns.Add("Employee Name");
                    result.Columns.Add("Designation");
                    //foreach (var requiredField in RequiredFields)
                    //{
                    //    result.Columns.Add(requiredField);
                    //}
                    Console.WriteLine(floor.Key);
                    var rosterData = floor.ToPivotArray(item => item.RosterDay, item => item.EmployeeInformation, items => items.Any() ?
                              items.Select(it => it.Shift).ToList() : new List<String>());
                    var rosterDataFirstRow = ((IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>)rosterData.FirstOrDefault()).ToList().Skip(1);
                    foreach (var requiredField in rosterDataFirstRow)
                    {
                        result.Columns.Add(requiredField.Key);
                    }
                    for (int i = 0; i < rosterData.Length; i++)
                    {
                        DataRow dr = result.NewRow();

                        var obj = ((IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>)rosterData[i]).ToList();
                        var employeeRow = obj.FirstOrDefault();
                        var employeeDetails = obj.Skip(1);
                        string[] empInfo = employeeRow.Value.ParseToText().Split(',');
                        dr["Employee Code"] = empInfo[0];
                        dr["Employee Name"] = empInfo[1];
                        dr["Designation"] = empInfo[2];
                        foreach (var empDetails in  employeeDetails)
                        {


                            var monthData = empDetails.Value as System.Collections.Generic.List<String>;
                            dr[empDetails.Key] = (monthData != null && monthData.Count > 0) ? monthData[0].ParseToText() : string.Empty;

                        }
                        result.Rows.Add(dr);




                    }

                    output.Add($"{floor.Key}, {new DateTime(2020, 01, 01).ToString("MMMM yyyy")}", result);
                }

            }
            return output;
        }

        

        public int SaveEmployeeData(EmployeeRosterHeader employee)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "UDP_APS_UpsertAPSEmployeeDetails";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@EmployeeCode", employee.EmployeeCode));
                command.Parameters.Add(new SqlParameter("@EmployeeName", employee.EmployeeName));
                command.Parameters.Add(new SqlParameter("@Designation", employee.Designation));
                command.Parameters.Add(new SqlParameter("@LoggedInUser", employee.LoggedInPerson));
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    return dt.Rows[0][0].ParseToInteger();

                return -1;
            }
        }

        public string SaveEmployeeRosterData(EmployeeRosterDetails employeeRoster)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "UDP_APS_UpsertAPSEmployeeRosterDetails";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@RosterYear", employeeRoster.Year));
                command.Parameters.Add(new SqlParameter("@RosterMonth", employeeRoster.Month));
                command.Parameters.Add(new SqlParameter("@RosterDay", employeeRoster.Day));
                command.Parameters.Add(new SqlParameter("@LocationId", employeeRoster.LocationId));
                command.Parameters.Add(new SqlParameter("@Post", employeeRoster.Post));
                command.Parameters.Add(new SqlParameter("@Shift", employeeRoster.Shift));
                command.Parameters.Add(new SqlParameter("@LoggedInUser", employeeRoster.LoggedInUser));
                command.Parameters.Add(new SqlParameter("@EmployeeId", employeeRoster.EmployeeId));

                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = command;
                var dt = new DataTable();
                adpt.Fill(dt);
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    return dt.Rows[0][0].ParseToText();

                return "failed";
            }
        }
    }
}


