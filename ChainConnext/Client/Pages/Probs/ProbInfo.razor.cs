using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Contracts;

namespace ChainConnext.Client.Pages.Probs
{
    public partial class ProbInfo
    {
        [Parameter]
        public Contract_Info? pConInf { get; set; }
        [Parameter]
        public int pHeight { get; set; }
        [Parameter]
        public int pWidth { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        int selectedIndex = 0;
    }
}
