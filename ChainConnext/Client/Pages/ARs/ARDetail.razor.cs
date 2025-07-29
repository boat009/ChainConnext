using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared;
using ChainConnext.Client.Services;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;
using ChainConnext.Shared.Authen;
using System.Net.Http.Json;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Contracts;
using Radzen;
using ChainConnext.Shared.Payments;
using System.Buffers;
using System.Drawing;

namespace ChainConnext.Client.Pages.ARs
{
    public partial class ARDetail
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        int selectedIndex = 0;
        int selectedIndexTop = 0;
        int selectedIndexChg = 0;
        int selectedIndexBill = 0;

        List<FindMode> findModes = new List<FindMode>();
        int findSelected = 1;
        bool IsLoad = false;

        int Height;
        int Width;

        Authens userData = new Authens();
        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        string SearchValue = "";

        BD_Debtora Dba = new BD_Debtora();

        protected override async Task OnInitializedAsync()
        {
            IsLoad = true;

            await CheckDefaultValues();

            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            await CheckPermission();

            if (IsAccess)
            {
                await GetFindModeData();

                if (Navigation.BaseUri.Contains("localhost"))
                {
                    SearchValue = "640109714";
                }
            }

            IsLoad = false;
        }

        async Task CheckDefaultValues()
        {
            string Key = ShareValues.GetTokenUrl() + "_DefaultValues";
            try
            {
                UserDefaultValues? userDefault;
                userDefault = await localStorage.GetItemAsync<UserDefaultValues>(Key);
                if (userDefault != null)
                {
                    findSelected = userDefault.ARFindMode;
                }
            }
            catch
            {
                await localStorage.RemoveItemAsync(Key);
            }
        }

        async Task SetDefaultValues()
        {
            string Key = ShareValues.GetTokenUrl() + "_DefaultValues";
            try
            {
                bool is_change = false;
                UserDefaultValues? userDefault;
                userDefault = await localStorage.GetItemAsync<UserDefaultValues>(Key);
                if (userDefault != null)
                {
                    if (userDefault.ARFindMode != findSelected)
                    {
                        userDefault.ARFindMode = findSelected;
                        is_change = true;
                    }
                }
                else
                {
                    userDefault = new UserDefaultValues();
                    userDefault.ARFindMode = findSelected;
                    is_change = true;
                }
                if (is_change)
                {
                    await localStorage.SetItemAsync(Key, userDefault);
                }
            }
            catch
            {
                await localStorage.RemoveItemAsync(Key);
            }
        }

        async Task CheckPermission()
        {
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (userData.MenuList.Count > 0)
            {
                var menu = userData.MenuList.Find(x => x.MenuId == 74);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsSave;
                }
            }
        }

        private async Task GetFindModeData()
        {
            var postBody = new FindMode();
            var response = await Http.PostAsJsonAsync("Find/ListFindMode", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    findModes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FindMode>>(Rs.Data.ToString());
                }
            }
        }

        async Task OnChange(string value, string name)
        {
            //Logger.LogInformation($"{name} value changed to {value}");
            switch (name)
            {
                case "Find":
                    {
                        if (!string.IsNullOrEmpty(value.Trim()))
                        {
                            int len = 8;
                            switch (findSelected)
                            {
                                case 1: len = 9; break;
                                case 2: len = 8; break;
                            }
                            SearchValue = BaseShared.FillRefNo(value.Trim(), len).ToUpper();
                            await FindData();
                        }
                    }
                    break;
            }
            StateHasChanged();
        }

        async Task Find(BD_Debtora? daTa, string key)
        {
            await FindData();
        }

        async Task FindTo(string? serch)
        {
            if (serch != null)
            {
                findSelected = 1;
                SearchValue = serch;
                await FindData();
            }
        }

        async Task FindData()
        {
            IsLoad = true;

            Dba = new BD_Debtora();
            bool is_found = false;

            var postBody = new FindMode { FindID = findSelected, FindValue = SearchValue, FindBy = userData.UserID, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/FindDebtora", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    Dba = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_Debtora>>(Rs.Data.ToString()).FirstOrDefault();

                    if (Dba != null)
                    {
                        if (Dba.id > 0)
                        {
                            is_found = true;
                        }
                    }

                }
                if (Rs.IsSuccess)
                {
                    //NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }

            if (!is_found)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "ไม่พบข้อมูล");
            }

            await SetDefaultValues();

            IsLoad = false;
        }

        async Task LoadTab(int index)
        {
            IsLoad = true;
            switch (index)
            {
                case 0:
                    {
                        
                    }
                    break;
                case 1:
                    {
                        
                    }
                    break;
                case 6:
                    {
                        
                    }
                    break;
            }
            IsLoad = false;
        }
    }
}
