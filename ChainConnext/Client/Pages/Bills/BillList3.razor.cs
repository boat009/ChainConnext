using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Payments;

namespace ChainConnext.Client.Pages.Bills
{
    public partial class BillList3
    {
        [Parameter]
        public Contract_Info? pConInf { get; set; }
        [Parameter]
        public int pHeight { get; set; }
        [Parameter]
        public int pWidth { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        List<Payment_Info> payment_Infos = new List<Payment_Info>();
        bool isLoading = false;
    }
}
