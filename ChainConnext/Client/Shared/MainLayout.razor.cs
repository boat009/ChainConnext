using BrowserInterop.Extensions;
using BrowserInterop.Geolocation;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;
using Radzen;
using System.Security.Claims;
using ChainConnext.Client.Services;
using ChainConnext.Shared.BD;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using System;

namespace ChainConnext.Client.Shared
{
    public partial class MainLayout
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        public string? Header { get; set; }
        string Lblversion = "";
        string LblServerName = "";
        string Versoin = "";

        bool sidebar1Expanded = true;

        protected override async Task OnInitializedAsync()
        {
            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");

            //var window = await jsRuntime.Window();
            //var navigator = await window.Navigator();
            //geolocationWrapper = navigator.Geolocation;
            Versoin = BaseShared.AppVersion;
            LblServerName = BaseShared.ServerPrefix;
            Lblversion = $"Version {Versoin}";


            await base.OnInitializedAsync();
            //var authState = await authenticationState;
            //var user = authState.User;
            //if (user.Identity.IsAuthenticated)
            //{
            //    await GetServerDetail();
            //}
        }

        public async Task GetServerDetail()
        {
            var postBody = new ExecResult();
            var response = await Http.PostAsJsonAsync("Support/GetDBServerName", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                LblServerName = Rs.Msg;
                Versoin = Rs.ID;
                Lblversion = $"Version {Rs.ID}";
                if (LblServerName == "192.168.110.132")
                {
                    Header = "PRD ";// + Rs.JsonData;
                }
                else
                {
                    Header = "UAT ";// + Rs.JsonData;
                }
                if (Navigation.BaseUri.Contains("localhost"))
                {
                    Header = Header + "(Localhost Dev.)";
                }
            }

            StateHasChanged();
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            if (context.TargetLocation.ToLower().EndsWith("login"))
            {
                //var isConfirmed = await jsRuntime.InvokeAsync<bool>("confirm",
                //    "Are you sure you want to navigate to the Index page?");

                //if (!isConfirmed)
                //{
                //    context.PreventNavigation();
                //}
                context.PreventNavigation();
            }
            //Logger.LogInformation(context.TargetLocation);
            //if (context.TargetLocation.ToLower().EndsWith("contract"))
            //{
            //    await CheckPermMenu();
            //}
        }

        async Task CheckPermMenu()
        {
            try
            {
                await _accountService.UpdatePermsAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private string? isDevice { get; set; }
        private bool mobile { get; set; }
        public async Task FindResponsiveness()
        {
            mobile = await jsRuntime.InvokeAsync<bool>("isDevice");
            isDevice = mobile ? "Mobile" : "Desktop";

        }

        protected override async Task OnParametersSetAsync()
        {
            await FindResponsiveness();
            sidebar1Expanded = !mobile;
            StateHasChanged();
        }

        async void OnProfileMenuClicked(RadzenProfileMenuItem item)
        {
            Logger.Log(LogLevel.Information, $"{item.Text} clicked", item);
            if (item.Text == "Logout")
            {
                await Logout();
            }
            else if (item.Text == "Update Permission")
            {
                await CheckPermMenu();
            }
        }
        public async Task Logout()
        {
            //await GetGeolocation();
            string UserID = "";
            var authState = await authenticationState;
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                var UserData = Newtonsoft.Json.JsonConvert.DeserializeObject<Authens>(user.Identity.GetData());
                if (UserData != null)
                {
                    UserID = UserData.UserID;
                }
            }

            var postBody = new LogoutRequest { UserID = UserID };

            //var response = await Http.PostAsJsonAsync("Authen/Logout", postBody);
            //var UserMainData = await response.Content.ReadFromJsonAsync<Authens>();
            //if (UserMainData != null)
            //{
            //    //sidebar1Expanded = false;
            await _accountService.LogoutAsync(postBody);

            //await jsRuntime.InvokeVoidAsync("ClearCache");

            Navigation.NavigateTo("login");
            //}
            //Navigation.NavigateTo("logout");
        }

        void OnParentClicked(MenuItemEventArgs args)
        {
            //console.Log($"{args.Text} clicked from parent");
        }

        void OnChildClicked(MenuItemEventArgs args)
        {
            //console.Log($"{args.Text} from child clicked");
        }

        private WindowNavigatorGeolocation geolocationWrapper;
        private GeolocationPosition currentPosition;
        private List<GeolocationPosition> positioHistory = new List<GeolocationPosition>();
        private IAsyncDisposable geopositionWatcher;

        public async Task GetGeolocation()
        {
            currentPosition = (await geolocationWrapper.GetCurrentPosition(new PositionOptions()
            {
                EnableHighAccuracy = true,
                MaximumAgeTimeSpan = TimeSpan.FromHours(1),
                TimeoutTimeSpan = TimeSpan.FromMinutes(1)
            })).Location;
        }
    }
}
