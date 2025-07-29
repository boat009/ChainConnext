using ChainConnext.Server.Helpers;
using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using ChainConnext.Shared.StockDB;
using ChainConnext.Shared.Payments;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class STKController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> ListProductTurnExchange(STK_ProductTurnExchange x)
        {
            string json = JsonConvert.SerializeObject(x);
            STK_ProductTurnExchange xx = JsonConvert.DeserializeObject<STK_ProductTurnExchange>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "STK_ProductTurnExchange_List";
                    sqlCon.AddParameter("@SerialNo", x.NewSerial);

                    List<STK_ProductTurnExchange> data = await sqlCon.ExecuteQueryListAsync<STK_ProductTurnExchange>();
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

            await SentDataLeakApi.SentApiFormat(x, "ListProductTurnExchange"
                , Rs.Msg
                , json
                , "STK_ProductTurnExchange_List");

            return Rs;
        }
    }
}
