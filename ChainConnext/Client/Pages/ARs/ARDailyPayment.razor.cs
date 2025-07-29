using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Authen;
using ChainConnext.Client.Services;
using Microsoft.JSInterop;
using System.Buffers;
using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Radzen;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.ARs
{
    public partial class ARDailyPayment
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

        List<BD_debtorpay> ListDate = new List<BD_debtorpay>();
        List<BD_debtorpay> ListDetail = new List<BD_debtorpay>();

        IList<BD_debtorpay>? selectedMain;
        IList<BD_debtorpay>? selectedDetail;

        string Posted = "";
        bool IsPosted = false;

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

        async Task CheckPermission()
        {
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (userData.MenuList.Count > 0)
            {
                var menu = userData.MenuList.Find(x => x.MenuId == 33);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsSave;
                }
            }
        }

        private async Task GetMainData()
        {
            IsLoad = true;

            ListDate = new List<BD_debtorpay>();
            ListDetail = new List<BD_debtorpay>();

            var postBody = new BD_debtorpay { Month = FindMonth.Value.Month, Year = FindMonth.Value.Year, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/DebtorPayDateList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        ListDate = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_debtorpay>>(Rs.Data.ToString());
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

        async Task OnCellClick(DataGridCellMouseEventArgs<BD_debtorpay> args)
        {
            IsLoad = true;
            ListDetail = new List<BD_debtorpay>();

            if (IsPosted)
            {
                Posted = "0";
            }
            else
            {
                Posted = "";
            }

            var postBody = new BD_debtorpay { paydate = args.Data.paydate, posted = Posted, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/DebtorPayDateDetailList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        ListDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_debtorpay>>(Rs.Data.ToString());
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
