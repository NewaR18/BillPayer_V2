using Azure;
using BillPay.DataAccess.Data;
using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models.ViewModels.Home;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository
{
    public class HomeRepo : IHomeRepo
    {
        private readonly AppDbContext _context;

        private readonly IConfiguration _configuration;
        public HomeRepo(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public DashboardElements GetDashboardElements(string userId)
        {
            try
            {
                DashboardElements dashboardElements = new DashboardElements();
                var conStr = _configuration.GetConnectionString("Myconnection");
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GET_DASHBOARD_ELEMENTS";
                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.Add("@USER_ID", SqlDbType.VarChar).Value = userId;
                        using (SqlDataReader sd = cmd.ExecuteReader())
                        {
                            if (sd.HasRows)
                            {
                                while (sd.Read())
                                {
                                    decimal expenses;
                                    decimal totalDue;
                                    decimal toReceive;
                                    dashboardElements.Expenses = decimal.TryParse(sd["EXPENSES"].ToString(), out expenses) == true ? expenses : 0;
                                    dashboardElements.TotalDue = decimal.TryParse(sd["TOTAL_DUE"].ToString(), out totalDue) == true ? totalDue : 0;
                                    dashboardElements.LastPaymentDate = DBNull.Value.Equals(sd["PAYMENT_DATE"]) ? "1999-09-09" : Convert.ToDateTime(sd["PAYMENT_DATE"]).ToString("yyyy-MM-dd");
                                    dashboardElements.ToReceive = decimal.TryParse(sd["TO_RECEIVE"].ToString(), out toReceive) == true ? toReceive : 0;
                                }
                            }
                        }
                    }
                }
                return dashboardElements;
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<LineChart> GetLineTrend(string userId)
        {
            try
            {
                List<LineChart> lineCharts = new List<LineChart>();
                var conStr = _configuration.GetConnectionString("Myconnection");
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "USP_USER_EXPENSE_CHART";
                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.Add("@USER_ID", SqlDbType.VarChar).Value = userId;
                        using (SqlDataReader sd = cmd.ExecuteReader())
                        {
                            if (sd.HasRows)
                            {
                                while (sd.Read())
                                {
                                    LineChart lineChart = new LineChart();
                                    lineChart.Date = sd["DATE"] == null ? "" : sd["DATE"].ToString()!;
                                    lineChart.Amount = sd["AMT"] == null ? "" : sd["AMT"].ToString()!;
                                    lineCharts.Add(lineChart);
                                }
                            }
                        }
                    }
                }
                return lineCharts;
            }
            catch
            {
                throw;
            }
        }
    }
}
