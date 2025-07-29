using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Supports;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using System.Net.Http.Json;
using System.Threading.Channels;

namespace ChainConnext.Client.Pages.Settings
{
    public partial class SetChanelSaleDialog
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        [Parameter]
        public long ID { get; set; }
        [Parameter]
        public string? Key { get; set; }
        Chanel chanel = new Chanel();
        List<NPT_Depart> nPT_Departs = new List<NPT_Depart>();

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

            await ListChanelDepData();

            await ListChanelData();

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
                var menu = userData.MenuList.Find(x => x.MenuId == 16);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        private async Task ListChanelDepData()
        {
            var postBody = new Chanel();
            var response = await Http.PostAsJsonAsync("BD/ListChanelDep", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    nPT_Departs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NPT_Depart>>(Rs.Data.ToString());
                }
            }
        }

        private async Task ListChanelData()
        {
            var postBody = new Chanel { ChanelDep = 0, id = ID };
            var response = await Http.PostAsJsonAsync("BD/ListChanel", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    var chanels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Chanel>>(Rs.Data.ToString());
                    if (chanels != null)
                    {
                        chanel = chanels.FirstOrDefault();
                    }
                }
            }
        }

        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            //NotificationService.Notify(NotificationSeverity.Error, "Error", $"InvalidSubmit: {JsonSerializer.Serialize(args, new JsonSerializerOptions() { WriteIndented = true })}");
        }

        async Task OnSubmit(Chanel model)
        {
            model.CreateBy = userData.UserID;
            var response = await Http.PostAsJsonAsync("BD/SaveChanel", model);
            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    dialogService.Close(Rs);
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
        }
    }
}
