using ChainConnext.Shared.BD;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Client.Services;
using Microsoft.JSInterop;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using Radzen;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ChainConnext.Shared.Customers;
using Newtonsoft.Json;

namespace ChainConnext.Client.Pages.Settings
{
    public partial class SetCreditTrip
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        bool isLoading = false;
        List<BD_CreditTip> bD_CreditTips = new List<BD_CreditTip>();
        IList<BD_CreditTip>? selectedCreditTips;

        BD_CreditTip bD_CreditTip = new BD_CreditTip();

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();

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
                await ListCreditTripData();
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
                var menu = userData.MenuList.Find(x => x.MenuId == 28);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        private async Task ListCreditTripData()
        {
            isLoading = true;

            var postBody = new BD_CreditTip { ayear = bD_CreditTip.ayear, periodno = 0, id = 0 };
            var response = await Http.PostAsJsonAsync("BD/ListCreditTrip", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        bD_CreditTips = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_CreditTip>>(Rs.Data.ToString());
                    }
                    else
                    {
                        bD_CreditTips = new List<BD_CreditTip>();
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
            isLoading = false;
        }

        async Task OpenDelete(BD_CreditTip daTa, string key)
        {
            var DRs = await dialogService.Confirm($"ลบ {daTa.periodno} / {daTa.ayear} หรือไม่?", $"ยืนยัน ลบ {daTa.periodno} / {daTa.ayear}", new ConfirmOptions() { OkButtonText = "ใช่", CancelButtonText = "ไม่" });
            if (DRs.Value)
            {
                daTa.LogBy = userData.UserID;
                var response = await Http.PostAsJsonAsync("BD/DeleteCreditTrip", daTa);
                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        await ListCreditTripData();
                    }
                    else
                    {
                        Logger.LogInformation(Rs.Msg);
                        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                    }
                }
            }
        }

        async Task OnSave()
        {
            isLoading = true;

            bD_CreditTip.LogBy = userData.UserID;

            var response = await Http.PostAsJsonAsync("BD/SaveCreditTrip", bD_CreditTip);
            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                    await ListCreditTripData();
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
            isLoading = false;
        }

        async Task OnYearChange()
        {
            await ListCreditTripData();
        }

        async Task OnMonthChange(int values)
        {
            selectedCreditTips = null;
            if (bD_CreditTip.OpenDate != null)
            {
                int m = values;
                int y = bD_CreditTip.OpenDate.Value.Year;
                int d = bD_CreditTip.OpenDate.Value.Day;
                if (values == 1)
                {
                    m = 12;
                    y = y - 1;
                }
                else
                {
                    m = values - 1;
                }
                bD_CreditTip.OpenDate = new DateTime(y, m, d);
                bD_CreditTip.CloseDate = bD_CreditTip.OpenDate.Value.AddMonths(1).AddDays(-1);
            }
        }

        async Task OnTripCellClick(DataGridCellMouseEventArgs<BD_CreditTip> args)
        {
            string json = JsonConvert.SerializeObject(args.Data);
            BD_CreditTip xx = JsonConvert.DeserializeObject<BD_CreditTip>(json);
            bD_CreditTip = xx;
        }
    }
}
