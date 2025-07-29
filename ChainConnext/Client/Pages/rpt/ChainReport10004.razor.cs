using Append.Blazor.Printing;
using ChainConnext.Client.Services;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Reports;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using OfficeOpenXml;
using Radzen;
using System.Data;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;

namespace ChainConnext.Client.Pages.rpt
{
    public partial class ChainReport10004
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        [Parameter]
        public string? pRptName { get; set; }
        [Parameter]
        public int pRptId { get; set; }

        Rpt_Parameter Rpt = new Rpt_Parameter();

        string TitlePage = "";

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        bool IsLoading = false;
        bool IsData = false;
        bool IsPdfData = false;
        //protected override async Task OnInitializedAsync()
        //{
            
        //}

        protected override async Task OnParametersSetAsync()
        {
            IsLoading = true;

            Rpt.RptName = pRptName;
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
                var menu = userData.MenuList.Find(x => x.MenuId == pRptId);
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
            if (Rpt.DateFrom == null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "เลือกวันที่ชำระเงิน ด้วย");
                return;
            }
            if (Rpt.DateTo == null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "เลือกวันที่ชำระเงิน ด้วย");
                return;
            }
            IsData = false;
            IsPdfData = false;
            IsLoading = true;

            var response = await Http.PostAsJsonAsync("Report/RptChainReport", Rpt);

            Rpt = await response.Content.ReadFromJsonAsync<Rpt_Parameter>();
            Logger.LogInformation(Rpt.QueryExc);
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
                            await jsRuntime.SaveAs(Rpt.RptName + ".xlsx", pck.GetAsByteArray());
                        }
                    }
                }
            }

            IsLoading = false;
        }
    }
}
