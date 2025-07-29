using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using ChainConnext.Shared.BD;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Client.Services;
using Microsoft.JSInterop;
using Radzen;
using System.Text.Json;
using FastMember;
using OfficeOpenXml;
using System.Data;
using System.Threading.Channels;
using System.Reflection;
using ChainConnext.Shared.Reports;

namespace ChainConnext.Client.Pages.Settings
{
    public partial class SetProModel
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        bool IsKindLoading = false;
        bool IsModelLoading = false;
        List<BDProModel> proModels = new List<BDProModel>();
        List<BDProKind> proKinds = new List<BDProKind>();

        IList<BDProModel>? selectedModel;
        IList<BDProKind>? selectedKind;

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();

        bool IsKindSelected = false;

        int Height;
        int Width;

        bool IsLoading = false;

        BDProModel pmdTmp = new BDProModel();

        protected override async Task OnParametersSetAsync()
        {

            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            await CheckPermission();

            if (IsAccess)
            {
                await GetProKindData();
                //await GetProModelData();
            }

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
                var menu = userData.MenuList.Find(x => x.MenuId == 14);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        async Task RemoveSetModelValues()
        {
            string Key = ShareValues.GetTokenUrl() + "_SetModel";
            await localStorage.RemoveItemAsync(Key);
        }

        private async Task<List<BDProModel>> GetProModelData(string cate)
        {
            IsModelLoading = true;

            await RemoveSetModelValues();

            List<BDProModel> data = new List<BDProModel>();

            var postBody = new BDProModel { cate = cate };
            var response = await Http.PostAsJsonAsync("BD/ListBDProModel", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BDProModel>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
            IsModelLoading = false;
            return data;
        }

        private async Task GetProKindData()
        {
            IsKindLoading = true;
            var postBody = new BDProKind();
            var response = await Http.PostAsJsonAsync("BD/ListProKind", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        proKinds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BDProKind>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
            IsKindLoading = false;
        }

        async Task OnKindCellClick(DataGridCellMouseEventArgs<BDProKind> args)
        {
            IsKindSelected = true;
            proModels = new List<BDProModel>();
            proModels = await GetProModelData(args.Data.code);
        }

        async Task OpenKindEdit(BDProKind? daTa, string key)
        {
            if (daTa == null)
            {
                daTa = new BDProKind();
            }

            daTa.CreateBy = userData.UserID;

            //Logger.LogInformation($"InvalidSubmit: {JsonSerializer.Serialize(daTa, new JsonSerializerOptions() { WriteIndented = true })}");

            ExecResult Rs = await dialogService.OpenAsync<SetProKindDialog>($"{key}",
                  new Dictionary<string, object>() { { "code", daTa.code }, { "Key", key } },
                  new DialogOptions() { Width = "800px", Height = "250px" });

            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    await GetProKindData();
                }
            }
        }
        async Task OpenKindDelete(BDProKind daTa, string key)
        {
            var DRs = await dialogService.Confirm($"ลบ {daTa.code} - {daTa.name} หรือไม่?", $"ยืนยัน ลบ {daTa.code} - {daTa.name}", new ConfirmOptions() { OkButtonText = "ใช่", CancelButtonText = "ไม่" });
            if (DRs.Value)
            {
                daTa.CreateBy = userData.UserID;
                var response = await Http.PostAsJsonAsync("BD/DeleteProKind", daTa);
                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        await GetProKindData();
                        StateHasChanged();
                    }
                    else
                    {
                        Logger.LogInformation(Rs.Msg);
                        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                    }
                }
            }
        }
        async Task OpenModelEdit(BDProModel? daTa, string key)
        {
            if (daTa == null)
            {
                daTa = new BDProModel();
            }
            daTa.CreateBy = userData.UserID;
            //Logger.LogInformation($"InvalidSubmit: {JsonSerializer.Serialize(daTa, new JsonSerializerOptions() { WriteIndented = true })}");

            ExecResult Rs = await dialogService.OpenAsync<SetProModelDialog>($"{key}",
                  new Dictionary<string, object>() { { "ModelCode", daTa.MODEL }, { "Cate", daTa.cate }, { "Key", key } },
                  new DialogOptions() { Width = "800px", Height = "650px" });

            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    proModels = await GetProModelData(selectedKind?.FirstOrDefault()?.code);
                }
            }
        }
        async Task OpenModelDelete(BDProModel daTa, string key)
        {
            var DRs = await dialogService.Confirm($"ลบ {daTa.MODEL} - {daTa.ModelDesc} หรือไม่?", $"ยืนยัน ลบ {daTa.MODEL} - {daTa.ModelDesc}", new ConfirmOptions() { OkButtonText = "ใช่", CancelButtonText = "ไม่" });
            if (DRs.Value)
            {
                daTa.CreateBy = userData.UserID;
                var response = await Http.PostAsJsonAsync("BD/DeleteBDProModel", daTa);
                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        proModels = await GetProModelData(selectedKind?.FirstOrDefault()?.code);
                    }
                    else
                    {
                        Logger.LogInformation(Rs.Msg);
                        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                    }
                }
            }
        }

        async Task OnCellContextMenu(DataGridCellMouseEventArgs<BDProModel> args)
        {
            ContextMenuService.Open(args,
                    new List<ContextMenuItem> {
                new ContextMenuItem(){ Text = "แก้ไข", Value = 1, Icon = "edit" },
                new ContextMenuItem(){ Text = "ลบ", Value = 2, Icon = "delete" },
                    },
                async (e) =>
                {
                    switch (e.Value)
                    {
                        case 1:
                            {
                                await OpenModelEdit(args.Data, "Edit");
                            }
                            break;
                        case 2:
                            {
                                await OpenModelDelete(args.Data, "Delete");
                            }
                            break;
                    }
                }
                 );
        }

        async Task ExportToExcel()
        {
            IsLoading = true;

            List<BDProModel> data = new List<BDProModel>();
            data = await GetProModelData("");

            if (data.Count > 0)
            {
                Type fieldsType = typeof(BDProModel);
                PropertyInfo[] props = fieldsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                DataTable dt = new DataTable();
                foreach (PropertyInfo prop in props)
                {
                    dt.Columns.Add(prop.Name);
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                    ws.Cells["A1"].LoadFromCollection<BDProModel>(data, true);
                    ws = BaseShared.FormatExccel(ws, dt);
                    var ms = new System.IO.MemoryStream();
                    pck.SaveAs(ms);
                    await jsRuntime.SaveAs("ProModel.xlsx", pck.GetAsByteArray());
                }
            }
            IsLoading = false;
        }
    }
}
