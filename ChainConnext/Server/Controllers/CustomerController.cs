using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ChainConnext.Shared.Customers;
using ChainConnext.Server.Helpers;
using Newtonsoft.Json;
using static FastReport.Barcode.Contact;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using ChainConnext.Shared.Contracts;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> ListTitle()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Customer_Title_List";

                    List<Customer_Title> data = await sqlCon.ExecuteQueryListAsync<Customer_Title>();
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
        public async Task<ExecResult> ListCustomerById([FromBody] Customer_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Customer_Info_GetById";
                    sqlCon.AddParameter("@CustomerId", x.CustomerId);

                    List<Customer_Info> data = await sqlCon.ExecuteQueryListAsync<Customer_Info>();
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

            await SentDataLeakApi.SentApiFormat(x, "ListCustomerById"
                , Rs.Msg, $"@CustomerId='{x.CustomerId}'"
                , "Customer_Info_GetById");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> ListCustomerByCitizenId([FromBody] Customer_Info x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Customer_Info_Find_CitizenId";
                    sqlCon.AddParameter("@CitizenId", x.CitizenId);

                    List<Customer_Info> data = await sqlCon.ExecuteQueryListAsync<Customer_Info>();
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

            await SentDataLeakApi.SentApiFormat(x, "ListCustomerByCitizenId"
                , Rs.Msg, $"@CitizenId='{x.CitizenId}'"
                , "Customer_Info_Find_CitizenId");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> SaveCustomerMini(Customer_Info x)
        {
            string json = JsonConvert.SerializeObject(x);
            Customer_Info xx = JsonConvert.DeserializeObject<Customer_Info>(json);
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
                    sqlCon.CommandString = "Customer_Info_Save_Mini";
                    sqlCon.AddParameter("@CitizenId", x.CitizenId);
                    sqlCon.AddParameter("@CitizenStartDate", x.CitizenStartDate);
                    sqlCon.AddParameter("@CitizenExpireDate", x.CitizenExpireDate);
                    sqlCon.AddParameter("@BirthDate", x.BirthDate);
                    sqlCon.AddParameter("@Title", x.Title);
                    sqlCon.AddParameter("@FirstName", x.FirstName);
                    sqlCon.AddParameter("@LastName", x.LastName);
                    sqlCon.AddParameter("@CreatedBy", x.CreatedBy);
                    sqlCon.AddParameter("@IsActive", x.IsActive);
                    sqlCon.AddParameter("@CardTypeId", x.CardTypeId);
                    sqlCon.AddParameter("@PrefixName", x.PrefixName);
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    sqlCon.AddParameter("@AuthorizeId", x.AuthorizeId);
                    DataTable dt = await sqlCon.ExecuteQueryAsync();

                    if (dt.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0]["CustomerId"].ToString().Trim()))
                        {
                            Rs.ID = dt.Rows[0]["CustomerId"].ToString().Trim();
                        }
                    }

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "SaveCustomerMini"
                , Rs.Msg, json
                , "Customer_Info_Save_Mini");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> SaveAddress(Customer_Address x)
        {
            string json = JsonConvert.SerializeObject(x);
            Customer_Address xx = JsonConvert.DeserializeObject<Customer_Address>(json);
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
                    sqlCon.CommandString = "Customer_Address_Save";
                    sqlCon.AddParameter("@AddressId", x.AddressId);
                    sqlCon.AddParameter("@CustomerId", x.CustomerId);
                    sqlCon.AddParameter("@AddressTypeId", x.AddressTypeId);
                    sqlCon.AddParameter("@AddressTitle", x.AddressTitle);
                    sqlCon.AddParameter("@AddressTitle2", x.AddressTitle2);
                    sqlCon.AddParameter("@AddressTitle3", x.AddressTitle3);
                    sqlCon.AddParameter("@AddressTitle4", x.AddressTitle4);
                    sqlCon.AddParameter("@AddressSubdistrict", x.AddressSubdistrict);
                    sqlCon.AddParameter("@AddressDistrict", x.AddressDistrict);
                    sqlCon.AddParameter("@AddressProvince", x.AddressProvince);
                    sqlCon.AddParameter("@AddressZipcode", x.AddressZipcode);
                    sqlCon.AddParameter("@Lat", x.Lat);
                    sqlCon.AddParameter("@Long", x.Long);
                    sqlCon.AddParameter("@CreatedBy", x.CreatedBy);
                    sqlCon.AddParameter("@IsActive", x.IsActive);
                    sqlCon.AddParameter("@AddressDate", x.AddressDate);
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    sqlCon.AddParameter("@AddressSubdistrict1", x.AddressSubdistrict1);
                    sqlCon.AddParameter("@AddressDistrict1", x.AddressDistrict1);
                    sqlCon.AddParameter("@AddressProvince1", x.AddressProvince1);
                    //sqlCon.AddParameter("@AddressZipcode1", x.AddressZipcode1);
                    //sqlCon.AddParameter("@BHRefNo", x.BHRefNo);
                    //sqlCon.AddParameter("@BHAddressID", x.BHAddressID);
                    //sqlCon.AddParameter("@DataDate", x.DataDate);
                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "SaveAddress"
                , Rs.Msg, json
                , "Customer_Address_Save");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> ListAddress([FromBody] Customer_Address x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Customer_Address_List";
                    sqlCon.AddParameter("@CustomerId", x.CustomerId);
                    if (x.AddressId != null)
                    {
                        if (!string.IsNullOrEmpty(x.AddressId.Trim()))
                        {
                            sqlCon.AddParameter("@AddressId", x.AddressId);
                        }
                    }

                    List<Customer_Address> data = await sqlCon.ExecuteQueryListAsync<Customer_Address>();
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

            await SentDataLeakApi.SentApiFormat(x, "ListAddress"
                , Rs.Msg, $"@CustomerId='{x.CustomerId}'"
                , "Customer_Address_List");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> SaveTelephone(Customer_Telephone x)
        {
            string json = JsonConvert.SerializeObject(x);
            Customer_Telephone xx = JsonConvert.DeserializeObject<Customer_Telephone>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Customer_Telephone_Save";
                    sqlCon.AddParameter("@CustomerId", x.CustomerId);
                    sqlCon.AddParameter("@TelTypeId", x.TelTypeId);
                    sqlCon.AddParameter("@TelNumber", x.TelNumber);
                    sqlCon.AddParameter("@CreatedBy", x.CreatedBy);
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    //sqlCon.AddParameter("@DataDate", x.DataDate);
                    Rs.IsSuccess = await sqlCon.ExecuteNonQueryAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "SaveTelephone"
                , Rs.Msg, json
                , "Customer_Telephone_Save");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> ListTelephone([FromBody] Customer_Telephone x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Customer_Telephone_List";
                    sqlCon.AddParameter("@CustomerId", x.CustomerId);
                    if (x.TelId != null)
                    {
                        if (!string.IsNullOrEmpty(x.TelId.Trim()))
                        {
                            sqlCon.AddParameter("@TelId", x.TelId);
                        }
                    }

                    List<Customer_Telephone> data = await sqlCon.ExecuteQueryListAsync<Customer_Telephone>();
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

            await SentDataLeakApi.SentApiFormat(x, "ListTelephone"
                , Rs.Msg, $"@CustomerId='{x.CustomerId}'"
                , "Customer_Telephone_List");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> ListAddressType()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Customer_Address_Type_List";

                    List<Customer_Address_Type> data = await sqlCon.ExecuteQueryListAsync<Customer_Address_Type>();
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
        public async Task<ExecResult> ListTelephoneType()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Customer_Telephone_Type_List";

                    List<Customer_Telephone_Type> data = await sqlCon.ExecuteQueryListAsync<Customer_Telephone_Type>();
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
        public async Task<ExecResult> ListCardType()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Customer_CardType_List";

                    List<Customer_CardType> data = await sqlCon.ExecuteQueryListAsync<Customer_CardType>();
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
