using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using FastMember;
using Microsoft.JSInterop;
using OfficeOpenXml;
using Radzen;
using System.Data;
using ChainConnext.Client.Services;

namespace ChainConnext.Client.Pages.Settings
{
    public partial class SetChanelSale
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        bool isLoading = false;
        List<Chanel> chanels = new List<Chanel>();

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();

        int Height;
        int Width;

        protected override async Task OnParametersSetAsync()
        {
            isLoading = true;

            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            await CheckPermission();

            if (IsAccess)
            {
                await ListChanelData();
            }
            isLoading = false;
        }

        async Task CheckPermission()
        {
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
                var menu = userData.MenuList.Find(x => x.MenuId == 15);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        private async Task ListChanelData()
        {
            var postBody = new Chanel { ChanelDep = 0, id = 0 };
            var response = await Http.PostAsJsonAsync("BD/ListChanel", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    chanels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Chanel>>(Rs.Data.ToString());
                }
            }
        }

        async Task OpenEdit(Chanel? daTa, string key)
        {
            if (daTa == null)
            {
                daTa = new Chanel();
            }

            //Logger.LogInformation($"InvalidSubmit: {JsonSerializer.Serialize(daTa, new JsonSerializerOptions() { WriteIndented = true })}");

            ExecResult Rs = await dialogService.OpenAsync<SetChanelSaleDialog>($"{key} : {daTa.code} - {daTa.name}",
                  new Dictionary<string, object>() { { "ID", daTa.id }, { "Key", key } },
                  new DialogOptions() { Width = "500px", Height = "500px" });

            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    await ListChanelData();
                }
            }
        }
        async Task OpenDelete(Chanel daTa, string key)
        {
            var DRs = await dialogService.Confirm($"ลบ {daTa.code} - {daTa.name} หรือไม่?", $"ยืนยัน ลบ ช่องทาง {daTa.code} - {daTa.name}", new ConfirmOptions() { OkButtonText = "ใช่", CancelButtonText = "ไม่" });
            if (DRs.Value)
            {
                var response = await Http.PostAsJsonAsync("BD/DeleteChanel", daTa);
                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        await ListChanelData();
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
            using (var reader = ObjectReader.Create(chanels))
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
                        await jsRuntime.SaveAs("Chanel.xlsx", pck.GetAsByteArray());
                    }
                }
            }
        }
    }
}
