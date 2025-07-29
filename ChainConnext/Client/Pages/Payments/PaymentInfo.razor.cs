using ChainConnext.Shared.Contracts;
using ChainConnext.Shared;
using Radzen;
using ChainConnext.Shared.Payments;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using ChainConnext.Shared.Authen;

namespace ChainConnext.Client.Pages.Payments
{
    public partial class PaymentInfo
    {
        [Parameter]
        public string? pContractId { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        List<Payment_Info> payment_Infos = new List<Payment_Info>();
        bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            await PaymentListData();
        }

        private async Task PaymentListData()
        {
            if (pContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pContractId.Trim()))
            {
                return;
            }

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new Payment_Info { ContractId = pContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Payment/PaymentList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    payment_Infos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment_Info>>(Rs.Data.ToString());
                }
            }
        }

        async Task OpenEdit(Contract_Info? daTa, string key)
        {
            if (daTa == null)
            {
                daTa = new Contract_Info();
            }
            ExecResult Rs = new ExecResult();
            daTa.ContractId = Guid.NewGuid().ToString();
            Rs = await dialogService.OpenAsync<PaymentEditor>($"{key} : Payment",
                  new Dictionary<string, object>() { { "pContractId", daTa.ContractId }, { "Key", key } },
                  new DialogOptions() { Width = "1000px", Height = "800px" });
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    await PaymentListData();
                }
            }
        }
    }
}
