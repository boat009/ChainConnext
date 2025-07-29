using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Customers;
using ChainConnext.Shared;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using ChainConnext.Shared.Authen;
using BrowserInterop;
using ChainConnext.Client.Services;
using Radzen;

namespace ChainConnext.Client.Pages.Products
{
    public partial class ProductDetail
    {
        [Parameter]
        public Contract_Info? ConInf { get; set; }
        [Parameter]
        public int pHeight { get; set; }
        [Parameter]
        public int pWidth { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        
        List<ProModel> PmdData = new List<ProModel>();

        bool isLoading = false;
        bool isLoadingMain = false;

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        bool DisableSave = true;

        Authens userData = new Authens();

        protected override async Task OnInitializedAsync()
        {
            isLoadingMain = true;

            await CheckPermission();

            Logger.LogInformation("ProductDetail OnInitializedAsync");

            isLoadingMain = false;
        }
        bool IsSavePro = false;
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
                var menu = userData.MenuList.Find(x => x.MenuId == 19);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }

                var menu2 = userData.MenuList.Find(x => x.MenuId == 78);
                if (menu2 != null)
                {
                    IsSavePro = menu2.IsSave;
                }
            }
            if (ConInf != null)
            {
                if (ConInf.ContractId == null)
                {
                    DisableSave = true;
                }
                else
                {
                    DisableSave = false;
                }
            }
        }

        async Task LoadData(LoadDataArgs args)
        {
            await GetProModelData();

            await InvokeAsync(StateHasChanged);
        }

        private async Task GetProModelData()
        {
            if (ShareValues.PmdData.Count > 0)
            {
                PmdData = ShareValues.PmdData;
            }
            else
            {
                var postBody = new ProModel();
                var response = await Http.PostAsJsonAsync("BD/ListProModel", postBody);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.Data != null)
                    {
                        PmdData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProModel>>(Rs.Data.ToString());
                        if (PmdData != null)
                        {
                            ShareValues.PmdData = PmdData;
                        }
                    }
                }
            }
        }

        async Task OnModelChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;

            //Logger.LogInformation($"{name} value changed to {str}");

            ConInf.ModelDesc = PmdData.FindAll(x => x.MODEL == str).FirstOrDefault().ModelDesc2;
        }

        async Task FindModel()
        {
            ExecResult Rs = new ExecResult();

            Rs = await dialogService.OpenAsync<ProModelDialog>($"Search Model",
                  new Dictionary<string, object>(),
                  new DialogOptions() { Width = "800px", Height = "600px" });
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Data != null)
                    {
                        var md = (BDProModel)Rs.Data;
                        ConInf.Model = md.MODEL;
                        ConInf.ModelDesc = md.Des;
                    }
                }
            }
        }
    }
}
