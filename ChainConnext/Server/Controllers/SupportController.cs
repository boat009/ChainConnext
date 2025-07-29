using ChainConnext.Shared.Payments;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ChainConnext.Shared.Supports;
using System.Collections.Generic;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class SupportController : ControllerBase
    {
        [HttpPost]
        public ExecResult GetDBServerName()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = true;
            Rs.Msg = BaseSettup.ServerName;
            Rs.JsonData = BaseShared.HostUrl;
            Rs.ID = BaseShared.AppVersion;
            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> GetFortnightYearWebPay()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                List<Fortnight_Info> list = new List<Fortnight_Info>();
                DataTable dataTable = new DataTable();
                using (SqlServerDataConnection sqlServerDataConnection = new SqlServerDataConnection())
                {
                    sqlServerDataConnection.SqlCommandType = CommandType.StoredProcedure;
                    sqlServerDataConnection.CommandString = "TSR_Application.dbo.Branch_Payment_GetFnYear";
                    dataTable = await sqlServerDataConnection.ExecuteQueryAsync();
                    Fortnight_Info fortnight_Info = new Fortnight_Info();

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(dataTable.Rows[i]["Fortnight_year"].ToString().Trim()))
                        {
                            fortnight_Info = new Fortnight_Info();
                            fortnight_Info.Fortnight_year = Convert.ToInt32(dataTable.Rows[i]["Fortnight_year"]);
                            fortnight_Info.Fortnight_year_TH = fortnight_Info.Fortnight_year + 543;
                            fortnight_Info.FnText = fortnight_Info.Fortnight_year_TH.ToString();
                            list.Add(fortnight_Info);
                        }
                    }
                }
                Rs.Data = list;
                Rs.Rows = list.Count;
                Rs.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> GetFortnightNoWeb()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                List<Fortnight_Info> list = new List<Fortnight_Info>();
                DataTable dataTable = new DataTable();
                using (SqlServerDataConnection sqlServerDataConnection = new SqlServerDataConnection())
                {
                    sqlServerDataConnection.SqlCommandType = CommandType.StoredProcedure;
                    sqlServerDataConnection.CommandString = "TSR_Application.dbo.NPT_Get_FnNo";
                    dataTable = await sqlServerDataConnection.ExecuteQueryAsync();
                    Fortnight_Info fortnight_Info = new Fortnight_Info();

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(dataTable.Rows[i]["Fortnight_no"].ToString().Trim()))
                        {
                            fortnight_Info = new Fortnight_Info();
                            fortnight_Info.Fortnight_no = Convert.ToInt32(dataTable.Rows[i]["Fortnight_no"]);
                            fortnight_Info.FnText = Convert.ToInt32(dataTable.Rows[i]["Fortnight_no"]).ToString();
                            list.Add(fortnight_Info);
                        }
                    }
                }
                Rs.Data = list;
                Rs.Rows = list.Count;
                Rs.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> GetNPTDetpart()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "TSR_Application.dbo.NPT_Depart_GetData";
                    dt = await sqlCon.ExecuteQueryAsync();
                    Rs.Msg = sqlCon.Message;
                }
                if (dt.Rows.Count > 0)
                {
                    Rs.Data = dt.ConvertTo<NPT_Depart>();
                }
                Rs.Rows = dt.Rows.Count;
                Rs.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }
    }
}
