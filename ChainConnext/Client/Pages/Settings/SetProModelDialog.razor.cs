using ChainConnext.Client.Services;
using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using System;
using System.Net.Http.Json;
using System.Reflection;

namespace ChainConnext.Client.Pages.Settings
{
    public partial class SetProModelDialog
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        [Parameter]
        public string? ModelCode { get; set; }
        [Parameter]
        public string? Cate { get; set; }
        [Parameter]
        public string? Key { get; set; }

        BDProModel proModel = new BDProModel();
        bool IsLoading = false;

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();
        List<BDProModel> KindDescList = new List<BDProModel>();
        List<BDProKind> proKinds = new List<BDProKind>();

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            CanSave = false;

            await CheckPermission();

            await ListKindDescData();
            await GetProKindData();

            IsLoading = false;

            if (Key == "Add")
            {
                proModel.cate = Cate;
                proModel.MODE = 1;
                await CheckSetModelValues();
                StateHasChanged();
            }
            else
            {
                await GetData();
            }
        }

        private async Task GetProKindData()
        {
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

        private async Task ListKindDescData()
        {
            var postBody = new BDProModel();
            var response = await Http.PostAsJsonAsync("BD/ListKindDesc", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        KindDescList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BDProModel>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
        }

        private async Task GetData()
        {
            if (ModelCode == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(ModelCode.Trim()))
            {
                return;
            }
            var postBody = new BDProModel { MODEL = ModelCode };
            var response = await Http.PostAsJsonAsync("BD/ListBDProModel", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        proModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BDProModel>>(Rs.Data.ToString()).FirstOrDefault();
                        await SetSetModelValues();
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
        }
        bool CanSave = false;
        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            CanSave = false;
            //NotificationService.Notify(NotificationSeverity.Error, "Error", $"InvalidSubmit: {JsonSerializer.Serialize(args, new JsonSerializerOptions() { WriteIndented = true })}");
        }
        bool CheckSave = false;
        async Task OnSubmit(BDProModel model)
        {
            CanSave = true;

            if (!CheckSave)
            {
                return;
            }
            
            proModel.CreateBy = userData.UserID;
            var response = await Http.PostAsJsonAsync("BD/SaveBDProModel", proModel);
            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    await SetSetModelValues();
                    dialogService.Close(Rs);
                }
            }
        }

        async Task CheckSetModelValues()
        {
            string Key = ShareValues.GetTokenUrl() + "_SetModel";
            try
            {
                BDProModel? pmd;
                pmd = await localStorage.GetItemAsync<BDProModel>(Key);
                if (pmd != null)
                {
                    proModel.Des = pmd.Des;
                    proModel.stdate = pmd.stdate;
                    proModel.CASH = pmd.CASH;
                    proModel.InvDesc = pmd.InvDesc;
                    proModel.vatdesc = pmd.vatdesc;
                    proModel.ErpItemCode = pmd.ErpItemCode;
                    proModel.KindDesc = pmd.KindDesc;
                }
            }
            catch
            {
                await localStorage.RemoveItemAsync(Key);
            }
        }

        async Task SetSetModelValues()
        {
            string Key = ShareValues.GetTokenUrl() + "_SetModel";
            try
            {
                await localStorage.SetItemAsync(Key, proModel);
            }
            catch
            {
                await localStorage.RemoveItemAsync(Key);
            }
        }

        async Task OnSave()
        {
            if (!CanSave)
            {
                return;
            }
            proModel.CreateBy = userData.UserID;
            var response = await Http.PostAsJsonAsync("BD/SaveBDProModel", proModel);
            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    await SetSetModelValues();
                    dialogService.Close(Rs);
                }
            }
        }

        async Task OnCalSales(decimal value, string name)
        {
            await Task.Run(() =>
            {
                proModel.Sales = (proModel.CREDIT * (proModel.MODE - 1)) + proModel.credit2;
            });
        }
    }
}
