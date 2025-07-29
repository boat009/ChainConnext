using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        [HttpPost]
        public async Task<Authens> CheckLogout([FromBody] LogoutRequest m)
        {
            DataLeakApi dataLeak = new DataLeakApi();
            await dataLeak.SentApi(new DataLeakApi
            {
                UserCode = m.UserID,
                ApplicationName = BaseSettup.ApplicationName,
                ApplicationURL = BaseSettup.ApplicationURL,
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

            //using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
            //{
            //    sqlCon.SqlCommandType = CommandType.StoredProcedure;
            //    sqlCon.CommandString = "User_Log_Login_Add";
            //    sqlCon.AddParameter("@UsrID", A.UserID);
            //    //sqlCon.AddParameter("@UserPgVersion", A.UserID);
            //    sqlCon.AddParameter("@IsLogin", A.IsAuthen);
            //    //sqlCon.AddParameter("@OsVersion", A.UserID);
            //    //sqlCon.AddParameter("@Res_Width", A.UserID);
            //    //sqlCon.AddParameter("@Res_Height", A.UserID);
            //    //sqlCon.AddParameter("@McAddr", A.UserID);
            //    sqlCon.AddParameter("@Lattitude", m.Latt);
            //    sqlCon.AddParameter("@Longitude", m.Longtt);
            //    //sqlCon.AddParameter("@UsrName", m.UserName);
            //    //sqlCon.AddParameter("@UsrPass", m.Password);
            //    sqlCon.AddParameter("@Remark", A.AuthenMsg);
            //    sqlCon.ExecuteNonQuery();
            //}

            return A;
        }
    }
}
