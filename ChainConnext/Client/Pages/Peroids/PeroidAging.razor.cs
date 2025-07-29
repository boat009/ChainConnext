using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.Contracts;
using ChainConnext.Shared;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Peroids
{
    public partial class PeroidAging
    {
        [Parameter]
        public string? pContractId { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        Contract_Aging Caging = new Contract_Aging();
        bool isLoading = false;
        protected override async Task OnParametersSetAsync()
        {
            Logger.LogInformation("PeroidAging OnParametersSetAsync");
            await GetAgingDetailData();
        }

        private async Task GetAgingDetailData()
        {
            //Debtoragi
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
            var response = await Http.PostAsJsonAsync("Contract/AgingDetail", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    Caging = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Aging>>(Rs.Data.ToString()).FirstOrDefault();
                }
            }
            isLoading = false;
        }
    }
}
