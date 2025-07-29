using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.ARs
{
    public partial class ARAgingList
    {
        [Parameter]
        public BD_Debtora? pDbta { get; set; }
        [Parameter]
        public int pHeight { get; set; }
        [Parameter]
        public int pWidth { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        List<BD_debtoragi> bD_Debtoragis = new List<BD_debtoragi>();

        bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            if (pDbta == null)
            {
                return;
            }

            await PaymentListData();
        }

        private async Task PaymentListData()
        {
            if (pDbta == null)
            {
                return;
            }
            if (pDbta.refno == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pDbta.refno.Trim()))
            {
                return;
            }

            Console.WriteLine("IsLoading");

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new BD_debtoragi { refno = pDbta.refno, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/DebtorAgiList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    if (Rs.IsSuccess)
                    {
                        if (Rs.Rows > 0)
                        {
                            bD_Debtoragis = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_debtoragi>>(Rs.Data.ToString());
                        }
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
}
