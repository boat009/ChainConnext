using Append.Blazor.Printing;
using ChainConnext.Client.Services;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.Reports;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using OfficeOpenXml;
using Radzen;
using System.Data;
using System.Net.Http.Json;
using System.Text;
using System.Globalization;
using ChainConnext.Shared.BD;

namespace ChainConnext.Client.Pages.rpt
{
    public partial class ContractReport
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        Rpt_Parameter Rpt = new Rpt_Parameter();
        List<Branch> branches = new List<Branch>();

        DateTime? ContDateFrom;
        DateTime? ContDateTo;
        string? ContNo;
        bool IsX = false;
        string? BranchCode;

        string TitlePage = "";

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        bool IsLoading = false;
        bool IsData = false;
        bool IsPdfData = false;
        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            await CheckPermission();

            if (IsAccess)
            {
                await ListBranch();
            }

            IsLoading = false;
        }

        async Task CheckPermission()
        {
            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            Rpt.UserData = userData;

            if (userData.PermsList.Count > 0)
            {
                var perms = userData.PermsList.Find(x => x.PermsName == "IsSave");
                if (perms != null)
                {
                    IsSave = perms.IsPerms;
                    IsDelete = perms.IsPerms;
                }
            }
            if (userData.MenuList.Count > 0)
            {
                var path = Navigation.Uri.Replace(Navigation.BaseUri, "");
                var menu = userData.MenuList.Find(x => x.MenuId == 20);
                if (menu != null)
                {
                    TitlePage = menu.MenuDesc;
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        private async Task ListBranch()
        {
            var postBody = new Branch();
            var response = await Http.PostAsJsonAsync("BD/ListBranch", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Data != null)
                {
                    branches = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Branch>>(Rs.Data.ToString());
                }
            }
        }

        async Task Find()
        {
            if (ContDateFrom == null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "เลือกวันที่ ด้วย");
                return;
            }
            if (ContDateTo == null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "เลือกวันที่ ด้วย");
                return;
            }
            IsData = false;
            IsPdfData = false;
            IsLoading = true;

            StringBuilder stbdQuery = new StringBuilder();
            if (BranchCode != null)
            {
                if (!string.IsNullOrEmpty(BranchCode.Trim()))
                {
                    stbdQuery.AppendFormat(" And Mc.CHECKER='{0}' ", BaseShared.CheckInj(BranchCode.Trim()));
                }
            }
            if (IsX)
            {
                stbdQuery.AppendFormat(" And Left(Mc.CONTNO,1)='X' ");
            }
            if (ContNo != null)
            {
                if (!string.IsNullOrEmpty(ContNo.Trim()))
                {
                    stbdQuery.AppendFormat(" And Mc.CONTNO='{0}' ", BaseShared.CheckInj(ContNo.Trim()));
                }
            }
            if (ContDateFrom != null)
            {
                string RDateF = string.Format(new CultureInfo("en-US", true), "{0:dd/MM/yyyy}", ContDateFrom);
                string RDateT = string.Format(new CultureInfo("en-US", true), "{0:dd/MM/yyyy}", ContDateTo);
                if (BaseShared.CheckCompareDate(Convert.ToDateTime(ContDateFrom.Value.ToShortDateString())
                    , Convert.ToDateTime(ContDateTo.Value.ToShortDateString())))
                {
                    stbdQuery.AppendFormat(" And Convert(Date,dbo.ToEnDate(Mc.EFFDATE),103)=Convert(Date,'{0}',103) ", RDateF);
                }
                else
                {
                    stbdQuery.AppendFormat(" And Convert(Date,dbo.ToEnDate(Mc.EFFDATE),103) Between Convert(Date,'{0}',103) And  Convert(Date,'{1}',103) "
                        , RDateF, RDateT);
                }

                string RDateFTH = string.Format(new CultureInfo("th-TH", true), "{0:dd/MM/yyyy}", ContDateFrom);
                string RDateTTH = string.Format(new CultureInfo("th-TH", true), "{0:dd/MM/yyyy}", ContDateTo);

                Rpt.CustomParametor1 = $"{RDateFTH} ถึง {RDateTTH}";
            }
            string Query = stbdQuery.ToString().Trim();
            Query = Query.Replace("'", "''");

            Rpt.WhereCond = Query;
            //Logger.LogInformation("WhereCond : " + Rpt.WhereCond);

            var response = await Http.PostAsJsonAsync("Report/RptContract", Rpt);

            Rpt = await response.Content.ReadFromJsonAsync<Rpt_Parameter>();
            //Logger.LogInformation(Rpt.Msg);
            //Logger.LogInformation("QueryExc : " + Rpt.QueryExc);
            //Logger.LogInformation("Json : " + Rpt.JsonSendData);
            if (!string.IsNullOrEmpty(Rpt.Data.Trim()))
            {
                IsData = true;
                if (!string.IsNullOrEmpty(Rpt.PdfData.Trim()))
                {
                    IsPdfData = true;
                }
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "ไม่พบข้อมูล");
            }

            IsLoading = false;
        }
        async Task Print()
        {
            IsLoading = true;

            if (!string.IsNullOrEmpty(Rpt.PdfData.Trim()))
            {
                await PrintingService.Print(new PrintOptions(Rpt.PdfData) { Base64 = true, ShowModal = true, ModalMessage = "Loading..." });
            }

            IsLoading = false;
        }
        async Task ExportExcel()
        {
            IsLoading = true;

            if (!string.IsNullOrEmpty(Rpt.Data.Trim()))
            {
                DataTable dt = BaseShared.JsonToDataTable(Rpt.Data);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        using (ExcelPackage pck = new ExcelPackage())
                        {
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                            ws.Cells["A1"].LoadFromDataTable(dt, true);
                            ws = BaseShared.FormatExccel(ws, dt);
                            var ms = new System.IO.MemoryStream();
                            pck.SaveAs(ms);
                            await jsRuntime.SaveAs("ReportContractClone.xlsx", pck.GetAsByteArray());
                        }
                    }
                }
            }

            IsLoading = false;
        }

        async Task OnChange(string value, string name)
        {
            //Logger.LogInformation($"{name} value changed to {value}");
            if (!string.IsNullOrEmpty(value.Trim()))
            {

                switch (name)
                {
                    case "RefNo":
                        {
                            //SearchValue = BaseShared.FillRefNo(value.Trim(), 9);
                        }
                        break;
                    case "ContNo":
                        {
                            ContNo = BaseShared.FillRefNo(value.Trim(), 8).ToUpper();
                        }
                        break;
                    case "CustName":
                        {
                            //SearchValue = value.Trim();
                        }
                        break;
                }
            }
        }
    }
}
