using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Settings
{
    public partial class SetProKindDialog
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        [Parameter]
        public string? code { get; set; }
        [Parameter]
        public string? Key { get; set; }

        BDProKind proKind = new BDProKind();
        bool IsLoading = false;

        string UserID = "";
        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            await CheckPermission();

            await GetData();

            IsLoading = false;
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

        private async Task GetData()
        {
            if (code == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(code.Trim()))
            {
                return;
            }
            var postBody = new BDProKind { code = code };
            var response = await Http.PostAsJsonAsync("BD/ListProKind", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        proKind = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BDProKind>>(Rs.Data.ToString()).FirstOrDefault();
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
        }

        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            //NotificationService.Notify(NotificationSeverity.Error, "Error", $"InvalidSubmit: {JsonSerializer.Serialize(args, new JsonSerializerOptions() { WriteIndented = true })}");
        }

        async Task OnSubmit(BDProKind model)
        {
            model.CreateBy = userData.UserID;
            var response = await Http.PostAsJsonAsync("BD/SaveProKind", model);
            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    dialogService.Close(Rs);
                }
            }
        }
    }
}
