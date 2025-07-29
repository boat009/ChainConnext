using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Radzen;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace ChainConnext.Client.Pages.ARs
{
    public partial class ARAccStatusDetailList
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        Authens userData = new Authens();
        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        [Parameter]
        public BD_accstatus? pAccs { get; set; }
        [Parameter]
        public List<BD_accstatus> ListDetail { get; set; } = new List<BD_accstatus>();
        [Parameter]
        public bool IsLoad { get; set; } = false;
        [Parameter]
        public int Height { get; set; }
        [Parameter]
        public int Width { get; set; }
        IList<BD_accstatus>? selectedDetail;
        [Parameter]
        public DateTime? pEffdate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await CheckPermission();
            await LoadAccStatusEffDateDetailList();
        }

        async Task CheckPermission()
        {
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (userData.MenuList.Count > 0)
            {
                var menu = userData.MenuList.Find(x => x.MenuId == 31);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsSave;
                }
            }
        }

        async Task LoadAccStatusEffDateDetailList()
        {
            if (pEffdate == null)
            {
                return;
            }
            IsLoad = true;
            StateHasChanged();

            var postBody = new BD_accstatus { effdate = pEffdate, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/AccStatusEffDateDetailList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        ListDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_accstatus>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                    Logger.LogError(Rs.Msg);
                }
            }
            IsLoad = false;
            StateHasChanged();
        }
    }
}
