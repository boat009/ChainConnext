using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Toss;

namespace ChainConnext.Client.Pages.Probs
{
    public partial class ProbList3
    {
        [Parameter]
        public Contract_Info? pConInf { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        List<Problem_Info> problem_Infos = new List<Problem_Info>();
        bool isLoading = false;
    }
}
