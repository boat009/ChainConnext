using ChainConnext.Client.Services;
using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Reports;
using FastMember;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using OfficeOpenXml;
using Radzen;
using System.Data;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChainConnext.Client.Pages.Settings
{
    public partial class SetCreditArea
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        bool isLoading = false;
        List<CArea> cAreas = new List<CArea>();
        IList<CArea>? selectedCArea;

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        int Height;
        int Width;

        protected override async Task OnParametersSetAsync()
        {
            isLoading = true;

            //var xx = Navigation.Uri.Replace(Navigation.BaseUri,"");
            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            await CheckPermission();

            if (IsAccess)
            {
                await ListCAreaData();
            }
            isLoading = false;
        }

        async Task CheckPermission()
        {
            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (userData.PermsList.Count > 0)
            {
                var perms = userData.PermsList.Find(x => x.PermsName == "IsSave");
                if (perms != null)
                {
                    IsSave = perms.IsPerms;
                }
                perms = userData.PermsList.Find(x => x.PermsName == "IsDelete");
                if (perms != null)
                {
                    IsDelete = perms.IsPerms;
                }
            }
            if (userData.MenuList.Count > 0)
            {
                var path = Navigation.Uri.Replace(Navigation.BaseUri, "");
                var menu = userData.MenuList.Find(x => x.MenuId == 16);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        private async Task ListCAreaData()
        {
            isLoading = true;
            StateHasChanged();

            var postBody = new CArea();
            var response = await Http.PostAsJsonAsync("BD/ListCArea", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        cAreas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CArea>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
            isLoading = false;
            StateHasChanged();
        }

        async Task OpenEdit(CArea? daTa, string key)
        {
            if (daTa == null)
            {
                daTa = new CArea();
            }

            //Logger.LogInformation($"InvalidSubmit: {JsonSerializer.Serialize(daTa, new JsonSerializerOptions() { WriteIndented = true })}");

            ExecResult Rs = await dialogService.OpenAsync<SetCreditAreaDialog>($"{key}",
                  new Dictionary<string, object>() { { "ID", daTa.ID }, { "Key", key } },
                  new DialogOptions() { Width = "500px", Height = "400px" });

            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    await ListCAreaData();
                }
            }
        }
        async Task OpenDelete(CArea daTa, string key)
        {
            var DRs = await dialogService.Confirm($"ลบ เขต {daTa.ACode} - {daTa.CCode} {daTa.Name} หรือไม่?", $"ยืนยัน ลบ เขต {daTa.ACode} - {daTa.CCode} {daTa.Name}", new ConfirmOptions() { OkButtonText = "ใช่", CancelButtonText = "ไม่" });
            if (DRs.Value)
            {
                var response = await Http.PostAsJsonAsync("BD/DeleteCArea", daTa);
                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        await ListCAreaData();
                    }
                    else
                    {
                        Logger.LogInformation(Rs.Msg);
                        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                    }
                }
            }
        }

        async Task ExportToExcel()
        {
            DataTable dt = new DataTable();
            using (var reader = ObjectReader.Create(cAreas))
            {
                dt.Load(reader);
            }
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
                        await jsRuntime.SaveAs("CreditArea.xlsx", pck.GetAsByteArray());
                    }
                }
            }
        }
    }
}
