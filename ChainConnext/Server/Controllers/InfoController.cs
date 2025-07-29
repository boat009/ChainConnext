using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ChainConnext.Shared.Infos;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> ListAmphur([FromBody]Info_Amphur x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Info_Amphur_List";
                    if (x.Province_Code != null)
                    {
                        if (!string.IsNullOrEmpty(x.Province_Code.Trim()))
                        {
                            sqlCon.AddParameter("@Province_Code", x.Province_Code);
                        }
                    }

                    List<Info_Amphur> data = await sqlCon.ExecuteQueryListAsync<Info_Amphur>();
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
        public async Task<ExecResult> ListDistrict([FromBody] Info_District x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Info_District_List";
                    if (x.Province_Code != null)
                    {
                        if (!string.IsNullOrEmpty(x.Province_Code.Trim()))
                        {
                            sqlCon.AddParameter("@Province_Code", x.Province_Code);
                        }
                    }
                    if (x.Amphur_Code != null)
                    {
                        if (!string.IsNullOrEmpty(x.Amphur_Code.Trim()))
                        {
                            sqlCon.AddParameter("@Amphur_Code", x.Amphur_Code);
                        }
                    }

                    List<Info_District> data = await sqlCon.ExecuteQueryListAsync<Info_District>();
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
        public async Task<ExecResult> ListProvince()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Info_Province_List";

                    List<Info_Province> data = await sqlCon.ExecuteQueryListAsync<Info_Province>();
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
        public async Task<ExecResult> ListZipcode([FromBody] Info_Zipcode x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Info_Zipcode_List";
                    if (x.district_code != null)
                    {
                        if (!string.IsNullOrEmpty(x.district_code.Trim()))
                        {
                            sqlCon.AddParameter("@District_Code", x.district_code);
                        }
                    }

                    List<Info_Zipcode> data = await sqlCon.ExecuteQueryListAsync<Info_Zipcode>();
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
