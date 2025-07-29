using ChainConnext.Server.Helpers;
using ChainConnext.Shared.Reports;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using ChainConnext.Shared.Cms;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CmsController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> CardTransSave(Cms_Card_Trans C)
        {
            string json = JsonConvert.SerializeObject(C);
            Cms_Card_Trans xx = JsonConvert.DeserializeObject<Cms_Card_Trans>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {

                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    //sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;

                    sqlCon.CommandString = "Cms_Imp_CardTrans";
                    sqlCon.AddParameter("@ContNo", C.ContNo);
                    sqlCon.AddParameter("@RefNo", C.RefNo);
                    if (C.CustName != null)
                    {
                        sqlCon.AddParameter("@CustName", C.CustName);
                    }
                    if (C.AreaFrom != null)
                    {
                        sqlCon.AddParameter("@AreaFrom", C.AreaFrom);
                    }
                    if (C.AreaTo != null)
                    {
                        sqlCon.AddParameter("@AreaTo", C.AreaTo);
                    }
                    sqlCon.AddParameter("@CreateBy", C.CreateBy);

                    List<Cms_Card_Trans> Data = await sqlCon.ExecuteQueryListAsync<Cms_Card_Trans>();

                    //Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Rows = Data.Count;
                    Rs.JsonData = sqlCon.Message;
                    Rs.Msg = sqlCon.Message;
                    if (Rs.IsSuccess)
                    {
                        var rs = Data.FirstOrDefault();
                        if (rs != null)
                        {
                            if (!rs.Result)
                            {
                                Rs.IsSuccess = rs.Result;
                                Rs.Msg = rs.ResultMsg;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }
            if (!C.ForTest)
            {
                await SentDataLeakApi.SentApiFormat(C, "CardTransSave"
                        , Rs.Msg
                        , json
                        , "Cms_Card_Trans_Save");
            }

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> CardTransSaveApprove(Cms_Card_Trans C)
        {
            string json = JsonConvert.SerializeObject(C);
            Cms_Card_Trans xx = JsonConvert.DeserializeObject<Cms_Card_Trans>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {

                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;

                    sqlCon.CommandString = "Cms_Imp_CardTrans_With_Approve";
                    sqlCon.AddParameter("@ContNo", C.ContNo);
                    sqlCon.AddParameter("@RefNo", C.RefNo);
                    if (C.CustName != null)
                    {
                        sqlCon.AddParameter("@CustName", C.CustName);
                    }
                    if (C.AreaFrom != null)
                    {
                        sqlCon.AddParameter("@AreaFrom", C.AreaFrom);
                    }
                    if (C.AreaTo != null)
                    {
                        sqlCon.AddParameter("@AreaTo", C.AreaTo);
                    }
                    sqlCon.AddParameter("@CreateBy", C.CreateBy);

                    List<Cms_Card_Trans> Data = await sqlCon.ExecuteQueryListAsync<Cms_Card_Trans>();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Rows = Data.Count;
                    Rs.JsonData = sqlCon.Message;
                    Rs.Msg = sqlCon.Message;
                    if (Rs.IsSuccess)
                    {
                        var rs = Data.FirstOrDefault();
                        if (rs != null)
                        {
                            if (!rs.Result)
                            {
                                Rs.IsSuccess = rs.Result;
                                Rs.Msg = rs.ResultMsg;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }
            if (!C.ForTest)
            {
                await SentDataLeakApi.SentApiFormat(C, "CardTransSave"
                        , Rs.Msg
                        , json
                        , "Cms_Card_Trans_Save");
            }

            return Rs;
        }
    }
}
