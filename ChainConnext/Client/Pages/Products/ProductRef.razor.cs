using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using ChainConnext.Shared.Customers;

namespace ChainConnext.Client.Pages.Products
{
    public partial class ProductRef
    {
        [Parameter]
        public Contract_Info? ConInf { get; set; }

        List<Contract_Info> infos = new List<Contract_Info>();
        bool isLoading = false;
        protected override async Task OnParametersSetAsync()
        {
            Logger.LogInformation("ProductRef OnParametersSetAsync");

            await GetRefData();
        }

        private async Task GetRefData()
        {
            if (ConInf == null)
            {
                return;
            }
            if (ConInf.CustomerId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(ConInf.CustomerId.Trim()))
            {
                return;
            }

            isLoading = true;

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            var postBody = new Contract_Info { CustomerId = ConInf.CustomerId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Contract/GetContractRef", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Data != null)
                {
                    infos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Info>>(Rs.Data.ToString());
                }
            }

            isLoading = false;
        }
    }
}
