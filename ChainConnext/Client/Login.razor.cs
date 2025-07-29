using BrowserInterop.Extensions;
using BrowserInterop.Geolocation;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Json;
using ChainConnext.Client.AuthProviders;
using System.Runtime.CompilerServices;
using ChainConnext.Shared.BD;
using System.Reflection.PortableExecutable;
using System;

namespace ChainConnext.Client
{
    public partial class Login
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }

        LoginRequest login_req = new LoginRequest();

        string LblServerName = "";
        string Versoin = "";
        string Lblversion = "";
        string Header = "";

        protected override async Task OnInitializedAsync()
        {
            login_req.RememberMe = true;

            await GetServerDetail();

            await FindResponsiveness();
            var window = await jsRuntime.Window();
            var navigator = await window.Navigator();
            geolocationWrapper = navigator.Geolocation;
            //await GetGeolocation();
            //string token = await localStorage.GetItemAsync<string>("token");
            //if (token != null)
            //{
            //    //await localStorage.RemoveItemAsync("token");
            //    Navigation.NavigateTo(Navigation.BaseUri);
            //}
        }

        async Task GetServerDetail()
        {
            var postBody = new ExecResult();
            var response = await Http.PostAsJsonAsync("Support/GetDBServerName", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                LblServerName = Rs.Msg;
                Versoin = Rs.ID;
                BaseShared.ServerPrefix = Rs.Msg;
                BaseShared.AppVersion = Rs.ID;
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
        }

        async Task GetClaims()
        {
            var authState = await authenticationState;
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                var data = user.Identity.GetData();
            }
        }
        private string? isDevice { get; set; }
        private bool mobile { get; set; }
        string VdoHeight = "100%";
        string VdoWidth = "100%";
        public async Task FindResponsiveness()
        {
            mobile = await jsRuntime.InvokeAsync<bool>("isDevice");
            isDevice = mobile ? "Mobile" : "Desktop";
            if (mobile)
            {
                VdoHeight = "100%";
                VdoWidth = "100%";
            }
            else
            {
                VdoHeight = "50%";
                VdoWidth = "50%";
            }
        }

        string login_msg = "";
        string userName = "";
        string password = "";
        bool rememberMe = true;
        string latt = "";
        string longtt = "";

        bool IsLoad = false;

        async Task OnLogin(LoginArgs args, string name)
        {
            IsLoad = true;
            StateHasChanged();
            //if (currentPosition == null)
            //{
            //    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = "เปิด Location ด้วย", Duration = 4000 });
            //    return;
            //}
            await ShowBusyDialog(args);

            //await LogInCheck(args);

            if (!string.IsNullOrEmpty(login_msg.Trim()))
            {
                //await dialogService.Alert(login_msg, "Login Error!!", new AlertOptions() { OkButtonText = "OK" });
                //ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = login_msg, Duration = 4000 });
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = login_msg, Duration = 4000 });
            }
            IsLoad = false;
        }

        async Task LogInCheck(LoginArgs args)
        {
            //await GetGeolocation();

            //await Task.Delay(10000);
            //Logger.LogError("Delay Complete");
            //NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = login_msg, Duration = 4000 });
            //return;

            login_msg = "";
            //Logger.LogInformation($"{name} -> Username: {args.Username}, password: {args.Password}, remember me: {args.RememberMe}");
            string PassEnc = "";
            var postBody = new LoginRequest { UserName = "Encrypt", Password = args.Password, CurrentURL = Navigation.Uri };
            var response = await Http.PostAsJsonAsync("Authen/GetEncPassWord", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Msg != null)
                    {
                        PassEnc = Rs.Msg;
                    }
                }
            }

            LoginRequest? login = new LoginRequest();
            login.UserName = args.Username;
            login.Password = PassEnc;
            login.Latt = latt;
            login.Longtt = longtt;
            login.CurrentURL = Navigation.Uri;
            login.RememberMe = args.RememberMe;
            var auth = await _accountService.LoginAsync(login);

            #region ใช้อันนี้ก็ได้
            /*
                var authState = await authenticationState;
                var user = authState.User;


                bool authenticated = user.Identity?.IsAuthenticated ?? false;
                */
            #endregion

            if (auth.Item1)
            {
                //await GetGeolocation();
                //Navigation.NavigateTo(Navigation.ToBaseRelativePath(Navigation.Uri));
                Navigation.NavigateTo(Navigation.BaseUri);
            }
            else
            {
                login_msg = auth.Item2;
                Logger.LogError(login_msg);

                var alert = dialogService.Alert(login_msg, "Authen fail", new AlertOptions() { OkButtonText = "OK" });

                
                //NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = login_msg, Duration = 4000 });
            }
        }

        async Task ShowBusyDialog(LoginArgs args)
        {
            InvokeAsync(async () =>
            {
                // Simulate background task

                await Task.Delay(5000);
                Logger.LogInformation("Delay Complete");
                //await LogInCheck(args);

                // Close the dialog
                dialogService.Close();
            });

            await BusyDialog("Login ...");
        }

        // Busy dialog from string
        async Task BusyDialog(string message)
        {
            await dialogService.OpenAsync("", ds =>
            {
                RenderFragment content = b =>
                {
                    b.OpenElement(0, "RadzenRow");

                    b.OpenElement(1, "RadzenColumn");
                    b.AddAttribute(2, "Size", "12");

                    b.AddContent(3, message);

                    b.CloseElement();
                    b.CloseElement();
                };
                return content;
            }, new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;width:auto", CloseDialogOnEsc = false });
        }

        void ShowNotification(NotificationMessage message)
        {
            NotificationService.Notify(message);

            Logger.LogInformation($"{message.Severity} notification");
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
            if (currentPosition == null)
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = "เปิด Location ด้วย", Duration = 4000 });
            }
            else
            {
                latt = currentPosition.Coords.Latitude.ToString();
                longtt = currentPosition.Coords.Longitude.ToString();

            }
        }

        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            //NotificationService.Notify(NotificationSeverity.Error, "Error", $"InvalidSubmit: {JsonSerializer.Serialize(args, new JsonSerializerOptions() { WriteIndented = true })}");
        }
        bool IsLoading = false;
        async Task OnSubmit(LoginRequest model)
        {
            IsLoading = true;

            //await Task.Delay(5000);
            //Logger.LogInformation("OnSubmit Delay Complete");

            login_msg = "";
            //Logger.LogInformation($"{name} -> Username: {args.Username}, password: {args.Password}, remember me: {args.RememberMe}");
            string PassEnc = "";
            var postBody = new LoginRequest { UserName = "Encrypt", Password = model.Password, CurrentURL = Navigation.Uri };
            var response = await Http.PostAsJsonAsync("Authen/GetEncPassWord", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Msg != null)
                    {
                        PassEnc = Rs.Msg;
                    }
                }
            }

            LoginRequest? login = new LoginRequest();
            login.UserName = model.UserName;
            login.Password = PassEnc;
            login.Latt = latt;
            login.Longtt = longtt;
            login.CurrentURL = Navigation.Uri;
            login.RememberMe = model.RememberMe;
            var auth = await _accountService.LoginAsync(login);

            if (auth.Item1)
            {
                //await GetGeolocation();
                //Navigation.NavigateTo(Navigation.ToBaseRelativePath(Navigation.Uri));
                Navigation.NavigateTo(Navigation.BaseUri);
            }
            else
            {
                login_msg = $"{auth.Item2} // รหัสไม่ถูก หรือ รหัสหมดอายุ";

                var alert = await dialogService.Alert(login_msg, "Authen fail", new AlertOptions() { OkButtonText = "OK" });
                
                Logger.LogError(login_msg);

                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = login_msg, Duration = 5000 });
            }

            IsLoading = false;
        }
    }
}
