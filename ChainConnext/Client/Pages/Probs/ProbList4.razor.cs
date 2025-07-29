using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Toss;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using ChainConnext.Client.Pages.Customers;
using ChainConnext.Shared.Customers;
using Radzen;
using BlazorStrap.V5;

namespace ChainConnext.Client.Pages.Probs
{
    public partial class ProbList4
    {
        [Parameter]
        public Contract_Info? pConInf { get; set; }
        [Parameter]
        public int pHeight { get; set; }
        [Parameter]
        public int pWidth { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        List<TOSS_Prob_Operation_Main> Pbm = new List<TOSS_Prob_Operation_Main>();
        List<TOSS_Prob_Operation_Detail> Pbd = new List<TOSS_Prob_Operation_Detail>();

        IList<TOSS_Prob_Operation_Main>? selectedPbm;

        bool IsLoading = false;
        bool IsDetailLoading = false;
        Authens userData = new Authens();

        protected override async Task OnParametersSetAsync()
        {
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            await ListLogData();
        }

        private async Task ListLogData()
        {
            if (pConInf == null)
            {
                return;
            }
            if (pConInf.RefNo == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pConInf.RefNo.Trim()))
            {
                return;
            }

            IsLoading = true;

            var postBody = new Contract_Info { RefNo = pConInf.RefNo, UserData = userData };
            var response = await Http.PostAsJsonAsync("Toss/ProbOperationMainList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    Pbm = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TOSS_Prob_Operation_Main>>(Rs.Data.ToString());
                }
            }
            IsLoading = false;
        }

        async Task OnPbmCellClick(DataGridCellMouseEventArgs<TOSS_Prob_Operation_Main> args)
        {
            IsDetailLoading = true;

            args.Data.UserData = userData;

            var response = await Http.PostAsJsonAsync("Toss/ProbOperationDetailList", args.Data);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Data != null)
                {
                    Pbd = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TOSS_Prob_Operation_Detail>>(Rs.Data.ToString());
                }
            }
            IsDetailLoading = false;
        }
    }
}
