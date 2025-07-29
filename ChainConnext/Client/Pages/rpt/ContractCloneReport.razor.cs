using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.Reports;
using Radzen.Blazor.Rendering;
using ChainConnext.Shared;
using System.Net.Http.Json;
using System.Xml.Linq;
using Radzen;
using Append.Blazor.Printing;
using System.Security.Cryptography;
using System.Data;
using OfficeOpenXml;
using ChainConnext.Client.Services;

namespace ChainConnext.Client.Pages.rpt
{
    public partial class ContractCloneReport
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        Rpt_Parameter Rpt = new Rpt_Parameter();

        string TitlePage = "";

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        bool IsLoading = false;
        bool IsData = false;
        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            await CheckPermission();

            if (IsAccess)
            {

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
                var menu = userData.MenuList.Find(x => x.MenuId == 4);
                if (menu != null)
                {
                    TitlePage = menu.MenuDesc;
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        async Task Find()
        {
            if (Rpt.todate1_from == null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "เลือกวันที่ ด้วย");
                return;
            }
            if (Rpt.todate1_to == null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "เลือกวันที่ ด้วย");
                return;
            }
            IsData = false;
            IsLoading = true;

            var response = await Http.PostAsJsonAsync("Report/RptContractClone", Rpt);

            Rpt = await response.Content.ReadFromJsonAsync<Rpt_Parameter>();
            Logger.LogInformation(Rpt.Msg);
            if (!string.IsNullOrEmpty(Rpt.PdfData.Trim()))
            {
                IsData = true;
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
                await PrintingService.Print(new PrintOptions(Rpt.PdfData) { Base64 = true, ShowModal = true });
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
    }
}
