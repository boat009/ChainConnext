using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace ChainConnext.Client.Pages.Peroids
{
    public partial class PeroidInfo
    {
        [Parameter]
        public string? pContractId { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        int selectedIndex = 0;
    }
}
