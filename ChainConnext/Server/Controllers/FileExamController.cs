using ChainConnext.Server.Helpers;
using ChainConnext.Shared;
using ChainConnext.Shared.Cms;
using ChainConnext.Shared.Imps;
using ChainConnext.Shared.Reports;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class FileExamController : ControllerBase
    {
        [HttpPost]
        public ExecResult ImpStatus()
        {
            ExecResult Rs = new ExecResult();

            string MPath = @$"{AppDomain.CurrentDomain.BaseDirectory}file_exam\ExcelImportStatus.xlsx";
            Rs = ExcelHelper.ImportExcel(MPath);

            return Rs;
        }
        [HttpPost]
        public ExecResult LoadProModel()
        {
            ExecResult Rs = new ExecResult();

            string MPath = @$"{AppDomain.CurrentDomain.BaseDirectory}file_exam\Load_ProModel.xlsx";
            Rs = ExcelHelper.ImportExcel(MPath);

            return Rs;
        }
        [HttpPost]
        public ExecResult ImpCardTrans()
        {
            ExecResult Rs = new ExecResult();

            string MPath = @$"{AppDomain.CurrentDomain.BaseDirectory}file_exam\Imp_Card_Trans.xlsx";
            Rs = ExcelHelper.ImportExcel(MPath);

            return Rs;
        }
        [HttpPost]
        public ExecResult ImpBHCheck()
        {
            ExecResult Rs = new ExecResult();

            string MPath = @$"{AppDomain.CurrentDomain.BaseDirectory}file_exam\Data_BHCheck.xlsx";
            Rs = ExcelHelper.ImportExcels(MPath);

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> ImpBHCheckSave(ImpBHCheck C)
        {
            string json = JsonConvert.SerializeObject(C);
            ImpBHCheck xx = JsonConvert.DeserializeObject<ImpBHCheck>(json);
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

                    sqlCon.CommandString = "ImportBHCheck";
                    sqlCon.AddParameter("@RecvCode", C.ReceiptCode);
                    sqlCon.AddParameter("@Remark", C.Remark);
                    sqlCon.AddParameter("@PayPlace", C.PayPlace);
                    sqlCon.AddParameter("@TransDate", C.TransDate);
                    sqlCon.AddParameter("@BillRecvID", C.BillRecvID);
                    sqlCon.AddParameter("@CheckBy", C.CheckBy);
                    //await sqlCon.ExecuteNonQueryAsync();
                    //Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    //Rs.Msg = sqlCon.Message;

                    List<ImpBHCheck> Data = await sqlCon.ExecuteQueryListAsync<ImpBHCheck>();

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
                await SentDataLeakApi.SentApiFormat(C, "ImpBHCheckSave"
                        , Rs.Msg
                        , json
                        , "ImportBHCheck");
            }

            return Rs;
        }
    }
}
