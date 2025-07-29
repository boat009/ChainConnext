using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Payments;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Bills
{
    public partial class BillList2
    {
        [Parameter]
        public Contract_Info? pConInf { get; set; }
        [Parameter]
        public int pHeight { get; set; }
        [Parameter]
        public int pWidth { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        [Parameter]
        public List<Payment_Transaction> payment_Infos { get; set; } = new List<Payment_Transaction>();
        [Parameter]
        public bool isLoading { get; set; } = false;

        //protected override async Task OnParametersSetAsync()
        //{
        //    await PaymentListData();
        //}

        private async Task PaymentListData()
        {
            if (pConInf == null)
            {
                return;
            }
            if (pConInf.ContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pConInf.ContractId.Trim()))
            {
                return;
            }

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new Payment_Transaction { ContractId = pConInf.ContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Payment/PaymentTransactionList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    payment_Infos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment_Transaction>>(Rs.Data.ToString());
                }
            }
        }
    }
}
