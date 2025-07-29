using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Contracts;
using ChainConnext.Shared.Payments;
using ChainConnext.Client.Services;
using Microsoft.JSInterop;

namespace ChainConnext.Client.Pages.Bills
{
    public partial class BillInfo
    {
        [Parameter]
        public Contract_Info? pConInf { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        int selectedIndex = 0;

        List<Payment_Info> payment_Infos = new List<Payment_Info>();
        bool isLoading = false;

        int Height;
        int Width;
        protected override async Task OnParametersSetAsync()
        {

            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;
        }
    }
}
