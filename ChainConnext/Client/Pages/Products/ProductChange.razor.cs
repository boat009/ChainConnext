using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace ChainConnext.Client.Pages.Products
{
    public partial class ProductChange
    {
        [Parameter]
        public Contract_Info? ConInf { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
    }
}
