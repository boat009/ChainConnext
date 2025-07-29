using ChainConnext.Server.Helpers;
using ChainConnext.Shared.Payments;
using ChainConnext.Shared;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ChainConnext.Shared.Reports;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using FastReport.Web;
using FastReport.Export.PdfSimple;
using Radzen;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        [HttpPost]
        public async Task<Rpt_Parameter> RptContractClone(Rpt_Parameter x)
        {
            string json = JsonConvert.SerializeObject(x);
            Rpt_Parameter xx = JsonConvert.DeserializeObject<Rpt_Parameter>(json);
            xx.UserData = null;
            json = JsonConvert.SerializeObject(xx);
            try
            {
                //StringBuilder stbdQuery = new StringBuilder();
                //if (x.todate1_from != null)
                //{
                //    string RDateF = string.Format(new CultureInfo("en-US", true), "{0:dd/MM/yyyy}", x.todate1_from);
                //    string RDateT = string.Format(new CultureInfo("en-US", true), "{0:dd/MM/yyyy}", x.todate1_to);
                //    if (BaseShared.CheckCompareDate(Convert.ToDateTime(x.todate1_from.Value.ToShortDateString())
                //        , Convert.ToDateTime(x.todate1_to.Value.ToShortDateString())))
                //    {
                //        stbdQuery.AppendFormat(" And Convert(Date,x.todate1,103)=Convert(Date,'{0}',103) ", RDateF);
                //    }
                //    else
                //    {
                //        stbdQuery.AppendFormat(" And Convert(Date,x.todate1,103) Between Convert(Date,'{0}',103) And  Convert(Date,'{1}',103) "
                //            , RDateF, RDateT);
                //    }
                //}
                //string Query = stbdQuery.ToString().Trim();
                //Query = Query.Replace("'", "''");


                //string MQuery = string.Format("Exec Rpt_Contract_Clone '{0}';", Query);

                DataTable dt = new DataTable();
                //if (!string.IsNullOrEmpty(Query.Trim()))
                if(x.todate1_from != null)
                {
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "Rpt_Contract_Clone";
                        sqlCon.AddParameter("@DateFrom", x.todate1_from);
                        sqlCon.AddParameter("@DateTo", x.todate1_to);
                        dt = await sqlCon.ExecuteQueryAsync();
                        
                        x.Msg = sqlCon.Message;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        x.Data = BaseShared.DataTableToJson(dt);

                        string MPath = @$"{AppDomain.CurrentDomain.BaseDirectory}Frxs\Rpt_Contract_Clone.frx";

                        try
                        {
                            WebReport web = new WebReport();
                            web.Report.Load(MPath);
                            web.Report.RegisterData(dt, "RptData");
                            web.Report.Prepare();
                            byte[] bytes;
                            using (var stream = new MemoryStream())
                            {
                                web.Report.Export(new PDFSimpleExport(), stream);
                                bytes = stream.ToArray();
                            }
                            x.RptName = web.Report.GetReportName;
                            string base64 = Convert.ToBase64String(bytes);
                            if (!string.IsNullOrEmpty(base64.Trim()))
                            {
                                x.PdfData = base64;
                            }
                            else
                            {
                                x.Msg = x.Msg + " // Load Report Fail..";
                            }
                        }
                        catch (Exception ex)
                        {
                            x.Msg = ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                x.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "RptContractClone"
                , x.Msg
                , json
                , "Rpt_Contract_Clone");

            return x;
        }
        [HttpPost]
        public async Task<Rpt_Parameter> RptContract(Rpt_Parameter x)
        {
            string json = JsonConvert.SerializeObject(x);
            Rpt_Parameter xx = JsonConvert.DeserializeObject<Rpt_Parameter>(json);
            xx.UserData = null;
            x.JsonSendData = JsonConvert.SerializeObject(xx);

            try
            {
                DataTable dt = new DataTable();
                if (x.WhereCond != null)
                {
                    x.QueryExc = string.Format("Exec Rpt_Contract '{0}';", x.WhereCond);
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.CommandString = x.QueryExc;

                        dt = await sqlCon.ExecuteQueryAsync();

                        x.Msg = sqlCon.Message;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        x.Data = BaseShared.DataTableToJson(dt);

                        string MPath = @$"{AppDomain.CurrentDomain.BaseDirectory}Frxs\TsrBranch\Rpt_Contract.frx";

                        DataSet ds = new DataSet();
                        ds.Tables.Add(dt);
                        ds.Tables[0].TableName = "RptData";
                        try
                        {
                            WebReport web = new WebReport();
                            web.Report.Load(MPath);
                            web.Report.RegisterData(ds, "RptData");
                            web.Report.SetParameterValue("pRptDate", x.CustomParametor1);
                            web.Report.Prepare();
                            byte[] bytes;
                            using (var stream = new MemoryStream())
                            {
                                web.Report.Export(new PDFSimpleExport(), stream);
                                bytes = stream.ToArray();
                            }
                            x.RptName = web.Report.GetReportName;
                            string base64 = Convert.ToBase64String(bytes);
                            if (!string.IsNullOrEmpty(base64.Trim()))
                            {
                                x.PdfData = base64;
                            }
                            else
                            {
                                x.Msg = x.Msg + " // Load Report Fail..";
                            }
                        }
                        catch (Exception ex) 
                        {
                            x.Msg = ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                x.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "RptContract"
                , x.Msg
                , x.JsonSendData
                , "Rpt_Contract");

            return x;
        }
        [HttpPost]
        public async Task<Rpt_Parameter> RptContractPayment(Rpt_Parameter x)
        {
            string json = JsonConvert.SerializeObject(x);
            Rpt_Parameter xx = JsonConvert.DeserializeObject<Rpt_Parameter>(json);
            xx.UserData = null;
            x.JsonSendData = JsonConvert.SerializeObject(xx);

            try
            {
                DataTable dt = new DataTable();
                if (x.WhereCond != null)
                {
                    x.QueryExc = string.Format("Exec Rpt_Recv '{0}';", x.WhereCond);
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.CommandString = x.QueryExc;

                        dt = await sqlCon.ExecuteQueryAsync();

                        x.Msg = sqlCon.Message;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        x.Data = BaseShared.DataTableToJson(dt);

                        string MPath = @$"{AppDomain.CurrentDomain.BaseDirectory}Frxs\TsrBranch\Rpt_Recv.frx";

                        DataSet ds = new DataSet();
                        ds.Tables.Add(dt);
                        ds.Tables[0].TableName = "RptData";

                        WebReport web = new WebReport();
                        web.Report.Load(MPath);
                        web.Report.RegisterData(ds, "RptData");
                        web.Report.SetParameterValue("pRptDate", x.CustomParametor1);
                        web.Report.Prepare();
                        byte[] bytes;
                        using (var stream = new MemoryStream())
                        {
                            web.Report.Export(new PDFSimpleExport(), stream);
                            bytes = stream.ToArray();
                        }
                        x.RptName = web.Report.GetReportName;
                        string base64 = Convert.ToBase64String(bytes);
                        if (!string.IsNullOrEmpty(base64.Trim()))
                        {
                            x.PdfData = base64;
                        }
                        else
                        {
                            x.Msg = x.Msg + " // Load Report Fail..";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                x.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "RptContractPayment"
                , x.Msg
                , x.JsonSendData
                , "Rpt_Recv");

            return x;
        }
        [HttpPost]
        public async Task<Rpt_Parameter> RptChainReport(Rpt_Parameter x)
        {
            string json = JsonConvert.SerializeObject(x);
            Rpt_Parameter xx = JsonConvert.DeserializeObject<Rpt_Parameter>(json);
            xx.UserData = null;
            x.JsonSendData = JsonConvert.SerializeObject(xx);

            try
            {
                DataTable dt = new DataTable();
                if (x.DateFrom != null)
                {
                    x.QueryExc = string.Format("Exec {2} '{0:yyyy-MM-dd}','{1:yyyy-MM-dd}';", x.DateFrom.Value.AddYears(543), x.DateTo.Value.AddYears(543), x.RptName);
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "SP_ChainReport_Execute";
                        sqlCon.AddParameter("@RptName", x.RptName);
                        sqlCon.AddParameter("@DateFrom", x.DateFrom);
                        sqlCon.AddParameter("@DateTo", x.DateTo);

                        dt = await sqlCon.ExecuteQueryAsync();

                        x.Msg = sqlCon.Message;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        x.Data = BaseShared.DataTableToJson(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                x.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "RptChainReport"
                , x.Msg
                , x.JsonSendData
                , "SP_ChainReport_Execute");

            return x;
        }

        [HttpPost]
        public async Task<Rpt_Parameter> RptPayRReport(Rpt_Parameter x)
        {
            string json = JsonConvert.SerializeObject(x);
            Rpt_Parameter xx = JsonConvert.DeserializeObject<Rpt_Parameter>(json);
            xx.UserData = null;
            x.JsonSendData = JsonConvert.SerializeObject(xx);

            try
            {
                DataTable dt = new DataTable();
                if (x.DateFrom != null)
                {
                    using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                    {
                        sqlCon.SqlCommandType = CommandType.StoredProcedure;
                        sqlCon.CommandString = "Rpt_Pay_R";
                        sqlCon.AddParameter("@DateFrom", x.DateFrom);
                        sqlCon.AddParameter("@DateTo", x.DateTo);

                        dt = await sqlCon.ExecuteQueryAsync();

                        x.Msg = sqlCon.Message;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        x.Data = BaseShared.DataTableToJson(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                x.Msg = ex.Message;
            }

            await SentDataLeakApi.SentApiFormat(x, "RptPayRReport"
                , x.Msg
                , x.JsonSendData
                , "Rpt_Pay_R");

            return x;
        }
    }
}
