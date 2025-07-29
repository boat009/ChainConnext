using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.Payments;
using ChainConnext.Shared;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Contracts
{
    public partial class ContractCloneLogList
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
        public List<BD_ChgCont> bD_ChgConts { get; set; } = new List<BD_ChgCont>();
        [Parameter]
        public bool IsLoading { get; set; } = false;

        string OldRefNoTitle = "มาจากเลขอ้างอิง";
        string OldContNoTitle = "มาจากเลขสัญญา";

        BD_ChgCont bdc = new BD_ChgCont();

        //protected override async Task OnInitializedAsync()
        //{
        //    await ListLogData();
        //}

        protected override async Task OnParametersSetAsync()
        {
            if (pConInf == null)
            {
                return;
            }
            if (pConInf.ContractNo == null)
            {
                return;
            }
            await Task.Run(() =>
            {
                if (bD_ChgConts.Count > 0)
                {
                    bdc = bD_ChgConts[0];
                }

                if (pConInf.ContractNo.Contains("?"))
                {
                    OldRefNoTitle = "ไปยังเลขอ้างอิง";
                    OldContNoTitle = "ไปยังเลขสัญญา";
                }
                else
                {
                    OldRefNoTitle = "มาจากเลขอ้างอิง";
                    OldContNoTitle = "มาจากเลขสัญญา";
                }
            });
        }

        private async Task ListLogData()
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

            Console.WriteLine("IsLoading");

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new BD_ChgCont { RefNo = pConInf.RefNo, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/ListChgCont", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    bD_ChgConts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_ChgCont>>(Rs.Data.ToString());
                }
            }
            IsLoading = false;
        }
    }
}
