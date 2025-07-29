using ChainConnext.Server.Helpers;
using ChainConnext.Shared.Reports;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using ChainConnext.Shared.Contracts;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class BHController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> RptImpPayment(Rpt_Parameter x)
        {
            string json = JsonConvert.SerializeObject(x);
            Rpt_Parameter xx = JsonConvert.DeserializeObject<Rpt_Parameter>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {

                if (x.DateFrom != null)
                {
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        if (x.ForTest)
                        {
                            sqlCon.CommandString = "Tmp_ReportDaily_Payment_List";
                            sqlCon.AddParameter("@TmpNo", x.WhereCond);
                        }
                        else
                        {
                            sqlCon.CommandString = "BH_Daily_Payment";
                            sqlCon.AddParameter("@DateFrom", x.DateFrom);
                        }

                        List<Tmp_ReportDaily_Payment> data = await sqlCon.ExecuteQueryListAsync<Tmp_ReportDaily_Payment>();
                        Rs.Rows = data.Count;
                        Rs.Data = data;

                        Rs.Msg = sqlCon.Message;
                        Rs.IsSuccess = sqlCon.IsSuccess;
                    }
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }
            if (!x.ForTest)
            {
                await SentDataLeakApi.SentApiFormat(x, "RptImpPayment"
                    , Rs.Msg
                    , json
                    , "Tmp_ReportDaily_Payment_List");
            }

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> RptPaymentSave(Tmp_ReportDaily_Payment x)
        {
            string json = JsonConvert.SerializeObject(x);
            Tmp_ReportDaily_Payment xx = JsonConvert.DeserializeObject<Tmp_ReportDaily_Payment>(json);
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
                    sqlCon.CommandString = "BH_Daily_Payment_Save";
                    sqlCon.AddParameter("@TmpID", x.TmpID);
                    sqlCon.AddParameter("@TmpNo", x.TmpNo);
                    sqlCon.AddParameter("@CreateBy", x.CreateBy);
                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }
            if (!x.ForTest)
            {
                await SentDataLeakApi.SentApiFormat(x, "RptPaymentSave"
                        , Rs.Msg
                        , json
                        , "BH_Daily_Payment_Save");
            }

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> RptPaymentErrorSave(Tmp_ReportDaily_Payment x)
        {
            string json = JsonConvert.SerializeObject(x);
            Tmp_ReportDaily_Payment xx = JsonConvert.DeserializeObject<Tmp_ReportDaily_Payment>(json);
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
                    sqlCon.CommandString = "Tmp_ReportDaily_Payment_Error_Save";
                    sqlCon.AddParameter("@TmpID", x.TmpID);
                    sqlCon.AddParameter("@TmpNo", x.TmpNo);
                    sqlCon.AddParameter("@InvNo", x.InvNo);
                    sqlCon.AddParameter("@IsError", x.IsError);
                    sqlCon.AddParameter("@ErrorMsg", x.ErrorMsg);
                    sqlCon.AddParameter("@ErrorBy", x.ErrorBy);
                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }
            if (!x.ForTest)
            {
                await SentDataLeakApi.SentApiFormat(x, "RptPaymentSave"
                        , Rs.Msg
                        , json
                        , "BH_Daily_Payment_Save");
            }

            return Rs;
        }

        [HttpPost]
        public async Task<Rpt_Parameter> RptImpPaymentDataTable(Rpt_Parameter x)
        {
            string json = JsonConvert.SerializeObject(x);
            Rpt_Parameter xx = JsonConvert.DeserializeObject<Rpt_Parameter>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            try
            {

                if (x.DateFrom != null)
                {
                    DataTable dt = new DataTable();
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        if (x.ForTest)
                        {
                            sqlCon.CommandString = "Tmp_ReportDaily_Payment_List";
                            sqlCon.AddParameter("@TmpNo", x.WhereCond);
                        }
                        else
                        {
                            sqlCon.CommandString = "Bighead_Mobile.dbo.usp_TSR_Migrate_GetReportDailyInvoiceA";
                            sqlCon.AddParameter("@Paydate", x.DateFrom);
                        }

                        dt = await sqlCon.ExecuteQueryAsync();

                        x.Msg = sqlCon.Message;
                    }

                    if (dt.Rows.Count > 0)
                    {
                        x.Data = BaseShared.DataTableToJson(dt);
                        x.DataList = dt.ConvertTo<Tmp_ReportDaily_Payment>();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            x.HeaderData.Add(dt.Columns[i].ColumnName.Trim());
                        }
                        if (x.IsCashCodeData)
                        {
                            dt = new DataTable();
                            using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                            {
                                sqlCon.SqlCommandType = CommandType.StoredProcedure;
                                sqlCon.CommandString = "TSRApp_Get_CashCode_Cms_Date";
                                sqlCon.AddParameter("@DateFrom", x.DateFrom);
                                dt = await sqlCon.ExecuteQueryAsync();
                            }
                            if (dt.Rows.Count > 0)
                            {
                                x.CashCodeData = dt.ConvertTo<Rpt_CashCode>();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                x.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "RptImpPaymentDataTable"
                , x.Msg
                , json
                , "Bighead_Mobile.dbo.usp_TSR_Migrate_GetReportDailyInvoiceA");

            return x;
        }

        [HttpPost]
        public Rpt_CashCode RptGetCashCode(Rpt_CashCode x)
        {
            string json = JsonConvert.SerializeObject(x);
            Rpt_Parameter xx = JsonConvert.DeserializeObject<Rpt_Parameter>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            Rpt_CashCode Rs = new Rpt_CashCode();
            try
            {

                if (x.DateFrom != null)
                {
                    if (x.RefNo != null)
                    {
                        if (!string.IsNullOrEmpty(x.RefNo.Trim()))
                        {
                            DataTable dt = new DataTable();
                            using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                            {
                                sqlCon.SqlCommandType = CommandType.StoredProcedure;
                                sqlCon.CommandString = "TSRApp_Get_CashCode_Cms";
                                sqlCon.AddParameter("@DateFrom", x.DateFrom);
                                sqlCon.AddParameter("@RefNo", x.RefNo);
                                if (x.ContNo != null)
                                {
                                    if (!string.IsNullOrEmpty(x.ContNo.Trim()))
                                    {
                                        sqlCon.AddParameter("@ContNo", x.ContNo);
                                    }
                                }
                                dt = sqlCon.ExecuteQuery();

                                Rs.Msg = sqlCon.Message;
                            }
                            Rs = dt.ConvertTo<Rpt_CashCode>().FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> RptImpPaymentListTmpNo(Tmp_ReportDaily_Payment x)
        {
            string json = JsonConvert.SerializeObject(x);
            Tmp_ReportDaily_Payment xx = JsonConvert.DeserializeObject<Tmp_ReportDaily_Payment>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Tmp_ReportDaily_Payment_List_TmpNo";

                    List<Tmp_ReportDaily_Payment> data = await sqlCon.ExecuteQueryListAsync<Tmp_ReportDaily_Payment>();
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
            if (!x.ForTest)
            {
                await SentDataLeakApi.SentApiFormat(x, "RptImpPaymentListTmpNo"
                        , Rs.Msg
                        , json
                        , "Tmp_ReportDaily_Payment_List_TmpNo");
            }
            return Rs;
        }
    }
}
