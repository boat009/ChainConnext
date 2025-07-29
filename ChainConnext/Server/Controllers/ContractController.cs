using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ChainConnext.Shared.Contracts;
using ChainConnext.Server.Helpers;
using ChainConnext.Shared.Customers;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Reflection;
using ChainConnext.Shared.Reports;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text;
using ChainConnext.Shared.Authen;
using FastReport;
using Microsoft.CodeAnalysis.Operations;
using System;
using ChainConnext.Shared.Imps;
using System.Collections.Generic;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> FindData([FromBody] FindMode x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                if (x.FindValue != null)
                {
                    //DataTable dt = new DataTable();
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "Contract_Info_Find";
                        sqlCon.AddParameter("@Mode", x.FindID);
                        sqlCon.AddParameter("@Values", x.FindValue);
                        if (x.FindBy != null)
                        {
                            sqlCon.AddParameter("@FindBy", x.FindBy);
                        }

                        List<Contract_Info> data = await sqlCon.ExecuteQueryListAsync<Contract_Info>();
                        Rs.Rows = data.Count;
                        Rs.Data = data;

                        Rs.Msg = sqlCon.Message;
                        Rs.IsSuccess = sqlCon.IsSuccess;
                    }
                    //if (dt.Rows.Count > 0)
                    //{
                    //    //string Json = JsonConvert.SerializeObject(dt);
                    //    //var settings = new JsonSerializerSettings
                    //    //{
                    //    //    NullValueHandling = NullValueHandling.Ignore,
                    //    //    MissingMemberHandling = MissingMemberHandling.Ignore
                    //    //};
                    //    //List<Contract_Info> JsonData = JsonConvert.DeserializeObject<List<Contract_Info>>(Json, settings);
                    //    List<Contract_Info> Data = dt.ConvertTo<Contract_Info>();
                    //    for (int i = 0; i < dt.Rows.Count; i++)
                    //    {
                    //        if (Data[i].FirstName != Convert.ToString(dt.Rows[i]["FirstName"]))
                    //        {
                    //            Data[i].Title = Convert.ToInt32(dt.Rows[i]["Title"]);
                    //            Data[i].FirstName = Convert.ToString(dt.Rows[i]["FirstName"]);
                    //            Data[i].LastName = Convert.ToString(dt.Rows[i]["LastName"]);
                    //            Data[i].MissData = "Miss Data";
                    //        }
                    //    }

                    //    Rs.Data = Data;
                    //}

                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            //DataLeakApi dataLeak = new DataLeakApi();
            //await dataLeak.SentApi(new DataLeakApi
            //{
            //    UserCode = x.UserData.UserID,
            //    UserName = x.UserData.UserName,
            //    ApplicationName = BaseSettup.ApplicationName,
            //    ApplicationURL = x.UserData.AppUrl,
            //    ServerIP = BaseSettup.ServerName,
            //    ServerName = BaseSettup.ServerName,
            //    UserDatabase = BaseSettup.DatabaseUserName,
            //    DataName = BaseSettup.DatabaseName,
            //    ActionName = "FindData",
            //    ActionNote = Rs.Msg,
            //    ActionParameter = $"@Mode='{x.FindID}',@Values='{x.FindValue}',@FindBy={x.FindBy}",
            //    ActionScript = "Contract_Info_Find"
            //});
            await SentDataLeakApi.SentApiFormat(x, "FindData", Rs.Msg, $"@Mode='{x.FindID}',@Values='{x.FindValue}',@FindBy={x.FindBy}", "Contract_Info_Find");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> FindRefNoContNoData([FromBody] Contract_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            string find_refno = "";
            string find_contno = "";
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Find_RefNo_ContNo";
                    if (x.RefNo != null)
                    {
                        if (!string.IsNullOrEmpty(x.RefNo.Trim()))
                        {
                            find_refno = x.RefNo.Trim();
                            sqlCon.AddParameter("@RefNo", x.RefNo);
                        }
                    }
                    if (x.ContractNo != null)
                    {
                        if (!string.IsNullOrEmpty(x.ContractNo.Trim()))
                        {
                            find_contno = x.ContractNo.Trim();
                            sqlCon.AddParameter("@ContNo", x.ContractNo);
                        }
                    }
                    sqlCon.AddParameter("@FindBy", x.CreatedBy);

                    List<Contract_Info> data = await sqlCon.ExecuteQueryListAsync<Contract_Info>();
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
            await SentDataLeakApi.SentApiFormat(x, "FindRefNoContNoData"
                , Rs.Msg
                , $"@RefNo='{find_refno}',@ContNo='{find_contno}'"
                , "Contract_Info_Find_RefNo_ContNo");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> FindSaveNewData([FromBody] Contract_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            string find_refno = "";
            string find_contno = "";
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_FindSaveNew";
                    if (x.RefNo != null)
                    {
                        if (!string.IsNullOrEmpty(x.RefNo.Trim()))
                        {
                            find_refno = x.RefNo.Trim();
                            sqlCon.AddParameter("@RefNo", x.RefNo);
                        }
                    }
                    if (x.ContractNo != null)
                    {
                        if (!string.IsNullOrEmpty(x.ContractNo.Trim()))
                        {
                            find_contno = x.ContractNo.Trim();
                            sqlCon.AddParameter("@ContNo", x.ContractNo);
                        }
                    }
                    sqlCon.AddParameter("@FindBy", x.CreatedBy);

                    List<Contract_Info_New> data = await sqlCon.ExecuteQueryListAsync<Contract_Info_New>();
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
            await SentDataLeakApi.SentApiFormat(x, "FindSaveNewData"
                , Rs.Msg
                , $"@RefNo='{find_refno}',@ContNo='{find_contno}'"
                , "Contract_Info_FindSaveNew");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult>GetContractDetailData([FromBody] Contract_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                if (x.ContractId != null)
                {
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "Contract_Info_Find_ContractId";
                        sqlCon.AddParameter("@ContractId", x.ContractId);

                        List<Contract_Info> data = await sqlCon.ExecuteQueryListAsync<Contract_Info>();
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

            await SentDataLeakApi.SentApiFormat(x, "GetContractDetailData", Rs.Msg, $"@ContractId='{x.ContractId}'", "Contract_Info_Find_ContractId");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> GetContractEmp([FromBody] Contract_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Emp";
                    sqlCon.AddParameter("@ContractId", x.ContractId);

                    List<Contract_Info> data = await sqlCon.ExecuteQueryListAsync<Contract_Info>();
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

            DataLeakApi dataLeak = new DataLeakApi();
            await dataLeak.SentApi(new DataLeakApi
            {
                UserCode = x.UserData.UserID,
                UserName = x.UserData.UserName,
                ApplicationName = $"{BaseSettup.ApplicationName}({x.UserData.AppVersion})",
                ApplicationURL = x.UserData.AppUrl,
                ServerIP = BaseSettup.ServerName,
                ServerName = BaseSettup.ServerName,
                UserDatabase = BaseSettup.DatabaseUserName,
                DataName = BaseSettup.DatabaseName,
                ActionName = "GetContractEmp",
                ActionNote = Rs.Msg,
                ActionParameter = $"@ContractId='{x.ContractId}'",
                ActionScript = "Contract_Info_Emp"
            });

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> ListSerialNo(Contract_SerialNo x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_SerialNo_List";
                    sqlCon.AddParameter("@ContractId", x.ContractId);

                    List<Contract_SerialNo> data = await sqlCon.ExecuteQueryListAsync<Contract_SerialNo>();
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

            await SentDataLeakApi.SentApiFormat(x, "ListSerialNo"
                , Rs.Msg, $"@ContractId='{x.ContractId}'"
                , "Contract_SerialNo_List");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> CheckSerialNo(Contract_SerialNo x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_SerialNo_Check";
                    sqlCon.AddParameter("@SerialNo", x.SerialNo);
                    DataTable dt = await sqlCon.ExecuteQueryAsync();
                    Rs.Rows = dt.Rows.Count;
                    if (dt.Rows.Count > 0)
                    {
                        Rs.ID = dt.Rows[0]["Result"].ToString().Trim();
                    }

                    Rs.Msg = sqlCon.Message;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "CheckSerialNo"
                , Rs.Msg, $"@SerialNo='{x.SerialNo}'"
                , "Contract_SerialNo_Check");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> SaveSerialNo(Contract_SerialNo x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_SerialNo_Save";
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    sqlCon.AddParameter("@SerialNo", x.SerialNo);
                    sqlCon.AddParameter("@SerialNoTypeId", x.SerialNoTypeId);
                    sqlCon.AddParameter("@CreatedBy", x.CreatedBy);
                    if (x.SerialNoId != null)
                    {
                        if (!string.IsNullOrEmpty(x.SerialNoId.Trim()))
                        {
                            sqlCon.AddParameter("@SerialNoId", x.SerialNoId);
                        }
                    }
                    sqlCon.AddParameter("@SaveBD", true);
                    await sqlCon.ExecuteNonQueryAsync();
                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
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
        public async Task<ExecResult> DeleteSerialNo(Contract_SerialNo x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_SerialNo_Delete";
                    sqlCon.AddParameter("@SerialNoId", x.SerialNoId);
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    sqlCon.AddParameter("@SerialNo", x.SerialNo);
                    sqlCon.AddParameter("@DeleteBy", x.CreatedBy);
                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
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
        public async Task<ExecResult> ListSerialNoType()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_SerialNoType_List";

                    List<Contract_SerialNoType> data = await sqlCon.ExecuteQueryListAsync<Contract_SerialNoType>();
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
        public async Task<ExecResult> ListAging(Aging_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Aging_Info_List";
                    sqlCon.AddParameter("@ContractId", x.ContractId);

                    List<Aging_Info> data = await sqlCon.ExecuteQueryListAsync<Aging_Info>();
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

            await SentDataLeakApi.SentApiFormat(x, "ListAging"
                , Rs.Msg, $"@ContractId='{x.ContractId}'"
                , "Aging_Info_List");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> AgingDetail(Contract_Aging x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Aging_Detail";
                    sqlCon.AddParameter("@ContractId", x.ContractId);

                    List<Contract_Aging> data = await sqlCon.ExecuteQueryListAsync<Contract_Aging>();
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

            await SentDataLeakApi.SentApiFormat(x, "AgingDetail"
                , Rs.Msg, $"@ContractId='{x.ContractId}'"
                , "Contract_Aging_Detail");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> ListContractStatus()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Status_List";

                    List<Contract_Status> data = await sqlCon.ExecuteQueryListAsync<Contract_Status>();
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
        public async Task<ExecResult> UpdateContractStatus(Contract_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Update_Status";
                    sqlCon.AddParameter("@ContractNo", x.ContractNo);
                    sqlCon.AddParameter("@ContractStatus", x.ContractStatus);
                    sqlCon.AddParameter("@ContractStatusDate", x.ContractStatusDate);
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
        public async Task<ExecResult> UpdateContractStatusRefNo(Contract_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Update_Status_RefNo";
                    sqlCon.AddParameter("@RefNo", x.RefNo);
                    sqlCon.AddParameter("@ContractStatus", x.ContractStatus);
                    sqlCon.AddParameter("@ContractStatusDate", x.ContractStatusDate);
                    sqlCon.AddParameter("@CreatedBy", x.CreatedBy);
                    Rs.IsSuccess = await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
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
        public async Task<ExecResult> UpdateContractStatusContractNo(Contract_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Update_Status_ContractNo";
                    sqlCon.AddParameter("@ContractNo", x.ContractNo);
                    sqlCon.AddParameter("@ContractStatus", x.ContractStatus);
                    sqlCon.AddParameter("@ContractStatusDate", x.ContractStatusDate);
                    sqlCon.AddParameter("@CreatedBy", x.CreatedBy);
                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
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
        public async Task<ExecResult> PreContractStatusContractNo([FromBody] ImpStatus x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                if (x.ContractNo != null)
                {
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "Contract_Info_Update_Status_ContractNo_PreFilter";
                        sqlCon.AddParameter("@ContractNo", x.ContractNo);

                        List<ImpStatus> data = await sqlCon.ExecuteQueryListAsync<ImpStatus>();
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

            await SentDataLeakApi.SentApiFormat(x, "PreContractStatusContractNo", Rs.Msg, $"@ContractNo='{x.ContractNo}'", "Contract_Info_Update_Status_ContractNo_PreFilter");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> GetContractRef([FromBody] Contract_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                if (x.CustomerId != null)
                {
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "Contract_Info_Get_RefCont";
                        sqlCon.AddParameter("@CustomerId", x.CustomerId);

                        List<Contract_Info> data = await sqlCon.ExecuteQueryListAsync<Contract_Info>();
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

            await SentDataLeakApi.SentApiFormat(x, "GetContractRef", Rs.Msg, $"@CustomerId='{x.CustomerId}'", "Contract_Info_Get_RefCont");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> WebSave(Contract_Info x)
        {
            string json = JsonConvert.SerializeObject(x);
            Contract_Info xx = JsonConvert.DeserializeObject<Contract_Info>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_WebSave";
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    sqlCon.AddParameter("@CustomerId", x.CustomerId);
                    sqlCon.AddParameter("@ContractNo", x.ContractNo);
                    sqlCon.AddParameter("@RefNo", x.RefNo);
                    sqlCon.AddParameter("@Title", x.Title);
                    sqlCon.AddParameter("@FirstName", x.FirstName);
                    sqlCon.AddParameter("@LastName", x.LastName);
                    sqlCon.AddParameter("@Model", x.Model);
                    sqlCon.AddParameter("@ModelDesc", x.ModelDesc);
                    sqlCon.AddParameter("@SerialNo", x.SerialNo);
                    sqlCon.AddParameter("@EffDate", x.EffDate);
                    sqlCon.AddParameter("@ContractStatus", x.ContractStatus);
                    sqlCon.AddParameter("@ContractStatusDate", x.ContractStatusDate);
                    sqlCon.AddParameter("@BranchCode", x.BranchCode);
                    sqlCon.AddParameter("@BranchName", x.BranchName);
                    sqlCon.AddParameter("@ChanelCode", x.ChanelCode);
                    sqlCon.AddParameter("@ChanelName", x.ChanelName);
                    sqlCon.AddParameter("@OpenDate", x.OpenDate);
                    sqlCon.AddParameter("@CloseDate", x.CloseDate);
                    sqlCon.AddParameter("@Sales", x.Sales);
                    sqlCon.AddParameter("@Credit", x.Credit);
                    sqlCon.AddParameter("@ContractPeriod", x.ContractPeriod);
                    sqlCon.AddParameter("@ContractPeriodAmount", x.ContractPeriodAmount);
                    sqlCon.AddParameter("@ContractFirstPeriodAmount", x.ContractFirstPeriodAmount);
                    sqlCon.AddParameter("@ContractDiscount", x.ContractDiscount);
                    sqlCon.AddParameter("@AfterDiscount", x.AfterDiscount);
                    sqlCon.AddParameter("@ContractFirstPay", x.ContractFirstPay);
                    sqlCon.AddParameter("@HandNo", x.HandNo);
                    sqlCon.AddParameter("@UpdatedBy", x.UpdatedBy);
                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "WebSave"
                , Rs.Msg
                , json
                , "Contract_Info_WebSave");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> SaveNew(Contract_Info_New x)
        {
            string json = JsonConvert.SerializeObject(x);
            Rpt_Parameter xx = JsonConvert.DeserializeObject<Rpt_Parameter>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_SaveNew";
                    if (x.ContractId != null)
                    {
                        sqlCon.AddParameter("@ContractId", x.ContractId);
                    }
                    if (x.CustomerId != null)
                    {
                        sqlCon.AddParameter("@CustomerId", x.CustomerId);
                    }
                    sqlCon.AddParameter("@ContractNo", x.ContractNo);
                    sqlCon.AddParameter("@RefNo", x.RefNo);
                    if (x.EffDate != null)
                    {
                        sqlCon.AddParameter("@EffDate", x.EffDate);
                    }
                    if (x.FillterNo != null)
                    {
                        sqlCon.AddParameter("@FillterNo", x.FillterNo);
                    }
                    if (x.HandNo != null)
                    {
                        sqlCon.AddParameter("@HandNo", x.HandNo);
                    }
                    if (x.RefNoNew != null)
                    {
                        sqlCon.AddParameter("@RefNoNew", x.RefNoNew);
                    }
                    if (x.ContractStatus != null)
                    {
                        sqlCon.AddParameter("@ContractStatus", x.ContractStatus);
                    }
                    if (x.ContractStatusDate != null)
                    {
                        sqlCon.AddParameter("@ContractStatusDate", x.ContractStatusDate);
                    }
                    if (x.TypeCard != null)
                    {
                        sqlCon.AddParameter("@TypeCard", x.TypeCard);
                    }
                    if (x.CaseID != null)
                    {
                        sqlCon.AddParameter("@CaseID", x.CaseID);
                    }
                    if (x.BranchCode != null)
                    {
                        sqlCon.AddParameter("@BranchCode", x.BranchCode);
                    }
                    if (x.ContractSale != null)
                    {
                        sqlCon.AddParameter("@ContractSale", x.ContractSale);
                    }
                    if (x.SaleCode != null)
                    {
                        sqlCon.AddParameter("@SaleCode", x.SaleCode);
                    }
                    if (x.SaleName != null)
                    {
                        sqlCon.AddParameter("@SaleName", x.SaleName);
                    }
                    if (x.SetupCode != null)
                    {
                        sqlCon.AddParameter("@SetupCode", x.SetupCode);
                    }
                    if (x.SetupName != null)
                    {
                        sqlCon.AddParameter("@SetupName", x.SetupName);
                    }
                    if (x.CheckerCode != null)
                    {
                        sqlCon.AddParameter("@CheckerCode", x.CheckerCode);
                    }
                    if (x.CheckerName != null)
                    {
                        sqlCon.AddParameter("@CheckerName", x.CheckerName);
                    }
                    if (x.ServiceCode != null)
                    {
                        sqlCon.AddParameter("@ServiceCode", x.ServiceCode);
                    }
                    if (x.ServiceName != null)
                    {
                        sqlCon.AddParameter("@ServiceName", x.ServiceName);
                    }
                    if (x.CashCode != null)
                    {
                        sqlCon.AddParameter("@CashCode", x.CashCode);
                    }
                    if (x.CashName != null)
                    {
                        sqlCon.AddParameter("@CashName", x.CashName);
                    }
                    if (x.TeamCode != null)
                    {
                        sqlCon.AddParameter("@TeamCode", x.TeamCode);
                    }
                    if (x.ChanelCode != null)
                    {
                        sqlCon.AddParameter("@ChanelCode", x.ChanelCode);
                    }
                    if (x.NTDetail != null)
                    {
                        sqlCon.AddParameter("@NTDetail", x.NTDetail);
                    }
                    if (x.ChanelTele != null)
                    {
                        sqlCon.AddParameter("@ChanelTele", x.ChanelTele);
                    }

                    if (x.Title != 0)
                    {
                        sqlCon.AddParameter("@Title", x.Title);
                    }
                    if (x.FirstName != null)
                    {
                        sqlCon.AddParameter("@FirstName", x.FirstName);
                    }
                    if (x.LastName != null)
                    {
                        sqlCon.AddParameter("@LastName", x.LastName);
                    }
                    if (x.CardTypeId != 0)
                    {
                        sqlCon.AddParameter("@CardTypeId", x.CardTypeId);
                    }
                    if (x.CitizenId != null)
                    {
                        sqlCon.AddParameter("@CitizenId", x.CitizenId);
                    }
                    if (x.BirthDate != null)
                    {
                        sqlCon.AddParameter("@BirthDate", x.BirthDate);
                    }
                    if (x.CitizenExpireDate != null)
                    {
                        sqlCon.AddParameter("@CitizenExpireDate", x.CitizenExpireDate);
                    }
                    if (x.AddrReg4 != null)
                    {
                        sqlCon.AddParameter("@AddrReg4", x.AddrReg4);
                    }
                    if (x.AddrReg3 != null)
                    {
                        sqlCon.AddParameter("@AddrReg3", x.AddrReg3);
                    }
                    if (x.AddrReg2 != null)
                    {
                        sqlCon.AddParameter("@AddrReg2", x.AddrReg2);
                    }
                    if (x.AddrReg1 != null)
                    {
                        sqlCon.AddParameter("@AddrReg1", x.AddrReg1);
                    }
                    if (x.AddrRegZip != null)
                    {
                        sqlCon.AddParameter("@AddrRegZip", x.AddrRegZip);
                    }
                    if (x.TelNo != null)
                    {
                        sqlCon.AddParameter("@TelNo", x.TelNo);
                    }
                    if (x.Addr4 != null)
                    {
                        sqlCon.AddParameter("@Addr4", x.Addr4);
                    }
                    if (x.Addr3 != null)
                    {
                        sqlCon.AddParameter("@Addr3", x.Addr3);
                    }
                    if (x.Addr2 != null)
                    {
                        sqlCon.AddParameter("@Addr2", x.Addr2);
                    }
                    if (x.Addr1 != null)
                    {
                        sqlCon.AddParameter("@Addr1", x.Addr1);
                    }
                    if (x.AddrZip != null)
                    {
                        sqlCon.AddParameter("@AddrZip", x.AddrZip);
                    }
                    if (x.MobileNo != null)
                    {
                        sqlCon.AddParameter("@MobileNo", x.MobileNo);
                    }

                    if (x.AddrReg41 != null)
                    {
                        sqlCon.AddParameter("@AddrReg41", x.AddrReg41);
                    }
                    if (x.AddrReg31 != null)
                    {
                        sqlCon.AddParameter("@AddrReg31", x.AddrReg31);
                    }
                    if (x.AddrReg21 != null)
                    {
                        sqlCon.AddParameter("@AddrReg21", x.AddrReg21);
                    }
                    if (x.Addr41 != null)
                    {
                        sqlCon.AddParameter("@Addr41", x.Addr41);
                    }
                    if (x.Addr31 != null)
                    {
                        sqlCon.AddParameter("@Addr31", x.Addr31);
                    }
                    if (x.Addr21 != null)
                    {
                        sqlCon.AddParameter("@Addr21", x.Addr21);
                    }
                    if (x.SerialNo != null)
                    {
                        sqlCon.AddParameter("@SerialNo", x.SerialNo);
                    }
                    if (x.Model != null)
                    {
                        sqlCon.AddParameter("@Model", x.Model);
                    }
                    if (x.ModelDesc != null)
                    {
                        sqlCon.AddParameter("@ModelDesc", x.ModelDesc);
                    }
                    if (x.TypeCode != null)
                    {
                        sqlCon.AddParameter("@TypeCode", x.TypeCode);
                    }
                    if (x.ModelCode != null)
                    {
                        sqlCon.AddParameter("@ModelCode", x.ModelCode);
                    }

                    sqlCon.AddParameter("@Cash", x.Cash);
                    sqlCon.AddParameter("@ContractPeriod", x.ContractPeriod);
                    sqlCon.AddParameter("@ContractFirstPeriodAmount", x.ContractFirstPeriodAmount);
                    sqlCon.AddParameter("@AfterDiscount", x.AfterDiscount);
                    sqlCon.AddParameter("@Sales", x.Sales);
                    sqlCon.AddParameter("@ContractPeriodAmount", x.ContractPeriodAmount);
                    sqlCon.AddParameter("@ContractDiscount", x.ContractDiscount);
                    sqlCon.AddParameter("@ContractFirstPay", x.ContractFirstPay);
                    sqlCon.AddParameter("@Credit", x.Credit);

                    sqlCon.AddParameter("@PayPeroid", x.PayPeroid);
                    sqlCon.AddParameter("@PayDate", x.PayDate);
                    sqlCon.AddParameter("@BookNo", x.BookNo);
                    sqlCon.AddParameter("@NumNo", x.NumNo);
                    sqlCon.AddParameter("@PayAmt", x.PayAmt);

                    sqlCon.AddParameter("@CreatedBy", x.CreatedBy);

                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "SaveNew"
                , Rs.Msg
                , json
                , "Contract_Info_SaveNew");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> SaveDue(Contract_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Save_Due";
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    sqlCon.AddParameter("@ContractNo", x.ContractNo);
                    sqlCon.AddParameter("@RefNo", x.RefNo);
                    sqlCon.AddParameter("@CreditDate", x.CreditDate);
                    if (x.CreditFnNo != 0)
                    {
                        sqlCon.AddParameter("@CreditFnNo", x.CreditFnNo);
                    }
                    if (x.CreditFnYear != 0)
                    {
                        sqlCon.AddParameter("@CreditFnYear", x.CreditFnYear);
                    }
                    sqlCon.AddParameter("@UpdatedBy", x.UpdatedBy);
                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
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
        public async Task<ExecResult> SaveEmp(Contract_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Emp_Save";
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    sqlCon.AddParameter("@ContractNo", x.ContractNo);
                    sqlCon.AddParameter("@RefNo", x.RefNo);

                    sqlCon.AddParameter("@SaleCode", x.SaleCode);
                    sqlCon.AddParameter("@SaleName", x.SaleName);
                    sqlCon.AddParameter("@CashCode", x.CashCode);
                    sqlCon.AddParameter("@CashName", x.CashName);
                    sqlCon.AddParameter("@ServiceCode", x.ServiceCode);
                    sqlCon.AddParameter("@ServiceName", x.ServiceName);
                    sqlCon.AddParameter("@CheckerCode", x.CheckerCode);
                    sqlCon.AddParameter("@CheckerName", x.CheckerName);
                    sqlCon.AddParameter("@SetupCode", x.SetupCode);
                    sqlCon.AddParameter("@SetupName", x.SetupName);
                    sqlCon.AddParameter("@CollectorCode", x.CollectorCode);
                    sqlCon.AddParameter("@CollectorName", x.CollectorName);
                    if (x.TypeCard != null)
                    {
                        if (!string.IsNullOrEmpty(x.TypeCard.Trim()))
                        {
                            if (x.TypeCard != "-")
                            {
                                sqlCon.AddParameter("@TypeCard", x.TypeCard);
                            }
                        }
                    }
                    if (x.CaseID != null)
                    {
                        if (!string.IsNullOrEmpty(x.TypeCard.Trim()))
                        {
                            if (x.CaseID != "-")
                            {
                                sqlCon.AddParameter("@CaseID", x.CaseID);
                            }
                        }
                    }

                    if (x.CreditDate != null)
                    {
                        sqlCon.AddParameter("@CreditDate", x.CreditDate);
                        if (x.CreditFnNo != 0)
                        {
                            sqlCon.AddParameter("@CreditFnNo", x.CreditFnNo);
                        }
                        if (x.CreditFnYear != 0)
                        {
                            sqlCon.AddParameter("@CreditFnYear", x.CreditFnYear);
                        }
                    }
                    sqlCon.AddParameter("@UpdatedBy", x.UpdatedBy);
                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
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
        public async Task<ExecResult> ListAccStatus()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_List_AccStatus";

                    List<Contract_Info> data = await sqlCon.ExecuteQueryListAsync<Contract_Info>();
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
        public async Task<ExecResult> SaveCloneData(Contract_Info_Clone x)
        {
            string json = JsonConvert.SerializeObject(x);
            Contract_Info_Clone xx = JsonConvert.DeserializeObject<Contract_Info_Clone>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Clone";
                    sqlCon.AddParameter("@ContractId", x.ConInfo.ContractId);
                    sqlCon.AddParameter("@ContractNo", x.ConInfo.ContractNo);
                    sqlCon.AddParameter("@RefNo", x.ConInfo.RefNo);
                    if (x.ConInfo.OpenDate != null)
                    {
                        sqlCon.AddParameter("@OpenDate", x.ConInfo.OpenDate);
                    }
                    if (x.ConInfo.CloseDate != null)
                    {
                        sqlCon.AddParameter("@CloseDate", x.ConInfo.CloseDate);
                    }
                    sqlCon.AddParameter("@ContractStatus", x.ConInfo.ContractStatus);
                    if (x.ConInfo.ContractStatusDate != null)
                    {
                        sqlCon.AddParameter("@ContractStatusDate", x.ConInfo.ContractStatusDate);
                    }
                    sqlCon.AddParameter("@CashCode", x.ConInfo.CashCode);
                    sqlCon.AddParameter("@CashName", x.ConInfo.CashName);
                    sqlCon.AddParameter("@ContractType", x.ConInfo.ContractType);
                    sqlCon.AddParameter("@SerialNo", x.ConInfo.SerialNo);
                    sqlCon.AddParameter("@CashCodeOld", x.ConInfo.CashCodeOld);
                    sqlCon.AddParameter("@CashNameOld", x.ConInfo.CashNameOld);
                    sqlCon.AddParameter("@ContractNoOld", x.ConInfo.ContractNoOld);
                    sqlCon.AddParameter("@RefNoOld", x.ConInfo.RefNoOld);

                    sqlCon.AddParameter("@ContractIdNew", x.ConInfoNew.ContractId);
                    sqlCon.AddParameter("@ContractNoNew", x.ConInfoNew.ContractNo);
                    sqlCon.AddParameter("@RefNoNew", x.ConInfoNew.RefNo);
                    sqlCon.AddParameter("@OpenDateNew", x.ConInfoNew.OpenDate);
                    sqlCon.AddParameter("@CloseDateNew", x.ConInfoNew.CloseDate);
                    sqlCon.AddParameter("@ContractStatusNew", x.ConInfoNew.ContractStatus);
                    sqlCon.AddParameter("@ContractStatusDateNew", x.ConInfoNew.ContractStatusDate);
                    sqlCon.AddParameter("@ContractNoFrom", x.ConInfoNew.ContractNoFrom);
                    sqlCon.AddParameter("@RefNoFrom", x.ConInfoNew.RefNoFrom);
                    sqlCon.AddParameter("@TransferDate", x.ConInfoNew.TransferDate);
                    sqlCon.AddParameter("@AccStatusCode", x.ConInfoNew.AccStatusCode);
                    sqlCon.AddParameter("@AccStatusDate", x.ConInfoNew.AccStatusDate);
                    sqlCon.AddParameter("@ModelNew", x.ConInfoNew.Model);
                    sqlCon.AddParameter("@ModelDescNew", x.ConInfoNew.ModelDesc);
                    sqlCon.AddParameter("@CashNew ", x.ConInfoNew.Cash);
                    sqlCon.AddParameter("@ContractPeriodNew", x.ConInfoNew.ContractPeriod);
                    sqlCon.AddParameter("@ContractFirstPeriodAmountNew", x.ConInfoNew.ContractFirstPeriodAmount);
                    sqlCon.AddParameter("@AfterDiscountNew", x.ConInfoNew.AfterDiscount);
                    sqlCon.AddParameter("@SalesNew ", x.ConInfoNew.Sales);
                    sqlCon.AddParameter("@CreditNew", x.ConInfoNew.Credit);
                    sqlCon.AddParameter("@ContractPeriodAmountNew", x.ConInfoNew.ContractPeriodAmount);
                    sqlCon.AddParameter("@ContractDiscountNew", x.ConInfoNew.ContractDiscount);
                    sqlCon.AddParameter("@ContractFirstPayNew", x.ConInfoNew.ContractFirstPay);
                    sqlCon.AddParameter("@SerialNoNew", x.ConInfoNew.SerialNo);
                    sqlCon.AddParameter("@ContractTypeNew", x.ConInfoNew.ContractType);
                    sqlCon.AddParameter("@CashCodeNew", x.ConInfoNew.CashCode);
                    sqlCon.AddParameter("@CashNameNew", x.ConInfoNew.CashName);
                    sqlCon.AddParameter("@EffDateNew", x.ConInfoNew.EffDate);

                    sqlCon.AddParameter("@ToNote", x.ConInfoNew.ToNote);
                    sqlCon.AddParameter("@ContractIdLink", x.ConInfoNew.ContractIdLink);
                    sqlCon.AddParameter("@CreatedBy", x.CreatedBy);

                    await sqlCon.ExecuteNonQueryAsync();

                    foreach (var item in x.PayDataDelete)
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "Contract_Info_Clone_Delete_Payment";
                        sqlCon.AddParameter("@ContractId", x.ConInfo.ContractId);
                        sqlCon.AddParameter("@ContractNo", x.ConInfo.ContractNo);
                        sqlCon.AddParameter("@RefNo", x.ConInfo.RefNo);
                        sqlCon.AddParameter("@PaymentId", item.PaymentId);
                        sqlCon.AddParameter("@PayPeroid", item.PayPeroid);
                        sqlCon.AddParameter("@CreatedBy", x.CreatedBy);
                        await sqlCon.ExecuteNonQueryAsync();
                    }
                    foreach (var item in x.PayDataDeleteNew)
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "Contract_Info_Clone_Delete_Payment";
                        sqlCon.AddParameter("@ContractId", x.ConInfo.ContractId);
                        sqlCon.AddParameter("@ContractNo", x.ConInfo.ContractNo);
                        sqlCon.AddParameter("@RefNo", x.ConInfo.RefNo);
                        sqlCon.AddParameter("@PaymentId", item.PaymentId);
                        sqlCon.AddParameter("@PayPeroid", item.PayPeroid);
                        sqlCon.AddParameter("@CreatedBy", x.CreatedBy);
                        await sqlCon.ExecuteNonQueryAsync();
                    }

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "SaveCloneData"
                , Rs.Msg
                , json
                , "Contract_Info_Clone");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> CheckCloneData(Contract_Info x)
        {
            string json = JsonConvert.SerializeObject(x);
            Contract_Info xx = JsonConvert.DeserializeObject<Contract_Info>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = true;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Clone_Check";
                    sqlCon.AddParameter("@ContNo", x.ContractNo);
                    sqlCon.AddParameter("@RefNo", x.RefNo);
                    DataTable dt = await sqlCon.ExecuteQueryAsync();
                    if (dt.Rows.Count > 0)
                    {
                        Rs.IsSuccess = Convert.ToBoolean(dt.Rows[0]["Result"]);
                        Rs.Msg = dt.Rows[0]["ResultMsg"].ToString().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "CheckCloneData"
                , Rs.Msg
                , json
                , "Contract_Info_Clone_Check");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> SaveChangeData(Contract_Info_Clone x)
        {
            string json = JsonConvert.SerializeObject(x);
            Contract_Info_Clone xx = JsonConvert.DeserializeObject<Contract_Info_Clone>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Change";
                    sqlCon.AddParameter("@ContractId", x.ConInfo.ContractId);
                    sqlCon.AddParameter("@ContractNo", x.ConInfo.ContractNo);
                    sqlCon.AddParameter("@RefNo", x.ConInfo.RefNo);

                    sqlCon.AddParameter("@OldSerialNo", x.ConInfo.SerialNo);
                    sqlCon.AddParameter("@OldModel", x.ConInfo.Model);
                    sqlCon.AddParameter("@NewSerialNo", x.ConInfoNew.SerialNo);
                    sqlCon.AddParameter("@NewModel", x.ConInfoNew.Model);
                    sqlCon.AddParameter("@Serder", x.Serder);
                    sqlCon.AddParameter("@Remark", x.Remark);
                    sqlCon.AddParameter("@DocDate", x.ConInfoNew.ChangeDate);
                    sqlCon.AddParameter("@RemarkByte", Encoding.GetEncoding("windows-874").GetBytes(x.Remark));

                    sqlCon.AddParameter("@CreatedBy", x.CreatedBy);

                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "SaveCloneData"
                , Rs.Msg
                , json
                , "Contract_Info_Clone");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> SaveMemo(Contract_Info_Memo x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Memo_Save";
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    sqlCon.AddParameter("@Memo", x.Memo);
                    sqlCon.AddParameter("@CreatedBy", x.CreatedBy);
                    await sqlCon.ExecuteNonQueryAsync();
                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
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
        public async Task<ExecResult> GetMemo(Contract_Info_Memo x)
        {
            string json = JsonConvert.SerializeObject(x);
            Contract_Info_Memo xx = JsonConvert.DeserializeObject<Contract_Info_Memo>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = true;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Memo_Get";
                    sqlCon.AddParameter("@ContractId", x.ContractId);

                    List<Contract_Info_Memo> data = await sqlCon.ExecuteQueryListAsync<Contract_Info_Memo>();
                    Rs.Rows = data.Count;
                    Rs.Data = data.FirstOrDefault();

                    Rs.Msg = sqlCon.Message;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "CheckCloneData"
                , Rs.Msg
                , json
                , "Contract_Info_Clone_Check");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> ListFind(Contract_Info_Find x)
        {
            string json = JsonConvert.SerializeObject(x);
            Contract_Info_Find xx = JsonConvert.DeserializeObject<Contract_Info_Find>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                StringBuilder stbdQuery = new StringBuilder();

                if (x.RefNo != null)
                {
                    if (!string.IsNullOrEmpty(x.RefNo.Trim()))
                    {
                        stbdQuery.AppendFormat(" And Cinf.RefNo Like '{0}%' ", BaseShared.CheckInj(x.RefNo.Trim()));
                    }
                }
                if (x.ContractNo != null)
                {
                    if (!string.IsNullOrEmpty(x.ContractNo.Trim()))
                    {
                        stbdQuery.AppendFormat(" And Cinf.ContractNo Like '{0}%' ", BaseShared.CheckInj(x.ContractNo.Trim()));
                    }
                }
                if (x.CustomerName != null)
                {
                    if (!string.IsNullOrEmpty(x.CustomerName.Trim()))
                    {
                        stbdQuery.AppendFormat(" And Replace(Cust.CustomerNameNonTitile,' ','') Like '%'+Replace('{0}',' ','')+'%' ", BaseShared.CheckInj(x.CustomerName.Trim()));
                    }
                }
                if (x.CitizenId != null)
                {
                    if (!string.IsNullOrEmpty(x.CitizenId.Trim()))
                    {
                        stbdQuery.AppendFormat(" And Replace(Cust.CitizenId,' ','') Like '%'+Replace('{0}',' ','')+'%' ", BaseShared.CheckInj(x.CitizenId.Trim()));
                    }
                }
                if (x.BranchCode != null)
                {
                    if (!string.IsNullOrEmpty(x.BranchCode.Trim()))
                    {
                        stbdQuery.AppendFormat(" And Cinf.BranchCode='{0}' ", BaseShared.CheckInj(x.BranchCode.Trim()));
                    }
                }
                if (x.SerialNo != null)
                {
                    if (!string.IsNullOrEmpty(x.SerialNo.Trim()))
                    {
                        stbdQuery.AppendFormat(" And Cinf.SerialNo Like '%{0}%' ", BaseShared.CheckInj(x.SerialNo.Trim()));
                    }
                }
                if (x.EffDate != null)
                {
                    string RDateF = string.Format(new CultureInfo("en-US", true), "{0:dd/MM/yyyy}", Convert.ToDateTime(x.EffDate.Value.ToShortDateString()));
                    if (x.EffDateTo != null)
                    {
                        string RDateT = string.Format(new CultureInfo("en-US", true), "{0:dd/MM/yyyy}", Convert.ToDateTime(x.EffDateTo.Value.ToShortDateString()));
                        if (BaseShared.CheckCompareDate(Convert.ToDateTime(x.EffDate.Value.ToShortDateString())
                            , Convert.ToDateTime(x.EffDateTo.Value.ToShortDateString())))
                        {
                            stbdQuery.AppendFormat(" And Convert(Date,Cinf.EffDate,103)=Convert(Date,'{0}',103) ", RDateF);
                        }
                        else
                        {
                            stbdQuery.AppendFormat(" And Convert(Date,Cinf.EffDate,103) Between Convert(Date,'{0}',103) And  Convert(Date,'{1}',103) "
                                , RDateF, RDateT);
                        }
                    }
                    else
                    {
                        stbdQuery.AppendFormat(" And Convert(Date,Cinf.EffDate,103)=Convert(Date,'{0}',103) ", RDateF);
                    }
                }

                string Query = stbdQuery.ToString().Trim();
                Rs.JsonData = Query;
                if (!string.IsNullOrEmpty(Query.Trim()))
                {
                    Query = Query.Replace("'", "''");
                    string MQuery = string.Format("Exec Contract_Info_List_Find '{0}';", Query);

                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.Text;
                        sqlCon.CommandString = MQuery;

                        List<Contract_Info_Find> data = await sqlCon.ExecuteQueryListAsync<Contract_Info_Find>();
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

            await SentDataLeakApi.SentApiFormat(x, "ListFind"
                , Rs.Msg
                , json
                , "Contract_Info_List_Find");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> EditRefNoContNo(Contract_Info x)
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
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Save_RefNo_ContNo";
                    sqlCon.AddParameter("@Mode", x.EditContNoRefNoType);
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    sqlCon.AddParameter("@ContractNo", x.ContractNo);
                    sqlCon.AddParameter("@RefNo", x.RefNo);
                    sqlCon.AddParameter("@UpdatedBy", x.CreatedBy);

                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "EditRefNoContNo"
                , Rs.Msg
                , json
                , "Contract_Info_Save_RefNo_ContNo");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> DeleteCont(Contract_Info x)
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
                    sqlCon.TransactionStart();

                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Contract_Info_Delete";
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    sqlCon.AddParameter("@ContractNo", x.ContractNo);
                    sqlCon.AddParameter("@RefNo", x.RefNo);
                    sqlCon.AddParameter("@DeleteBy", x.CreatedBy);

                    List<ExecResult> data = await sqlCon.ExecuteQueryListAsync<ExecResult>();
                    Rs.Rows = data.Count;
                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    if (data.Count > 0)
                    {
                        Rs.Msg = data.FirstOrDefault().Msg;
                    }
                    else
                    {
                        Rs.Msg = sqlCon.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "DeleteCont"
                , Rs.Msg
                , json
                , "Contract_Info_Delete");

            return Rs;
        }
    }
}
