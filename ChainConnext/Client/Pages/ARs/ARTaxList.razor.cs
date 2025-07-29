using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.ARs
{
    public partial class ARTaxList
    {
        [Parameter]
        public BD_Debtora? pDbta { get; set; }
        [Parameter]
        public int pHeight { get; set; }
        [Parameter]
        public int pWidth { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        List<BD_mastvat> bD_Mastvats = new List<BD_mastvat>();

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

            var postBody = new BD_mastvat { RefNo = pDbta.refno, CONTNO = pDbta.contno, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/MastVatList", postBody);

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
                            bD_Mastvats = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_mastvat>>(Rs.Data.ToString());
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
