using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using ChainConnext.Shared.Customers;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using ChainConnext.Shared.Authen;
using Microsoft.AspNetCore.Components.Authorization;
using ChainConnext.Shared.Infos;
using Radzen;

namespace ChainConnext.Client.Pages.Customers
{
    public partial class CustomerTelephone
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        [Parameter]
        public string? pCustomerId { get; set; }
        [Parameter]
        public string? pTelId { get; set; }
        [Parameter]
        public string? pContractId { get; set; }
        [Parameter]
        public string? Key { get; set; }
        List<Customer_Telephone_Type> TelType = new List<Customer_Telephone_Type>();

        Customer_Telephone customer_Telephone = new Customer_Telephone();

        Authens userData = new Authens();
        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        protected override async Task OnInitializedAsync()
        {
            await CheckPermission();

            await GetTelephoneTypeData();
            await GetTelephoneData();
        }
        bool IsSaveCust = false;
        async Task CheckPermission()
        {
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (userData.PermsList.Count > 0)
            {
                var perms = userData.PermsList.Find(x => x.PermsName == "IsSave");
                if (perms != null)
                {
                    IsSave = perms.IsPerms;
                }
                perms = userData.PermsList.Find(x => x.PermsName == "IsDelete");
                if (perms != null)
                {
                    IsDelete = perms.IsPerms;
                }
            }
            if (userData.MenuList.Count > 0)
            {
                var menu2 = userData.MenuList.Find(x => x.MenuId == 78);
                if (menu2 != null)
                {
                    IsSaveCust = menu2.IsSave;
                }
            }
        }

        private async Task GetTelephoneTypeData()
        {
            var postBody = new Customer_Telephone_Type();
            var response = await Http.PostAsJsonAsync("Customer/ListTelephoneType", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    TelType = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer_Telephone_Type>>(Rs.Data.ToString());
                }
            }
        }

        private async Task GetTelephoneData()
        {
            if (pTelId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pTelId))
            {
                return;
            }

            var postBody = new Customer_Telephone { CustomerId = pCustomerId, TelId = pTelId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Customer/ListTelephone", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    customer_Telephone = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer_Telephone>>(Rs.Data.ToString()).FirstOrDefault();
                }
            }
        }

        async Task OnSave()
        {
            customer_Telephone.UserData = userData;
            customer_Telephone.CreatedBy = userData.UserID;
            customer_Telephone.ContractId = pContractId;

            var response = await Http.PostAsJsonAsync("Customer/SaveTelephone", customer_Telephone);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    dialogService.Close(Rs);
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
        }
    }
}
