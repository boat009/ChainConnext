using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Contracts;

namespace ChainConnext.Client.Pages.Products
{
    public partial class ProductInfo
    {
        [Parameter]
        public Contract_Info? ConInf { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        int selectedIndex = 0;
        DateTime? value = DateTime.Now;
    }
}
