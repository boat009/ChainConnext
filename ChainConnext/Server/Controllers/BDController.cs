using BlazorStrap.V5;
using ChainConnext.Server.Helpers;
using ChainConnext.Shared;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Contracts;
using ChainConnext.Shared.Supports;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class BDController : ControllerBase
    {
        [HttpPost]
        public async Task<ExecResult> NewRefNoClone()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                DataTable dt = new DataTable();
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_MastCont_NewRefNo_Clone";
                    dt = await sqlCon.ExecuteQueryAsync();
                    Rs.Msg = sqlCon.Message;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                }
                if (dt.Rows.Count > 0)
                {
                    Rs.Data = dt.Rows[0]["RefNo"].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> ListProModel()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "ProModel_List";

                    List<ProModel> data = await sqlCon.ExecuteQueryListAsync<ProModel>();
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
        public async Task<ExecResult> ListKindDesc()
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_ProModel_ListKindDesc";

                    List<BDProModel> data = await sqlCon.ExecuteQueryListAsync<BDProModel>();
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
        public async Task<ExecResult> ListProKind(BDProKind x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_ProKind_List";
                    if (x.code != null)
                    {
                        if (!string.IsNullOrEmpty(x.code.Trim()))
                        {
                            sqlCon.AddParameter("@code", x.code);
                        }
                    }

                    List<BDProKind> data = await sqlCon.ExecuteQueryListAsync<BDProKind>();
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
        public async Task<ExecResult> SaveProKind(BDProKind x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_ProKind_Save";
                    sqlCon.AddParameter("@code", x.code);
                    sqlCon.AddParameter("@name", x.name);
                    sqlCon.AddParameter("@CreateBy", x.CreateBy);
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
        public async Task<ExecResult> DeleteProKind(BDProKind x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_ProKind_Delete";
                    sqlCon.AddParameter("@code", x.code);
                    sqlCon.AddParameter("@name", x.name);
                    sqlCon.AddParameter("@CreateBy", x.CreateBy);

                    List<ExecResult> data = await sqlCon.ExecuteQueryListAsync<ExecResult>();
                    Rs.Rows = data.Count;
                    Rs.Data = data;

                    Rs.Msg = data.FirstOrDefault().Msg;
                    Rs.IsSuccess = data.FirstOrDefault().IsSuccess;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> ListBDProModel(BDProModel x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_ProModel_List";
                    if (x.cate != null)
                    {
                        if (!string.IsNullOrEmpty(x.cate.Trim()))
                        {
                            sqlCon.AddParameter("@Cate", x.cate);
                        }
                    }
                    if (x.MODEL != null)
                    {
                        if (!string.IsNullOrEmpty(x.MODEL.Trim()))
                        {
                            sqlCon.AddParameter("@MODEL", x.MODEL);
                        }
                    }

                    List<BDProModel> data = await sqlCon.ExecuteQueryListAsync<BDProModel>();
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
        public async Task<ExecResult> SaveBDProModel(BDProModel x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_ProModel_Save";
                    sqlCon.AddParameter("@MODEL", x.MODEL);
                    sqlCon.AddParameter("@Des", x.Des);
                    sqlCon.AddParameter("@status", x.status);
                    if (x.stdate != null)
                    {
                        sqlCon.AddParameter("@stdate", x.stdate);
                    }
                    sqlCon.AddParameter("@CASH", x.CASH);
                    sqlCon.AddParameter("@Sales", x.Sales);
                    sqlCon.AddParameter("@CREDIT", x.CREDIT);
                    sqlCon.AddParameter("@credit2", x.credit2);
                    sqlCon.AddParameter("@MODE", x.MODE);
                    sqlCon.AddParameter("@InvDesc", x.InvDesc);
                    sqlCon.AddParameter("@vatdesc", x.vatdesc);
                    sqlCon.AddParameter("@rate", x.rate);
                    sqlCon.AddParameter("@Cate", x.cate);
                    sqlCon.AddParameter("@BaseRate", x.BaseRate);
                    sqlCon.AddParameter("@ErpItemCode", x.ErpItemCode);
                    sqlCon.AddParameter("@KindDesc", x.KindDesc);
                    sqlCon.AddParameter("@WPrice", x.WPrice);
                    sqlCon.AddParameter("@CreateBy", x.CreateBy);
                    if (x.ProStartDate != null)
                    {
                        sqlCon.AddParameter("@ProStartDate", x.ProStartDate);
                    }
                    if (x.ProEndDate != null)
                    {
                        sqlCon.AddParameter("@ProEndDate", x.ProEndDate);
                    }
                    sqlCon.AddParameter("@RemarkDesc", x.Remark);
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
        public async Task<ExecResult> DeleteBDProModel(BDProModel x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_ProModel_Delete";
                    sqlCon.AddParameter("@MODEL", x.MODEL);
                    sqlCon.AddParameter("@CreateBy", x.CreateBy);

                    List<ExecResult> data = await sqlCon.ExecuteQueryListAsync<ExecResult>();
                    Rs.Rows = data.Count;
                    Rs.Data = data;

                    Rs.Msg = data.FirstOrDefault().Msg;
                    Rs.IsSuccess = data.FirstOrDefault().IsSuccess;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> GetProModelByID(ProModel x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "ProModel_GetByID";
                    sqlCon.AddParameter("@MODEL", x.MODEL);

                    List<ProModel> data = await sqlCon.ExecuteQueryListAsync<ProModel>();
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

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> GetProModelFind(BDProModel x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                if (x.ModelDesc != null)
                {
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "BD_ProModel_List";
                        sqlCon.AddParameter("@ModelDesc", x.ModelDesc);

                        List<BDProModel> data = await sqlCon.ExecuteQueryListAsync<BDProModel>();
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

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> GetProModelFindModel(BDProModel x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                if (x.MODEL != null)
                {
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "BD_ProModel_List";
                        sqlCon.AddParameter("@MODEL", x.MODEL);

                        List<BDProModel> data = await sqlCon.ExecuteQueryListAsync<BDProModel>();
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

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> ListErr(MasterErr x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "MasterErr_List";
                    sqlCon.AddParameter("@kind", x.kind);

                    List<MasterErr> data = await sqlCon.ExecuteQueryListAsync<MasterErr>();
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
        public async Task<ExecResult> ListChanel(Chanel x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_Chanel_List";
                    if (x.ChanelDep != 0)
                    {
                        sqlCon.AddParameter("@DepID", x.ChanelDep);
                    }
                    if (x.code != null)
                    {
                        if (!string.IsNullOrEmpty(x.code.Trim()))
                        {
                            sqlCon.AddParameter("@Code", x.code);
                        }
                    }
                    if (x.id != 0)
                    {
                        sqlCon.AddParameter("@id", x.id);
                    }

                    List<Chanel> data = await sqlCon.ExecuteQueryListAsync<Chanel>();
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
        public async Task<ExecResult> ListChanelDep(Chanel x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_Chanel_Dep_List";

                    List<NPT_Depart> data = await sqlCon.ExecuteQueryListAsync<NPT_Depart>();
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
        public async Task<ExecResult> SaveChanel(Chanel x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_Chanel_Save";
                    sqlCon.AddParameter("@id", x.id);
                    sqlCon.AddParameter("@code", x.code);
                    sqlCon.AddParameter("@name", x.name);
                    sqlCon.AddParameter("@ChanelTag", x.ChanelTag);
                    sqlCon.AddParameter("@ChanelMap", x.ChanelMap);
                    sqlCon.AddParameter("@Used", x.Used);
                    sqlCon.AddParameter("@aline", x.aline);
                    sqlCon.AddParameter("@ateam", x.ateam);
                    sqlCon.AddParameter("@ChanelDep", x.ChanelDep);
                    sqlCon.AddParameter("@teamno", x.teamno);
                    sqlCon.AddParameter("@CreateBy", x.CreateBy);
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
        public async Task<ExecResult> DeleteChanel(Chanel x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_Chanel_Delete";
                    sqlCon.AddParameter("@id", x.id);
                    sqlCon.AddParameter("@CreateBy", x.CreateBy);
                    Rs.IsSuccess = await sqlCon.ExecuteNonQueryAsync();
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
        [HttpPost]
        public async Task<ExecResult> ListCArea(CArea x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_CArea_List";
                    if (x.CCode != null)
                    {
                        if (!string.IsNullOrEmpty(x.CCode.Trim()))
                        {
                            sqlCon.AddParameter("@CCode", x.CCode);
                        }
                    }
                    if (x.ID != 0)
                    {
                        sqlCon.AddParameter("@ID", x.ID);
                    }

                    List<CArea> data = await sqlCon.ExecuteQueryListAsync<CArea>();
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
        public async Task<ExecResult> ListCAreaDep(CArea x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_CArea_List_Dep";

                    List<CArea> data = await sqlCon.ExecuteQueryListAsync<CArea>();
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
        public async Task<ExecResult> SaveCArea(CArea x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_CArea_Save";
                    sqlCon.AddParameter("@ID", x.ID);
                    sqlCon.AddParameter("@EmpId", x.EmpId);
                    sqlCon.AddParameter("@Name", x.Name);
                    sqlCon.AddParameter("@ACode", x.ACode);
                    sqlCon.AddParameter("@CCode", x.CCode);
                    sqlCon.AddParameter("@MCode", x.MCode);
                    sqlCon.AddParameter("@Department", x.Department);
                    sqlCon.AddParameter("@CreateBy", x.CreateBy);
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
        public async Task<ExecResult> DeleteCArea(CArea x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_CArea_Delete";
                    sqlCon.AddParameter("@ID", x.ID);
                    sqlCon.AddParameter("@CreateBy", x.CreateBy);
                    Rs.IsSuccess = await sqlCon.ExecuteNonQueryAsync();
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
        [HttpPost]
        public async Task<ExecResult> ListBranch(Branch x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_Branch_List";
                    if (x.Code != null)
                    {
                        if (!string.IsNullOrEmpty(x.Code.Trim()))
                        {
                            sqlCon.AddParameter("@Code", x.Code);
                        }
                    }

                    List<Branch> data = await sqlCon.ExecuteQueryListAsync<Branch>();
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
        public async Task<ExecResult> FindSaleName(BDSaleName x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_SaleName_Find";
                    sqlCon.AddParameter("@SaleCode", x.SaleCode);
                    sqlCon.AddParameter("@FindMode", x.SFindMode);

                    List<BDSaleName> data = await sqlCon.ExecuteQueryListAsync<BDSaleName>();
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

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> ListTranSt(TranSt x)
        {
            string json = JsonConvert.SerializeObject(x);
            TranSt xx = JsonConvert.DeserializeObject<TranSt>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_TranSt_List";
                    sqlCon.AddParameter("@RefNo", x.RefNo);

                    List<TranSt> data = await sqlCon.ExecuteQueryListAsync<TranSt>();
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

            await SentDataLeakApi.SentApiFormat(x, "ListTranSt"
                , Rs.Msg
                , json
                , "BD_TranSt_List");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> ListPayTrans(BD_InvoiceABH x)
        {
            string json = JsonConvert.SerializeObject(x);
            BD_InvoiceABH xx = JsonConvert.DeserializeObject<BD_InvoiceABH>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                bool is_Search = false;

                if (x.RefNo != null)
                {
                    if (!string.IsNullOrEmpty(x.RefNo.Trim()))
                    {
                        is_Search = true;
                    }
                }
                if (x.ContNo != null)
                {
                    if (!string.IsNullOrEmpty(x.ContNo.Trim()))
                    {
                        is_Search = true;
                    }
                }
                if (x.InvNo != null)
                {
                    if (!string.IsNullOrEmpty(x.InvNo.Trim()))
                    {
                        is_Search = true;
                    }
                }

                if (is_Search)
                {
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "BD_vw_InvoiceABH_List";
                        if (x.RefNo != null)
                        {
                            if (!string.IsNullOrEmpty(x.RefNo.Trim()))
                            {
                                sqlCon.AddParameter("@RefNo", x.RefNo);
                            }
                        }
                        if (x.ContNo != null)
                        {
                            if (!string.IsNullOrEmpty(x.ContNo.Trim()))
                            {
                                sqlCon.AddParameter("@ContNo", x.ContNo);
                            }
                        }
                        if (x.InvNo != null)
                        {
                            if (!string.IsNullOrEmpty(x.InvNo.Trim()))
                            {
                                sqlCon.AddParameter("@InvNo", x.InvNo);
                            }
                        }

                        List<BD_InvoiceABH> data = await sqlCon.ExecuteQueryListAsync<BD_InvoiceABH>();
                        Rs.Rows = data.Count;
                        Rs.Data = data;

                        Rs.Msg = sqlCon.Message;
                        Rs.IsSuccess = sqlCon.IsSuccess;
                    }
                }
                else
                {
                    Rs.IsSuccess = false;
                    Rs.Msg = "ค้นหาจาก RefNo / ContNo / InvNo";
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "ListPayTrans"
                , Rs.Msg
                , json
                , "BD_vw_InvoiceABH_List");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> ListCreditTrip(BD_CreditTip x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_CreditTrip_List";
                    if (x.id > 0)
                    {
                        sqlCon.AddParameter("@id", x.id);
                    }
                    if (x.periodno > 0)
                    {
                        sqlCon.AddParameter("@periodno", x.periodno);
                    }
                    if (x.ayear > 0)
                    {
                        sqlCon.AddParameter("@ayear", x.ayear);
                    }

                    List<BD_CreditTip> data = await sqlCon.ExecuteQueryListAsync<BD_CreditTip>();
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
        public async Task<ExecResult> CreditTripGetFn(BD_CreditTip x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_CreditTrip_GetFn";
                    sqlCon.AddParameter("@DueDate", x.DueDate);

                    List<BD_CreditTip> data = await sqlCon.ExecuteQueryListAsync<BD_CreditTip>();
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

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> SaveCreditTrip(BD_CreditTip x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_CreditTrip_Save";
                    sqlCon.AddParameter("@ayear", x.ayear);
                    sqlCon.AddParameter("@periodno", x.periodno);
                    sqlCon.AddParameter("@OpenDate", x.OpenDate);
                    sqlCon.AddParameter("@CloseDate", x.CloseDate);
                    sqlCon.AddParameter("@LogBy", x.LogBy);
                    Rs.IsSuccess = await sqlCon.ExecuteNonQueryAsync();
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
        [HttpPost]
        public async Task<ExecResult> DeleteCreditTrip(BD_CreditTip x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_CreditTrip_Delete";
                    sqlCon.AddParameter("@id", x.id);
                    sqlCon.AddParameter("@LogBy", x.LogBy);
                    Rs.IsSuccess = await sqlCon.ExecuteNonQueryAsync();
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
        [HttpPost]
        public async Task<ExecResult> ListChgCont(BD_ChgCont x)
        {
            string json = JsonConvert.SerializeObject(x);
            BD_ChgCont xx = JsonConvert.DeserializeObject<BD_ChgCont>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_ChgCont_List";
                    sqlCon.AddParameter("@RefNo", x.RefNo);

                    List<BD_ChgCont> data = await sqlCon.ExecuteQueryListAsync<BD_ChgCont>();
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

            await SentDataLeakApi.SentApiFormat(x, "ListChgCont"
                , Rs.Msg
                , json
                , "BD_ChgCont_List");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> BDListChangeModel(BD_ChgModel x)
        {
            string json = JsonConvert.SerializeObject(x);
            BD_ChgModel xx = JsonConvert.DeserializeObject<BD_ChgModel>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);

            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_ChgModel_List";
                    sqlCon.AddParameter("@RefNo", x.RefNo);
                    List<BD_ChgModel> BDData = await sqlCon.ExecuteQueryListAsync<BD_ChgModel>();
                    foreach (var item in BDData)
                    {
                        if (item.Remark != null)
                        {
                            item.RemarkText = Encoding.GetEncoding("windows-874").GetString(item.Remark);
                        }
                    }
                    Rs.Data = BDData;
                    Rs.Rows = BDData.Count;
                    Rs.Msg = sqlCon.Message;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                }
                
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "ListChgModel"
                , Rs.Msg
                , json
                , "BD_ChgModel_List");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> FindDebtora([FromBody] FindMode x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                if (x.FindValue != null)
                {
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "BD_Debtora_Find";
                        sqlCon.AddParameter("@Mode", x.FindID);
                        sqlCon.AddParameter("@Values", x.FindValue);
                        if (x.FindBy != null)
                        {
                            sqlCon.AddParameter("@FindBy", x.FindBy);
                        }

                        List<BD_Debtora> data = await sqlCon.ExecuteQueryListAsync<BD_Debtora>();
                        Rs.Rows = data.Count;
                        Rs.Data = data;

                        Rs.IsSuccess = sqlCon.IsSuccess;
                        Rs.Msg = sqlCon.Message;
                    }

                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "FindDebtora", Rs.Msg, $"@Mode='{x.FindID}',@Values='{x.FindValue}',@FindBy={x.FindBy}", "BD_Debtora_Find");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> MastPayList(BD_MastPay x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_MastPay_List";
                    sqlCon.AddParameter("@RefNo", x.RefNo);
                    if (x.CONTNO != null)
                    {
                        if (!string.IsNullOrEmpty(x.CONTNO.Trim()))
                        {
                            sqlCon.AddParameter("@CONTNO", x.CONTNO);
                        }
                    }
                    List<BD_MastPay> Data = await sqlCon.ExecuteQueryListAsync<BD_MastPay>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "MastPayList", Rs.Msg, $"@RefNo='{x.RefNo}',@CONTNO='{x.CONTNO}'", "BD_MastPay_List");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> DebtorPayBList(BD_debtorpayb x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_debtorpayb_List";
                    sqlCon.AddParameter("@RefNo", x.refno);
                    if (x.contno != null)
                    {
                        if (!string.IsNullOrEmpty(x.contno.Trim()))
                        {
                            sqlCon.AddParameter("@CONTNO", x.contno);
                        }
                    }
                    List<BD_debtorpayb> Data = await sqlCon.ExecuteQueryListAsync<BD_debtorpayb>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;

                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "DebtorPayBList", Rs.Msg, $"@RefNo='{x.refno}',@CONTNO='{x.contno}'", "BD_debtorpayb_List");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> MastVatList(BD_mastvat x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_mastvat_List";
                    sqlCon.AddParameter("@RefNo", x.RefNo);
                    if (x.CONTNO != null)
                    {
                        if (!string.IsNullOrEmpty(x.CONTNO.Trim()))
                        {
                            sqlCon.AddParameter("@CONTNO", x.CONTNO);
                        }
                    }
                    List<BD_mastvat> Data = await sqlCon.ExecuteQueryListAsync<BD_mastvat>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "MastVatList", Rs.Msg, $"@RefNo='{x.RefNo}',@CONTNO='{x.CONTNO}'", "BD_mastvat_List");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> DebtorBList(BD_debtorb x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_debtorb_List";
                    sqlCon.AddParameter("@RefNo", x.refno);
                    if (x.contno != null)
                    {
                        if (!string.IsNullOrEmpty(x.contno.Trim()))
                        {
                            sqlCon.AddParameter("@CONTNO", x.contno);
                        }
                    }
                    List<BD_debtorb> Data = await sqlCon.ExecuteQueryListAsync<BD_debtorb>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "DebtorBList", Rs.Msg, $"@RefNo='{x.refno}',@CONTNO='{x.contno}'", "BD_debtorb_List");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> DebtorAgiList(BD_debtoragi x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_debtoragi_List";
                    sqlCon.AddParameter("@RefNo", x.refno);
                    List<BD_debtoragi> Data = await sqlCon.ExecuteQueryListAsync<BD_debtoragi>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "DebtorAgiList", Rs.Msg, $"@RefNo='{x.refno}'", "BD_debtoragi_List");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> DebtorPayDateList(BD_debtorpay x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_debtorpay_List_PayDate";
                    sqlCon.AddParameter("@Month", x.Month);
                    sqlCon.AddParameter("@Year", x.Year);
                    List<BD_debtorpay> Data = await sqlCon.ExecuteQueryListAsync<BD_debtorpay>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "DebtorPayDateList", Rs.Msg, $"@Month={x.Month},@Year={x.Year}", "BD_debtorpay_List_PayDate");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> DebtorPayDateDetailList(BD_debtorpay x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_debtorpay_List_PayDate_Detail";
                    sqlCon.AddParameter("@PayDate", x.paydate);
                    if (x.posted != null)
                    {
                        if (!string.IsNullOrEmpty(x.posted.Trim()))
                        {
                            sqlCon.AddParameter("@posted", x.posted);
                        }
                    }
                    List<BD_debtorpay> Data = await sqlCon.ExecuteQueryListAsync<BD_debtorpay>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "DebtorPayDateDetailList", Rs.Msg, $"@PayDate='{x.paydate}'", "BD_debtorpay_List_PayDate_Detail");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> AccStatusEffDateList(BD_accstatus x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_accstatus_List_effdate";
                    sqlCon.AddParameter("@Month", x.Month);
                    sqlCon.AddParameter("@Year", x.Year);
                    List<BD_accstatus> Data = await sqlCon.ExecuteQueryListAsync<BD_accstatus>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "AccStatusEffDateList", Rs.Msg, $"@Month={x.Month},@Year={x.Year}", "BD_accstatus_List_effdate");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> AccStatusEffDateDetailList(BD_accstatus x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_accstatus_List_effdate_detail";
                    sqlCon.AddParameter("@effdate", x.effdate);
                    if (x.todebtor != null)
                    {
                        if (!string.IsNullOrEmpty(x.todebtor.Trim()))
                        {
                            sqlCon.AddParameter("@todebtor", x.todebtor);
                        }
                    }
                    List<BD_accstatus> Data = await sqlCon.ExecuteQueryListAsync<BD_accstatus>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "AccStatusEffDateDetailList", Rs.Msg, $"@effdate='{x.effdate}'", "BD_accstatus_List_effdate_detail");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> AccStatusEffDateMastContDetailList(BD_MastCont x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_accstatus_List_effdate_detail_mastcont";
                    sqlCon.AddParameter("@effdate", x.EFFDATE);
                    if (x.toacc != null)
                    {
                        if (!string.IsNullOrEmpty(x.toacc.Trim()))
                        {
                            sqlCon.AddParameter("@toacc", x.toacc);
                        }
                    }
                    List<BD_MastCont> Data = await sqlCon.ExecuteQueryListAsync<BD_MastCont>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "AccStatusEffDateMastContDetailList", Rs.Msg, $"@effdate='{x.EFFDATE}'", "BD_accstatus_List_effdate_detail_mastcont");

            return Rs;
        }
        [HttpPost]
        public async Task<ExecResult> AccStatusEffDateMastContImportList(BD_MastCont x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_accstatus_List_effdate_mastcont_import";
                    sqlCon.AddParameter("@effdate", x.EFFDATE);
                    sqlCon.AddParameter("@is_change", x.is_change);
                    if (x.toacc != null)
                    {
                        if (!string.IsNullOrEmpty(x.toacc.Trim()))
                        {
                            sqlCon.AddParameter("@toacc", x.toacc);
                        }
                    }
                    List<BD_MastCont> Data = await sqlCon.ExecuteQueryListAsync<BD_MastCont>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "AccStatusEffDateMastContImportList", Rs.Msg, $"@effdate='{x.EFFDATE}'", "BD_accstatus_List_effdate_mastcont_import");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> AccStatusEffDateListCheck(BD_accstatus x)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = CommandType.StoredProcedure;
                    sqlCon.CommandString = "BD_accstatus_List_effdate_check";
                    sqlCon.AddParameter("@effdate", x.effdate);
                    sqlCon.AddParameter("@count_only", false);
                    List<BD_accstatus> Data = await sqlCon.ExecuteQueryListAsync<BD_accstatus>();
                    Rs.Data = Data;
                    Rs.Rows = Data.Count;
                    Rs.IsSuccess = sqlCon.IsSuccess;
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "AccStatusEffDateListCheck", Rs.Msg, $"@effdate={x.effdate.Value.ToString("yyyy-MM-dd")}", "BD_accstatus_List_effdate_check");

            return Rs;
        }

        [HttpPost]
        public async Task<ExecResult> AccStatusImport(BD_MastCont x)
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
                    sqlCon.CommandString = "BD_accstatus_import_from_mastcont";
                    sqlCon.AddParameter("@refno", x.RefNo);
                    sqlCon.AddParameter("@contno", x.CONTNO);
                    sqlCon.AddParameter("@createby", x.CreatedBy);
                    await sqlCon.ExecuteNonQueryAsync();

                    Rs.IsSuccess = await sqlCon.ExecuteTransactionAsync();
                    Rs.Msg = sqlCon.Message;
                }
            }
            catch (Exception ex)
            {
                Rs.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "AccStatusImport", Rs.Msg, $"@refno='{x.RefNo}',@contno='{x.CONTNO}',@createby='{x.CreatedBy}'", "BD_accstatus_import_from_mastcont");

            return Rs;
        }
    }
}
