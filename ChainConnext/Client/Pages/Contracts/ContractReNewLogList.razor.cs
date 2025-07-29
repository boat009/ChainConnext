using ChainConnext.Shared.BD;
using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Contracts
{
    public partial class ContractReNewLogList
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
        public List<BD_ChgModel> bD_ChgModels { get; set; } = new List<BD_ChgModel>();
        [Parameter]
        public bool IsLoading { get; set; } = false;

        //protected override async Task OnInitializedAsync()
        //{
        //    await ListChgModelLogData();
        //}

        //protected override async Task OnParametersSetAsync()
        //{
        //    //await ListChgModelLogData();
        //}

        private async Task ListChgModelLogData()
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

            IsLoading = true;

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new BD_ChgModel { RefNo = pConInf.RefNo, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/BDListChangeModel", postBody);
            //var response = await Http.PostAsJsonAsync("BD/ListChgCont", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    bD_ChgModels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_ChgModel>>(Rs.Data.ToString());
                }
            }
            IsLoading = false;
        }
    }
}
