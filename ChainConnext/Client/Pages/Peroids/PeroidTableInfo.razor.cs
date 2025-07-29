using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Contracts;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.Customers;
using ChainConnext.Shared;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Peroids
{
    public partial class PeroidTableInfo
    {
        [Parameter]
        public string? pContractId { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        List<Aging_Info> aging_Infos = new List<Aging_Info>();
        bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            Logger.LogInformation("PeroidTableInfo OnParametersSetAsync");
            await GetCustomerData();
        }

        private async Task GetCustomerData()
        {
            if (pContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pContractId.Trim()))
            {
                return;
            }
            isLoading = true;
            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            var postBody = new Aging_Info { ContractId = pContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Contract/ListAging", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    aging_Infos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Aging_Info>>(Rs.Data.ToString());
                }
            }
            isLoading = false;
        }
    }
}
