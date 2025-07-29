using ChainConnext.Shared;
using ChainConnext.Shared.ARs;
using ChainConnext.Shared.BD;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ARController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> IRRCal(IRR_Contract_Cal x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.CommandString = "Select * From dbo.IRR_Contract_Cal(@Credit,@Sales,@Peroid,@PeroidAmt,@FirstPeroidAmt,@NetCredit,@Discount)";
                    sqlCon.AddParameter("@Credit", x.Credit);
                    sqlCon.AddParameter("@Sales", x.Sales);
                    sqlCon.AddParameter("@Peroid", x.Peroid);
                    sqlCon.AddParameter("@PeroidAmt", x.PeroidAmt);
                    sqlCon.AddParameter("@FirstPeroidAmt", x.FirstPeroidAmt);
                    sqlCon.AddParameter("@NetCredit", x.NetCredit);
                    sqlCon.AddParameter("@Discount", x.Discount);
                    List<IRR_Contract_Cal> data = await sqlCon.ExecuteQueryListAsync<IRR_Contract_Cal>();
                    Rs.Rows = data.Count;
                    Rs.Data = data;

                    Rs.Msg = sqlCon.Message;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> IRRCalDetail(IRR_Contract_Cal x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.CommandString = "Select * From dbo.IRR_Contract_Cal_Detail(@Credit,@Sales,@Peroid,@PeroidAmt,@FirstPeroidAmt,@NetCredit,@Discount)";
                    sqlCon.AddParameter("@Credit", x.Credit);
                    sqlCon.AddParameter("@Sales", x.Sales);
                    sqlCon.AddParameter("@Peroid", x.Peroid);
                    sqlCon.AddParameter("@PeroidAmt", x.PeroidAmt);
                    sqlCon.AddParameter("@FirstPeroidAmt", x.FirstPeroidAmt);
                    sqlCon.AddParameter("@NetCredit", x.NetCredit);
                    sqlCon.AddParameter("@Discount", x.Discount);
                    List<IRR_Contract_Cal_Detail> data = await sqlCon.ExecuteQueryListAsync<IRR_Contract_Cal_Detail>();
                    Rs.Rows = data.Count;
                    Rs.Data = data;

                    Rs.Msg = sqlCon.Message;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> IRRCalDiff(IRR_Contract_Cal x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.CommandString = "Select * From dbo.IRR_Contract_Cal_Diff(@Credit,@Sales,@Peroid,@PeroidAmt,@FirstPeroidAmt,@NetCredit,@Discount)";
                    sqlCon.AddParameter("@Credit", x.Credit);
                    sqlCon.AddParameter("@Sales", x.Sales);
                    sqlCon.AddParameter("@Peroid", x.Peroid);
                    sqlCon.AddParameter("@PeroidAmt", x.PeroidAmt);
                    sqlCon.AddParameter("@FirstPeroidAmt", x.FirstPeroidAmt);
                    sqlCon.AddParameter("@NetCredit", x.NetCredit);
                    sqlCon.AddParameter("@Discount", x.Discount);
                    List<IRR_Contract_Cal> data = await sqlCon.ExecuteQueryListAsync<IRR_Contract_Cal>();
                    Rs.Rows = data.Count;
                    Rs.Data = data;

                    Rs.Msg = sqlCon.Message;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> IRRCalDetailDiff(IRR_Contract_Cal x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.CommandString = "Select * From dbo.IRR_Contract_Cal_Detail(@Credit,@Sales,@Peroid,@PeroidAmt,@FirstPeroidAmt,@NetCredit,@Discount)";
                    sqlCon.AddParameter("@Credit", x.Credit);
                    sqlCon.AddParameter("@Sales", x.Sales);
                    sqlCon.AddParameter("@Peroid", x.Peroid);
                    sqlCon.AddParameter("@PeroidAmt", x.PeroidAmt);
                    sqlCon.AddParameter("@FirstPeroidAmt", x.FirstPeroidAmt);
                    sqlCon.AddParameter("@NetCredit", x.NetCredit);
                    sqlCon.AddParameter("@Discount", x.Discount);
                    List<IRR_Contract_Cal_Detail> data = await sqlCon.ExecuteQueryListAsync<IRR_Contract_Cal_Detail>();
                    Rs.Rows = data.Count;
                    Rs.Data = data;

                    Rs.Msg = sqlCon.Message;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }
    }
}
