using ChainConnext.Client.Services;
using ChainConnext.Shared.Authen;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ChainConnext.Shared.BD;
using Radzen;
using ChainConnext.Shared;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.ARs
{
    public partial class ARDailyContract
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        Authens userData = new Authens();
        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        int Height;
        int Width;
        bool IsLoad = false;
        DateTime? FindMonth = DateTime.Now.AddMonths(-1);

        List<BD_accstatus> ListDate = new List<BD_accstatus>();
        List<BD_accstatus> ListDetail = new List<BD_accstatus>();
        List<BD_MastCont> ListDetailMastCont = new List<BD_MastCont>();

        IList<BD_accstatus>? selectedMain;
        IList<BD_accstatus>? selectedDetail;
        IList<BD_MastCont>? selectedDetailMastCont;

        string Todebtor = "";
        bool IsTodebtor = false;

        int selectedIndex = 0;
        int selectedIndexDate = 0;

        protected override async Task OnInitializedAsync()
        {
            IsLoad = true;


            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            await CheckPermission();

            if (IsAccess)
            {
                await GetMainData();
            }

            IsLoad = false;
        }

        private async Task GetMainData()
        {
            IsLoad = true;

            ListDate = new List<BD_accstatus>();
            ListDetail = new List<BD_accstatus>();
            ListDetailMastCont = new List<BD_MastCont>();

            var postBody = new BD_accstatus { Month = FindMonth.Value.Month, Year = FindMonth.Value.Year, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/AccStatusEffDateList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        ListDate = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_accstatus>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                    Logger.LogError(Rs.Msg);
                }
            }
            IsLoad = false;
        }

        async Task CheckPermission()
        {
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (userData.MenuList.Count > 0)
            {
                var menu = userData.MenuList.Find(x => x.MenuId == 32);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsSave;
                }
            }
        }

        async Task OnCellClick(DataGridCellMouseEventArgs<BD_accstatus> args)
        {
            await LoadAccStatusEffDateDetailList(args.Data);
            await LoadAccStatusEffDateMastContDetailList(args.Data);
        }

        async Task LoadAccStatusEffDateDetailList(BD_accstatus x)
        {
            IsLoad = true;
            ListDetail = new List<BD_accstatus>();

            if (IsTodebtor)
            {
                Todebtor = "0";
            }
            else
            {
                Todebtor = "";
            }

            var postBody = new BD_accstatus { effdate = x.effdate, todebtor = Todebtor, UserData = userData };
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
        }

        bool IsLoadMc = false;
        async Task LoadAccStatusEffDateMastContDetailList(BD_accstatus x)
        {
            IsLoadMc = true;
            ListDetailMastCont = new List<BD_MastCont>();

            if (IsTodebtor)
            {
                Todebtor = "0";
            }
            else
            {
                Todebtor = "";
            }

            //Logger.LogInformation($"AccStatusEffDateMastContDetailList Start At : {DateTime.Now.ToString()}");

            var postBody = new BD_MastCont { EFFDATE = x.effdate, toacc = Todebtor, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/AccStatusEffDateMastContDetailList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation($"AccStatusEffDateMastContDetailList Result : {Rs.Msg} At : {DateTime.Now.ToString()}");
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        ListDetailMastCont = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_MastCont>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                    Logger.LogError(Rs.Msg);
                }
                //Logger.LogInformation($"AccStatusEffDateMastContDetailList Binding : {Rs.Msg} At : {DateTime.Now.ToString()}");
            }
            IsLoadMc = false;
        }

        async Task OnDateChange(DateTime? date)
        {
            if (date == null)
            {
                return;
            }
            if (date.Value.ToString("MMyyyy") == FindMonth.Value.ToString("MMyyyy"))
            {
                return;
            }
            FindMonth = date.Value;
            
            await GetMainData();
        }

        async Task OnButtonDateChange(int add_month)
        {
            FindMonth = FindMonth.Value.AddMonths(add_month);
            await GetMainData();
        }
    }
}
