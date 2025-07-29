using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using ChainConnext.Shared.Customers;
using Microsoft.AspNetCore.Components;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.Contracts;
using Radzen;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Components.Authorization;
using ChainConnext.Shared.Infos;

namespace ChainConnext.Client.Pages.Customers
{
    public partial class CustomerInfo
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        [Parameter]
        public string? pCustomerId { get; set; }
        [Parameter]
        public string? pContractId { get; set; }

        Customer_Info CustInf = new Customer_Info();
        List<Customer_Title> customer_Titles = new List<Customer_Title>();
        List<Customer_CardType> customer_CardTypes = new List<Customer_CardType>();
        List<Customer_Address> customer_Addresses = new List<Customer_Address>();
        List<Customer_Telephone> customer_Telephones = new List<Customer_Telephone>();
        bool isLoading = false;

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();

        List<Aging_Info> aging_Infos = new List<Aging_Info>();

        IList<Customer_Address>? selectedAddr;
        IList<Customer_Telephone>? selectedTel;

        bool DisableSave = true;

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation("CustomerInfo OnInitializedAsync");

            await CheckPermission();
            await GetTitleData();
            await GetCardTypeData();
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

            if (pCustomerId == null)
            {
                DisableSave = true;
            }
            else
            {
                DisableSave = false;
            }
        }

        private async Task GetTitleData()
        {
            var postBody = new Customer_Title();
            var response = await Http.PostAsJsonAsync("Customer/ListTitle", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    customer_Titles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer_Title>>(Rs.Data.ToString());
                    CustInf.Title = customer_Titles[0].TitleID;
                }
            }
        }
        private async Task GetCardTypeData()
        {
            var postBody = new Customer_CardType();
            var response = await Http.PostAsJsonAsync("Customer/ListCardType", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    customer_CardTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer_CardType>>(Rs.Data.ToString());
                }
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await GetCustomerData();
            await GetAddressData();
            await GetTelephoneData();
        }
        private async Task GetCustomerData()
        {
            if (pCustomerId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pCustomerId.Trim()))
            {
                return;
            }
            isLoading = true;
            Authens userData = new Authens();

            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            var postBody = new Customer_Info { CustomerId = pCustomerId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Customer/ListCustomerById", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    CustInf = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer_Info>>(Rs.Data.ToString()).FirstOrDefault();
                }
            }
            isLoading = false;
        }
        private async Task GetAddressData()
        {
            if (pCustomerId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pCustomerId.Trim()))
            {
                return;
            }
            isLoading = true;
            Authens userData = new Authens();

            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            var postBody = new Customer_Address { CustomerId = pCustomerId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Customer/ListAddress", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    customer_Addresses = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer_Address>>(Rs.Data.ToString());
                }
            }
            isLoading = false;
        }
        private async Task GetTelephoneData()
        {
            if (pCustomerId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pCustomerId.Trim()))
            {
                return;
            }
            isLoading = true;

            Authens userData = new Authens();

            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            var postBody = new Customer_Telephone { CustomerId = pCustomerId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Customer/ListTelephone", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    customer_Telephones = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer_Telephone>>(Rs.Data.ToString());
                }
            }
            isLoading = false;
        }

        async Task OpenAddressEdit(Customer_Info? daTa, string key)
        {
            if (daTa == null)
            {
                daTa = new Customer_Info();
            }
            ExecResult Rs = new ExecResult();

            Rs = await dialogService.OpenAsync<CustomerAddress>($"{key} : ข้อมูลที่อยู่",
                  new Dictionary<string, object>() { { "pCustomerId", daTa.CustomerId }, { "pContractId", pContractId }, { "Key", key } },
                  new DialogOptions() { Width = "800px", Height = "400px" });
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    await GetAddressData();
                }
            }
        }

        async Task OpenTelephoneEdit(Customer_Info? daTa, string key)
        {
            if (daTa == null)
            {
                daTa = new Customer_Info();
            }
            ExecResult Rs = new ExecResult();

            Rs = await dialogService.OpenAsync<CustomerTelephone>($"{key} : ข้อมูลเบอร์โทร",
                  new Dictionary<string, object>() { { "pCustomerId", daTa.CustomerId }, { "pContractId", pContractId }, { "Key", key } },
                  new DialogOptions() { Width = "800px", Height = "150px" });
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    await GetTelephoneData();
                }
            }
        }

        async Task OnAddrCellClick(DataGridCellMouseEventArgs<Customer_Address> args)
        {
            string key = "แก้ไข";
            ExecResult Rs = new ExecResult();

            Rs = await dialogService.OpenAsync<CustomerAddress>($"{key} : ข้อมูลที่อยู่",
                  new Dictionary<string, object>() { { "pCustomerId", args.Data.CustomerId }, { "pContractId", pContractId }, {"pAddressId", args.Data.AddressId }, { "Key", key } },
                  new DialogOptions() { Width = "800px", Height = "400px" });
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    await GetAddressData();
                }
            }
        }
        async Task OnTelellClick(DataGridCellMouseEventArgs<Customer_Telephone> args)
        {
            string key = "แก้ไข";
            ExecResult Rs = new ExecResult();

            Rs = await dialogService.OpenAsync<CustomerTelephone>($"{key} : ข้อมูลเบอร์โทร",
                  new Dictionary<string, object>() { { "pCustomerId", args.Data.CustomerId }, { "pContractId", pContractId }, { "pTelId", args.Data.TelId }, { "Key", key } },
                  new DialogOptions() { Width = "800px", Height = "150px" });
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    await GetTelephoneData();
                }
            }
        }

        async Task OnSave()
        {
            CustInf.UserData = userData;
            CustInf.CreatedBy = userData.UserID;
            CustInf.ContractId = pContractId;

            var response = await Http.PostAsJsonAsync("Customer/SaveCustomerMini", CustInf);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                    await GetCustomerData();
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
        }
    }
}
