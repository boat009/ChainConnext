using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ChainConnext.Shared.TSRApps;
using ChainConnext.Shared.Toss;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TSRAppController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> FindEmp(TSRApp_EmpData x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "TSRApp_Emp_Find";
                    sqlCon.AddParameter("@EmpCode", x.empid);

                    List<TSRApp_EmpData> data = await sqlCon.ExecuteQueryListAsync<TSRApp_EmpData>();
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
