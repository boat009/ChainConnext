using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using ChainConnext.Shared.Customers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using ChainConnext.Shared.Infos;
using Microsoft.AspNetCore.Components.Authorization;
using ChainConnext.Shared.Authen;
using BlazorStrap.V5;
using Radzen;

namespace ChainConnext.Client.Pages.Customers
{
    public partial class CustomerAddress
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        [Parameter]
        public string? pCustomerId { get; set; }
        [Parameter]
        public string? pAddressId { get; set; }
        [Parameter]
        public string? pContractId { get; set; }
        [Parameter]
        public string? Key { get; set; }
        List<Customer_Address_Type> AddrType = new List<Customer_Address_Type>();

        List<Info_Amphur> info_Amphurs = new List<Info_Amphur>();
        List<Info_District> info_Districts = new List<Info_District>();
        List<Info_Province> info_Provinces = new List<Info_Province>();
        List<Info_Zipcode> info_Zipcodes = new List<Info_Zipcode>();

        Customer_Address customer_Address = new Customer_Address();

        bool isloaddata = false;

        Authens userData = new Authens();
        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        protected override async Task OnInitializedAsync()
        {
            isloaddata = true;
            await CheckPermission();

            await GetAddressTypeData();

            //await GetAmphurData();
            //await GetDistrictData();
            await GetProvinceData();
            //await GetZipcodeData();

            await GetAddressData();

            isloaddata = false;
            StateHasChanged();
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

        private async Task GetAddressData()
        {
            if (pAddressId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pAddressId))
            {
                return;
            }
            var postBody = new Customer_Address { CustomerId = pCustomerId, AddressId = pAddressId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Customer/ListAddress", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    customer_Address = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer_Address>>(Rs.Data.ToString()).FirstOrDefault();
                    await GetAmphurData();
                    await GetDistrictData();
                }
            }
        }
        private async Task GetAddressTypeData()
        {
            var postBody = new Customer_Address();
            var response = await Http.PostAsJsonAsync("Customer/ListAddressType", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    AddrType = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer_Address_Type>>(Rs.Data.ToString());
                }
            }
        }
        private async Task GetAmphurData()
        {
            var postBody = new Info_Amphur { Province_Code = customer_Address.AddressProvince1 };
            var response = await Http.PostAsJsonAsync("Info/ListAmphur", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    info_Amphurs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Info_Amphur>>(Rs.Data.ToString());
                }
            }
        }
        private async Task GetDistrictData()
        {
            var postBody = new Info_District { Province_Code = customer_Address.AddressProvince1, Amphur_Code = customer_Address.AddressDistrict1 };
            var response = await Http.PostAsJsonAsync("Info/ListDistrict", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    info_Districts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Info_District>>(Rs.Data.ToString());
                }
            }
        }
        private async Task GetProvinceData()
        {
            var postBody = new Info_Province();
            var response = await Http.PostAsJsonAsync("Info/ListProvince", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    info_Provinces = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Info_Province>>(Rs.Data.ToString());
                }
            }
        }
        private async Task GetZipcodeData()
        {
            var postBody = new Info_Zipcode { district_code = customer_Address.AddressSubdistrict1};
            var response = await Http.PostAsJsonAsync("Info/ListZipcode", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    info_Zipcodes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Info_Zipcode>>(Rs.Data.ToString());
                }
            }

            if (info_Zipcodes.Count == 1)
            {
                customer_Address.AddressZipcode = info_Zipcodes.FirstOrDefault().zipcode;
            }
        }

        async Task OnProvinceChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;

            Logger.LogInformation($"{name} value changed to {str}");

            await GetAmphurData();
        }
        async Task OnAmphurChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;

            Logger.LogInformation($"{name} value changed to {str}");

            await GetDistrictData();
        }
        async Task OnDistrictChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;

            Logger.LogInformation($"{name} value changed to {str}");

            await GetZipcodeData();
        }

        async Task OnSave()
        {
            customer_Address.UserData = userData;
            customer_Address.CreatedBy = userData.UserID;
            customer_Address.ContractId = pContractId;

            var Addr41 = info_Provinces.FindAll(x => x.Province_Code == customer_Address.AddressProvince1).FirstOrDefault();
            if (Addr41 != null)
            {
                customer_Address.AddressProvince = Addr41.Province_Name;
            }
            var Addr31 = info_Amphurs.FindAll(x => x.Province_Code == customer_Address.AddressProvince1 && x.Amphur_Code == customer_Address.AddressDistrict1).FirstOrDefault();
            if (Addr31 != null)
            {
                customer_Address.AddressDistrict = Addr31.Amphur_Name;
            }
            var Addr21 = info_Districts.FindAll(x => x.Province_Code == customer_Address.AddressProvince1 
            && x.Amphur_Code == customer_Address.AddressDistrict1 
            && x.District_Code == customer_Address.AddressSubdistrict1).FirstOrDefault();
            if (Addr21 != null)
            {
                customer_Address.AddressSubdistrict = Addr21.District_Name;
            }

            var response = await Http.PostAsJsonAsync("Customer/SaveAddress", customer_Address);

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
