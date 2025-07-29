using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.ARs
{
    public partial class ARUrUvList
    {
        [Parameter]
        public BD_Debtora? pDbta { get; set; }
        [Parameter]
        public int pHeight { get; set; }
        [Parameter]
        public int pWidth { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        List<BD_debtorb> bD_Debtorbs = new List<BD_debtorb>();

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

            var postBody = new BD_debtorb { refno = pDbta.refno, contno = pDbta.contno, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/DebtorBList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Data != null)
                {
                    if (Rs.IsSuccess)
                    {
                        if (Rs.Rows > 0)
                        {
                            bD_Debtorbs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_debtorb>>(Rs.Data.ToString());
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
