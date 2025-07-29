using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ChainConnext.Shared.Payments;
using ChainConnext.Server.Helpers;
using System;
using ChainConnext.Shared.Infos;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> SaveMiniData([FromBody] Payment_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Payment_Info_Save_Mini";
                    if (x.PaymentId != null)
                    {
                        sqlCon.AddParameter("@PaymentId", x.PaymentId);
                    }
                    if (x.OrderId != null)
                    {
                        sqlCon.AddParameter("@OrderId", x.OrderId);
                    }
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    if (x.PayPeroid != 0)
                    {
                        sqlCon.AddParameter("@PayPeroid", x.PayPeroid);
                    }
                    sqlCon.AddParameter("@PayAmt", x.PayAmt);
                    if (x.PayDate != null)
                    {
                        sqlCon.AddParameter("@PayDate", x.PayDate);
                    }
                    if (x.PayTypeId != 0)
                    {
                        sqlCon.AddParameter("@PayTypeId", x.PayTypeId);
                    }
                    sqlCon.AddParameter("@IsActive", x.IsActive);
                    if (x.BookNo != null)
                    {
                        sqlCon.AddParameter("@BookNo", x.BookNo);
                    }
                    if (x.NumNo != null)
                    {
                        sqlCon.AddParameter("@NumNo", x.NumNo);
                    }
                    sqlCon.AddParameter("@PayNum", x.PayNum);
                    if (x.PayById != 0)
                    {
                        sqlCon.AddParameter("@PayById", x.PayById);
                    }
                    if (x.PayPlaceId != 0)
                    {
                        sqlCon.AddParameter("@PayPlaceId", x.PayPlaceId);
                    }
                    if (x.TransDate != null)
                    {
                        sqlCon.AddParameter("@TransDate", x.TransDate);
                    }
                    if (x.DocDate != null)
                    {
                        sqlCon.AddParameter("@DocDate", x.DocDate);
                    }
                    if (x.CashCode != null)
                    {
                        sqlCon.AddParameter("@CashCode", x.CashCode);
                    }
                    if (x.InvNo != null)
                    {
                        sqlCon.AddParameter("@InvNo", x.InvNo);
                    }
                    if (x.CashName != null)
                    {
                        sqlCon.AddParameter("@CashName", x.CashName);
                    }
                    if (x.ContractNoNew != null)
                    {
                        sqlCon.AddParameter("@ContractNoNew", x.ContractNoNew);
                    }
                    if (x.DepID != 0)
                    {
                        sqlCon.AddParameter("@DepID", x.DepID);
                    }
                    if (x.FnNo != 0)
                    {
                        sqlCon.AddParameter("@FnNo", x.FnNo);
                    }
                    if (x.FnYear != 0)
                    {
                        sqlCon.AddParameter("@FnYear", x.FnYear);
                    }
                    sqlCon.AddParameter("@CreatedBy", x.CreatedBy);
                    Rs.IsSuccess = await sqlCon.ExecuteNonQueryAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> PaymentList([FromBody] Payment_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Payment_Info_List";
                    sqlCon.AddParameter("@ContractId", x.ContractId);

                    List<Payment_Info> data = await sqlCon.ExecuteQueryListAsync<Payment_Info>();
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

            await SentDataLeakApi.SentApiFormat(x, "PaymentList"
                , Rs.Msg, $"@ContractId='{x.ContractId}'"
                , "Payment_Info_List");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> PaymentTransactionList([FromBody] Payment_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Payment_Transaction_List";
                    sqlCon.AddParameter("@ContractId", x.ContractId);

                    List<Payment_Transaction> data = await sqlCon.ExecuteQueryListAsync<Payment_Transaction>();
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

            await SentDataLeakApi.SentApiFormat(x, "PaymentTransactionList"
                , Rs.Msg, $"@ContractId='{x.ContractId}'"
                , "Payment_Transaction_List");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> PayByList()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Payment_PayBy_List";

                    List<Payment_PayBy> data = await sqlCon.ExecuteQueryListAsync<Payment_PayBy>();
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
        public async Task<ExecResult> PayPlaceList([FromBody] Payment_Place x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Payment_Place_List";
                    if (x.PayById != 0)
                    {
                        sqlCon.AddParameter("@PayById", x.PayById);
                    }

                    List<Payment_Place> data = await sqlCon.ExecuteQueryListAsync<Payment_Place>();
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
        public async Task<ExecResult> DeletePayment(Payment_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_MastPay_Delete";
                    sqlCon.AddParameter("@RefNo", x.RefNo);
                    sqlCon.AddParameter("@CONTNO", x.ContractNo);
                    sqlCon.AddParameter("@PAYPERIOD", x.PayPeroid);
                    sqlCon.AddParameter("@PAYDATE", x.PayDate);
                    sqlCon.AddParameter("@InvNo", x.InvNo);
                    sqlCon.AddParameter("@CreateBy", x.CreatedBy);
                    Rs.IsSuccess = await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.Msg = sqlCon.Message;
                }
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
