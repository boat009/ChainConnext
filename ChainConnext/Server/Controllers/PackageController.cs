using ChainConnext.Shared;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Packages;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Policy;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> MainSave(Package_Main x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Package_Main_Save";
                    if (x.PackageId == null)
                    {
                        x.PackageId = Guid.NewGuid().ToString();
                    }
                    sqlCon.AddParameter("@PackageId", x.PackageId);
                    sqlCon.AddParameter("@ItemCode", x.ItemCode);
                    sqlCon.AddParameter("@BrandCode", x.BrandCode);
                    sqlCon.AddParameter("@CatCode", x.CatCode);
                    sqlCon.AddParameter("@CatSub1", x.CatSub1);
                    sqlCon.AddParameter("@CatSub2", x.CatSub2);
                    sqlCon.AddParameter("@ModelGroup", x.ModelGroup);
                    sqlCon.AddParameter("@PackageDesc", x.PackageDesc);
                    sqlCon.AddParameter("@PackagePrice", x.PackagePrice);
                    sqlCon.AddParameter("@PackageDiscount", x.PackageDiscount);
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
        public async Task<ExecResult> MainList(Package_Main x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Package_Main_List";
                    if (x.PackageId != null)
                    {
                        sqlCon.AddParameter("@PackageId", x.PackageId);
                    }

                    List<Package_Main> data = await sqlCon.ExecuteQueryListAsync<Package_Main>();
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
        public async Task<ExecResult> DetailSave(Package_Main_Detail x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Package_Main_Detail_Save";
                    if (x.PackageDetailId == null)
                    {
                        x.PackageDetailId = Guid.NewGuid().ToString();
                    }
                    sqlCon.AddParameter("@PackageDetailId", x.PackageDetailId);
                    sqlCon.AddParameter("@PackageId", x.PackageId);
                    sqlCon.AddParameter("@ModelCode", x.ModelCode);
                    sqlCon.AddParameter("@Peroid", x.Peroid);
                    sqlCon.AddParameter("@PeroidAmt", x.PeroidAmt);
                    sqlCon.AddParameter("@DiscountAmt", x.DiscountAmt);
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
        public async Task<ExecResult> DetailList(Package_Main_Detail x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "Package_Main_Detail_List";
                    if (x.PackageId != null)
                    {
                        sqlCon.AddParameter("@PackageId", x.PackageId);
                    }
                    if (x.PackageDetailId != null)
                    {
                        sqlCon.AddParameter("@PackageDetailId", x.PackageDetailId);
                    }

                    List<Package_Main> data = await sqlCon.ExecuteQueryListAsync<Package_Main>();
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
