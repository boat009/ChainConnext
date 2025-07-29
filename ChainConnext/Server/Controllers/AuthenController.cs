using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Helpers;
using ChainConnext.Client;
using ChainConnext.Shared.BD;
using System.Globalization;
using System;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        [HttpPost]
        public async Task<Authens> CheckLogin([FromBody] LoginRequest m)
        {
            BaseSettup.ApplicationURL = m.CurrentURL;
            Authens A = new Authens();
            A.IsAuthen = false;
            A.AuthenMsg = "Please Login";

            using (HttpClient Http = new HttpClient())
            {
                m.Password = PassEnDecrypt.Decrypt(m.Password);
                var postBody = new AuthenADRequest { username = m.UserName, password = m.Password };
                var response = await Http.PostAsJsonAsync(BaseSettup.ApiLogin, postBody);
                AuthenADData? Rs = await response.Content.ReadFromJsonAsync<AuthenADData>();
                if (Rs != null)
                {
                    A.AuthenMsg = $"Login Success : Status = {Rs.status} ,Message = {Rs.message}";
                    if (Rs.data != null)
                    {
                        if (Rs.data.Count > 0)
                        {
                            A.IsAuthen = true;
                            A.UserID = Rs.data[0].emp_id;
                            if (Rs.data[0].userprincipalname == null)
                            {
                                A.UserName = m.UserName;
                            }
                            else
                            {
                                A.UserName = Rs.data[0].userprincipalname;
                            }
                            A.FullName = Rs.data[0].displayname;
                            A.RememberMe = m.RememberMe;
                            A.LoginDate = DateTime.Now;
                            A.ClientID = m.ClientID;
                            A.ServerName = BaseSettup.ServerName;

                            using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                            {
                                sqlCon.SqlCommandType = CommandType.StoredProcedure;
                                sqlCon.CommandString = "User_Main_Save_Login";
                                sqlCon.AddParameter("@UsrID", A.UserID);
                                sqlCon.ExecuteNonQuery();
                            }

                            using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                            {
                                sqlCon.SqlCommandType = CommandType.StoredProcedure;
                                sqlCon.CommandString = "User_Main_List";
                                sqlCon.AddParameter("@UsrID", A.UserID);
                                var Um = sqlCon.ExecuteQuery().ConvertTo<User_Main>();
                                if (Um != null)
                                {
                                    if (Um.Count > 0)
                                    {
                                        A.Perms = Um.FirstOrDefault();
                                    }
                                }
                            }
                            using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                            {
                                sqlCon.SqlCommandType = CommandType.StoredProcedure;
                                sqlCon.CommandString = "User_Perms_List";
                                sqlCon.AddParameter("@UsrID", A.UserID);
                                A.PermsList = sqlCon.ExecuteQuery().ConvertTo<User_Perms>();
                            }
                            using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                            {
                                sqlCon.SqlCommandType = CommandType.StoredProcedure;
                                sqlCon.CommandString = "User_Menu_List";
                                sqlCon.AddParameter("@UsrID", A.UserID);
                                A.MenuList = sqlCon.ExecuteQuery().ConvertTo<User_Menu>();
                            }
                        }
                    }
                    else
                    {
                        A.IsAuthen = false;
                        A.AuthenMsg = $"Login Fail : Status = {Rs.status} ,Message = {Rs.message}";
                    }
                }
                else
                {
                    A.IsAuthen = false;
                    A.AuthenMsg = "Login Fail...";
                }
            }

            DataLeakApi dataLeak = new DataLeakApi();
            await dataLeak.SentApi(new DataLeakApi
            {
                UserCode = A.UserID,
                UserName = A.UserName,
                ApplicationName = BaseSettup.ApplicationName,
                ApplicationURL = BaseSettup.ApplicationURL,
                ServerIP = BaseSettup.ServerName,
                ServerName = BaseSettup.ServerName,
                UserDatabase = BaseSettup.DatabaseUserName,
                DataName = BaseSettup.DatabaseName,
                ActionName = "Login",
                ActionNote = A.AuthenMsg,
                ActionParameter = $"User = {m.UserName} ,Pass = {PassEnDecrypt.Encrypt(m.Password)}",
                ActionScript = BaseSettup.ApiLogin
            });

            //DataTable dt = new DataTable();
            //using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
            //{
            //    sqlCon.SqlCommandType = CommandType.StoredProcedure;
            //    sqlCon.CommandString = "User_Main_CheckLogin";
            //    sqlCon.AddParameter("@UsrName", m.UserName);
            //    sqlCon.AddParameter("@UsrPass", m.Password);
            //    dt = sqlCon.ExecuteQuery();
            //}
            //if (dt.Rows.Count > 0)
            //if(true)
            //{
            //    A.IsAuthen = true;
            //    //A.UserID = dt.Rows[0]["UsrID"].ToString().Trim();
            //    //A.UserName = dt.Rows[0]["UsrName"].ToString().Trim();
            //    //A.FullName = dt.Rows[0]["UsrFullName"].ToString().Trim();
            //    A.UserID = "UsrID";
            //    A.UserName = "UsrName";
            //    A.FullName = "UsrFullName";
            //    A.RememberMe = m.RememberMe;
            //    A.LoginDate = DateTime.Now;
            //    A.AuthenMsg = "Login Success";
            //    A.ClientID = m.ClientID;
            //}
            //else
            //{
            //    A.IsAuthen = false;
            //    A.AuthenMsg = "Login Fail...";
            //}

            return A;
        }
        [HttpPost]
        public async Task<Authens> Logout([FromBody] LoginRequest m)
        {
            DataLeakApi dataLeak = new DataLeakApi();
            await dataLeak.SentApi(new DataLeakApi
            {
                UserCode = m.UserID,
                UserName = m.UserName,
                ApplicationName = BaseSettup.ApplicationName,
                ApplicationURL = "",
                ServerIP = BaseSettup.ServerName,
                ServerName = BaseSettup.ServerName,
                UserDatabase = BaseSettup.DatabaseUserName,
                DataName = BaseSettup.DatabaseName,
                ActionName = "Logout"
            });
            Authens A = new Authens();
            A.UserID = m.UserID.ToString().Trim();
            A.IsAuthen = false;
            A.AuthenMsg = "Logout";

            using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
            {
                sqlCon.SqlCommandType = CommandType.StoredProcedure;
                sqlCon.CommandString = "User_Log_Login_Add";
                sqlCon.AddParameter("@UsrID", A.UserID);
                //sqlCon.AddParameter("@UserPgVersion", A.UserID);
                sqlCon.AddParameter("@IsLogin", A.IsAuthen);
                //sqlCon.AddParameter("@OsVersion", A.UserID);
                //sqlCon.AddParameter("@Res_Width", A.UserID);
                //sqlCon.AddParameter("@Res_Height", A.UserID);
                //sqlCon.AddParameter("@McAddr", A.UserID);
                sqlCon.AddParameter("@Lattitude", m.Latt);
                sqlCon.AddParameter("@Longitude", m.Longtt);
                sqlCon.AddParameter("@UsrName", m.UserName);
                sqlCon.AddParameter("@UsrPass", m.Password);
                sqlCon.AddParameter("@Remark", A.AuthenMsg);
                sqlCon.ExecuteNonQuery();
            }

            return A;
        }
        [HttpPost]
        public ExecResult GetEncPassWord([FromBody] LoginRequest m)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                if (m.UserName == "Encrypt")
                {
                    Rs.Msg = PassEnDecrypt.Encrypt(m.Password);
                    Rs.IsSuccess = true;
                }
                else if (m.UserName == "Decrypt")
                {
                    Rs.Msg = PassEnDecrypt.Decrypt(m.Password);
                    Rs.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> ListUser(User_Main x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "User_Main_List";
                    if (x.UsrID != null)
                    {
                        if (!string.IsNullOrEmpty(x.UsrID.Trim()))
                        {
                            sqlCon.AddParameter("@UsrID", x.UsrID);
                        }
                    }

                    List<User_Main> data = await sqlCon.ExecuteQueryListAsync<User_Main>();
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
        public async Task<ExecResult> ListMenuGroup()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Menu_Main_Group_List";

                    List<Menu_Main> data = await sqlCon.ExecuteQueryListAsync<Menu_Main>();
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
        public async Task<ExecResult> ListMenu(User_Menu x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Menu_Main_List";
                    if (x.MenuGroup != null)
                    {
                        if (!string.IsNullOrEmpty(x.MenuGroup.Trim()))
                        {
                            sqlCon.AddParameter("@MenuGroup", x.MenuGroup);
                        }
                    }

                    List<User_Menu> data = await sqlCon.ExecuteQueryListAsync<User_Menu>();
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
        public async Task<ExecResult> ListMenuSub(User_Menu x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Menu_Main_Sub_List";
                    sqlCon.AddParameter("@MainMenuId", x.MainMenuId);

                    List<User_Menu> data = await sqlCon.ExecuteQueryListAsync<User_Menu>();
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
        public async Task<ExecResult> ListUserMenu(User_Menu x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "User_Menu_List";
                    if (x.UsrID != null)
                    {
                        if (!string.IsNullOrEmpty(x.UsrID.Trim()))
                        {
                            sqlCon.AddParameter("@UsrID", x.UsrID);
                        }
                    }
                    if (x.MainMenuId > 0)
                    {
                        sqlCon.AddParameter("@MainMenuId", x.MainMenuId);
                    }

                    List<User_Menu> data = await sqlCon.ExecuteQueryListAsync<User_Menu>();
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
        public async Task<ExecResult> ListUserPerms(User_Perms x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "User_Perms_List";
                    if (x.UsrID != null)
                    {
                        if (!string.IsNullOrEmpty(x.UsrID.Trim()))
                        {
                            sqlCon.AddParameter("@UsrID", x.UsrID);
                        }
                    }

                    List<User_Perms> data = await sqlCon.ExecuteQueryListAsync<User_Perms>();
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
        public async Task<ExecResult> ListMainPerms()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "User_Perms_Main_List";

                    List<User_Perms> data = await sqlCon.ExecuteQueryListAsync<User_Perms>();
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
        public async Task<ExecResult> SavePerms(Authens x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.TransactionStart();

                    foreach (var item in x.PermsList)
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "User_Perms_Save";
                        sqlCon.AddParameter("@UsrID", item.UsrID);
                        sqlCon.AddParameter("@PermsName", item.PermsName);
                        sqlCon.AddParameter("@IsPerms", item.IsPerms);
                        sqlCon.AddParameter("@CreatedBy", item.CreatedBy);
                        await sqlCon.ExecuteNonQueryAsync();
                    }
                    foreach (var item in x.MenuList)
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "User_Menu_Save";
                        sqlCon.AddParameter("@UsrID", item.UsrID);
                        sqlCon.AddParameter("@MenuId", item.MenuId);
                        sqlCon.AddParameter("@IsAccess", item.IsAccess);
                        sqlCon.AddParameter("@IsSave", item.IsSave);
                        sqlCon.AddParameter("@IsDelete", item.IsDelete);
                        sqlCon.AddParameter("@CreatedBy", item.CreatedBy);
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

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> ListUserMenuCheck(User_Menu x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "User_Menu_List_Check";
                    sqlCon.AddParameter("@UsrID", x.UsrID);
                    sqlCon.AddParameter("@DataDate", x.DataDate);

                    List<User_Menu> data = await sqlCon.ExecuteQueryListAsync<User_Menu>();
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
        public async Task<ExecResult> ListUserPermsCheck(User_Perms x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "User_Perms_List_Check";
                    sqlCon.AddParameter("@UsrID", x.UsrID);
                    sqlCon.AddParameter("@DataDate", x.DataDate);

                    List<User_Perms> data = await sqlCon.ExecuteQueryListAsync<User_Perms>();
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
