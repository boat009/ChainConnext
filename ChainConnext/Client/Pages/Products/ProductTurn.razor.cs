using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.StockDB;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Products
{
    public partial class ProductTurn
    {
        [Parameter]
        public Contract_Info? pConInf { get; set; }
        [Parameter]
        public int pHeight { get; set; }
        [Parameter]
        public int pWidth { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        List<STK_ProductTurnExchange> sTK_ProductTurnExchanges = new List<STK_ProductTurnExchange>();
        bool IsLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            await ListProductTurnExchangeData();
        }

        private async Task ListProductTurnExchangeData()
        {
            if (pConInf == null)
            {
                return;
            }
            if (pConInf.SerialNo == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pConInf.SerialNo.Trim()))
            {
                return;
            }

            IsLoading = true;

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new STK_ProductTurnExchange { NewSerial = pConInf.SerialNo, UserData = userData };
            var response = await Http.PostAsJsonAsync("STK/ListProductTurnExchange", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Data != null)
                {
                    sTK_ProductTurnExchanges = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STK_ProductTurnExchange>>(Rs.Data.ToString());
                }
            }
            IsLoading = false;
        }
    }
}
