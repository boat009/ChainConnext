using ChainConnext.Server.Helpers;
using ChainConnext.Shared.StockDB;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using ChainConnext.Client.Pages.Contracts;
using ChainConnext.Shared.Contracts;
using ChainConnext.Shared.Toss;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TossController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> ProbOperationMainList(Contract_Info x)
        {
            string json = JsonConvert.SerializeObject(x);
            Contract_Info xx = JsonConvert.DeserializeObject<Contract_Info>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "TOSS_Prob_Operation_Main_List";
                    sqlCon.AddParameter("@RefNo", x.RefNo);

                    List<TOSS_Prob_Operation_Main> data = await sqlCon.ExecuteQueryListAsync<TOSS_Prob_Operation_Main>();
                    Rs.Rows = data.Count;
                    Rs.Data = data;

                    Rs.Msg = sqlCon.Message;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                }
                Rs.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "ProbOperationMainList"
                , Rs.Msg
                , json
                , "TOSS_Prob_Operation_Main_List");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> ProbOperationDetailList(TOSS_Prob_Operation_Main x)
        {
            string json = JsonConvert.SerializeObject(x);
            TOSS_Prob_Operation_Main xx = JsonConvert.DeserializeObject<TOSS_Prob_Operation_Main>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "TOSS_Prob_Operation_Detail_List";
                    sqlCon.AddParameter("@InformID", x.informId);

                    List<TOSS_Prob_Operation_Detail> data = await sqlCon.ExecuteQueryListAsync<TOSS_Prob_Operation_Detail>();
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

            await SentDataLeakApi.SentApiFormat(x, "ProbOperationDetailList"
                , Rs.Msg
                , json
                , "TOSS_Prob_Operation_Detail_List");

            return Rs;
        }
    }
}
