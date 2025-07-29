using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class EditorLogController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> Save(Editor_Log x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Editor_Log_Save";
                    sqlCon.AddParameter("@FromValue", x.FromValue);
                    sqlCon.AddParameter("@ToValue", x.ToValue);
                    sqlCon.AddParameter("@EditRemark", x.EditRemark);
                    sqlCon.AddParameter("@EditBy", x.EditBy);
                    sqlCon.AddParameter("@AuthenBy", x.AuthenBy);
                    sqlCon.AddParameter("@ContractId", x.ContractId);
                    sqlCon.AddParameter("@ContNo", x.ContNo);
                    sqlCon.AddParameter("@RefNo", x.RefNo);
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
    }
}
