using ChainConnext.Client.Services;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Contracts;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Radzen;
using ChainConnext.Shared.Infos;
using Microsoft.Extensions.Logging;
using ChainConnext.Shared.Customers;
using Microsoft.JSInterop;
using System.Reflection;
using BlazorStrap;
using ChainConnext.Shared.Authen;
using BlazorStrap.V5;
using ChainConnext.Client.Pages.Products;

namespace ChainConnext.Client.Pages
{
    public partial class ContractNew2
    {
        [Parameter]
        public string? ContractId { get; set; }
        [Parameter]
        public string? Key { get; set; }

        Contract_Info_New ConInf = new Contract_Info_New();
        List<ProModel> PmdData = new List<ProModel>();
        List<BDProKind> proKinds = new List<BDProKind>();

        bool isloaddata = false;

        List<MasterErr> masterErrsN = new List<MasterErr>();
        List<MasterErr> masterErrsR = new List<MasterErr>();

        List<Info_Amphur> info_Amphurs = new List<Info_Amphur>();
        List<Info_District> info_Districts = new List<Info_District>();
        List<Info_Province> info_Provinces = new List<Info_Province>();
        List<Info_Zipcode> info_Zipcodes = new List<Info_Zipcode>();
        List<Info_Amphur> reg_Amphurs = new List<Info_Amphur>();
        List<Info_District> reg_Districts = new List<Info_District>();
        List<Info_Province> reg_Provinces = new List<Info_Province>();
        List<Info_Zipcode> reg_Zipcodes = new List<Info_Zipcode>();

        List<Contract_Status> contract_Statuses = new List<Contract_Status>();
        List<Branch> branches = new List<Branch>();
        List<Customer_Title> customer_Titles = new List<Customer_Title>();
        List<Customer_CardType> customer_CardTypes = new List<Customer_CardType>();

        bool CheckSave = false;
        bool CanDelete = false;

        Authens userData = new Authens();

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            isloaddata = true;

            await CheckPermission();
            if (IsAccess)
            {
                //await GetProKindData();

                await ErrsNData();
                await ErrsRData();

                await GetProvinceData();

                await ListContractStatus();
                await ListBranch();
                await GetTitleData();
                await GetCardTypeData();
            }

            isloaddata = false;
            StateHasChanged();
        }

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
                var path = Navigation.Uri.Replace(Navigation.BaseUri, "");
                var menu = userData.MenuList.Find(x => x.MenuId == 22);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }
        bool IsKindLoading = false;
        private async Task GetProKindData()
        {
            IsKindLoading = true;
            var postBody = new BDProKind();
            var response = await Http.PostAsJsonAsync("BD/ListProKind", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    proKinds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BDProKind>>(Rs.Data.ToString());
                }
            }
            IsKindLoading = false;
        }

        private async Task ListContractStatus()
        {
            var postBody = new Contract_Status();
            var response = await Http.PostAsJsonAsync("Contract/ListContractStatus", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    contract_Statuses = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Status>>(Rs.Data.ToString());
                }
            }
        }

        private async Task ListBranch()
        {
            var postBody = new Branch();
            var response = await Http.PostAsJsonAsync("BD/ListBranch", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    branches = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Branch>>(Rs.Data.ToString());
                }
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
                    ConInf.Title = customer_Titles[0].TitleID;
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
                if (Rs.Data != null)
                {
                    customer_CardTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer_CardType>>(Rs.Data.ToString());
                }
            }
        }

        private async Task ErrsNData()
        {
            var postBody = new MasterErr { kind = "N" };
            var response = await Http.PostAsJsonAsync("BD/ListErr", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    masterErrsN = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MasterErr>>(Rs.Data.ToString());
                }
            }
        }
        private async Task ErrsRData()
        {
            var postBody = new MasterErr { kind = "R" };
            var response = await Http.PostAsJsonAsync("BD/ListErr", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    masterErrsR = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MasterErr>>(Rs.Data.ToString());
                }
            }
        }
        private async Task GetAmphurData(string name)
        {
            string pProvince_Code = "";
            if (name == "reg")
            {
                pProvince_Code = ConInf.AddrReg41;
            }
            else
            {
                pProvince_Code = ConInf.Addr41;
            }
            var postBody = new Info_Amphur { Province_Code = pProvince_Code };
            var response = await Http.PostAsJsonAsync("Info/ListAmphur", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    if (name == "reg")
                    {
                        reg_Amphurs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Info_Amphur>>(Rs.Data.ToString());
                    }
                    else
                    {
                        info_Amphurs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Info_Amphur>>(Rs.Data.ToString());
                    }
                }
            }
        }
        private async Task GetDistrictData(string name)
        {
            string pProvince_Code = "";
            string pAmphur_Code = "";
            if (name == "reg")
            {
                pProvince_Code = ConInf.AddrReg41;
                pAmphur_Code = ConInf.AddrReg31;
            }
            else
            {
                pProvince_Code = ConInf.Addr41;
                pAmphur_Code = ConInf.Addr31;
            }
            var postBody = new Info_District { Province_Code = pProvince_Code, Amphur_Code = pAmphur_Code };
            var response = await Http.PostAsJsonAsync("Info/ListDistrict", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    if (name == "reg")
                    {
                        reg_Districts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Info_District>>(Rs.Data.ToString());
                    }
                    else
                    {
                        info_Districts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Info_District>>(Rs.Data.ToString());
                    }
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
                    reg_Provinces = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Info_Province>>(Rs.Data.ToString());
                }
            }
        }
        private async Task GetZipcodeData(string name)
        {
            string pDistrict_Code = "";
            if (name == "reg")
            {
                pDistrict_Code = ConInf.AddrReg2;
            }
            else
            {
                pDistrict_Code = ConInf.Addr2;
            }
            var postBody = new Info_Zipcode { district_code = pDistrict_Code };
            var response = await Http.PostAsJsonAsync("Info/ListZipcode", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    if (name == "reg")
                    {
                        reg_Zipcodes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Info_Zipcode>>(Rs.Data.ToString());
                    }
                    else
                    {
                        info_Zipcodes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Info_Zipcode>>(Rs.Data.ToString());
                    }
                }
            }
            if (name == "reg")
            {
                if (reg_Zipcodes.Count == 1)
                {
                    ConInf.AddrRegZip = reg_Zipcodes.FirstOrDefault().zipcode;
                }
            }
            else
            {
                if (info_Zipcodes.Count == 1)
                {
                    ConInf.AddrZip = info_Zipcodes.FirstOrDefault().zipcode;
                }
            }
        }

        async Task OnProvinceChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;

            //Logger.LogInformation($"{name} value changed to {str}");

            await GetAmphurData(name);
        }
        async Task OnAmphurChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;

            //Logger.LogInformation($"{name} value changed to {str}");

            await GetDistrictData(name);
        }
        async Task OnDistrictChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;

            //Logger.LogInformation($"{name} value changed to {str}");

            await GetZipcodeData(name);
        }

        DateTime? value = DateTime.Now;
        bool isLoading = false;
        async Task SubmitAsync(Contract_Info arg)
        {
            if (!CheckSave)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "ติ๊ก ยืนยันบันทึก ก่อน");
                return;
            }

            var AddrReg41 = reg_Provinces.FindAll(x => x.Province_Code == ConInf.AddrReg41).FirstOrDefault();
            if (AddrReg41 != null)
            {
                ConInf.AddrReg4 = AddrReg41.Province_Name;
            }
            var AddrReg31 = reg_Amphurs.FindAll(x => x.Province_Code == ConInf.AddrReg41 && x.Amphur_Code == ConInf.AddrReg31).FirstOrDefault();
            if (AddrReg31 != null)
            {
                ConInf.AddrReg3 = AddrReg31.Amphur_Name;
            }
            var AddrReg21 = reg_Districts.FindAll(x => x.Province_Code == ConInf.AddrReg41 && x.Amphur_Code == ConInf.AddrReg31 && x.District_Code == ConInf.AddrReg21).FirstOrDefault();
            if (AddrReg21 != null)
            {
                ConInf.AddrReg2 = AddrReg21.District_Name;
            }

            var Addr41 = info_Provinces.FindAll(x => x.Province_Code == ConInf.Addr41).FirstOrDefault();
            if (Addr41 != null)
            {
                ConInf.AddrReg4 = Addr41.Province_Name;
            }
            var Addr31 = info_Amphurs.FindAll(x => x.Province_Code == ConInf.Addr41 && x.Amphur_Code == ConInf.Addr31).FirstOrDefault();
            if (Addr31 != null)
            {
                ConInf.Addr3 = Addr31.Amphur_Name;
            }
            var Addr21 = info_Districts.FindAll(x => x.Province_Code == ConInf.Addr41 && x.Amphur_Code == ConInf.Addr31 && x.District_Code == ConInf.Addr21).FirstOrDefault();
            if (Addr21 != null)
            {
                ConInf.Addr2 = Addr21.District_Name;
            }

            isLoading = true;

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            ConInf.UserData = userData;
            ConInf.CreatedBy = userData.UserID;
            ConInf.UpdatedBy = userData.UserID;
            ConInf.ContractSale = ConInf.SaleCode;

            var response = await Http.PostAsJsonAsync("Contract/SaveNew", ConInf);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            isLoading = false;
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                    ConInf = new Contract_Info_New();
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
        }

        async Task OnChangeContractDiscount(decimal value, string name)
        {
            await Task.Run(() =>
            {

                ConInf.AfterDiscount = ConInf.Credit - value;
                ConInf.ContractFirstPay = ConInf.ContractFirstPeriodAmount - value;
            });
        }

        async Task OnChangeCitizenID(string value, string name)
        {
            if (string.IsNullOrEmpty(value.Trim()))
            {
                return;
            }

            Logger.LogInformation("Start CitizenId Find : " + value);

            value = value.Replace("-", "").Trim();
            if (value.Length < 13)
            {
                return;
            }
            ConInf.CitizenId = value;
            await FindCitizenId(null, "");
        }
        bool IsLoadCitizenId = false;
        async Task FindCitizenId(Contract_Info? daTa, string key)
        {
            IsLoadCitizenId = true;

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new Customer_Info { CitizenId = ConInf.CitizenId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Customer/ListCustomerByCitizenId", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    var md = Newtonsoft.Json.JsonConvert.DeserializeObject<Customer_Info>(Rs.Data.ToString());
                    if (md != null)
                    {
                        ConInf.CitizenId = md.CitizenId;
                        ConInf.Title = md.Title;
                        ConInf.FirstName = md.FirstName;
                        ConInf.LastName = md.LastName;
                        ConInf.CardTypeId = md.CardTypeId;
                        ConInf.BirthDate = md.BirthDate;
                        ConInf.CitizenExpireDate = md.CitizenExpireDate;
                        ConInf.CustomerId = md.CustomerId;
                    }
                }
            }

            IsLoadCitizenId = false;
        }
        bool IsLoadRefNo = false;
        async Task FindRefNo(string key)
        {
            IsLoadRefNo = true;

            var postBody = new Contract_Info();
            switch (key)
            {
                case "RefNo":
                    {
                        postBody.RefNo = ConInf.RefNo;
                    }
                    break;
                case "ContNo":
                    {
                        postBody.ContractNo = ConInf.ContractNo;
                    }
                    break;
            }
            postBody.UserData = userData;
            postBody.CreatedBy = userData.UserID;
            var response = await Http.PostAsJsonAsync("Contract/FindSaveNewData", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    ConInf = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Info_New>>(Rs.Data.ToString()).FirstOrDefault();
                    if (ConInf != null)
                    {
                        if (ConInf.SaleCode != null)
                        {
                            await OnGetName(ConInf.SaleCode, "SaleName");
                        }
                        if (ConInf.SetupCode != null)
                        {
                            await OnGetName(ConInf.SetupCode, "SetupName");
                        }
                        if (ConInf.CheckerCode != null)
                        {
                            await OnGetName(ConInf.CheckerCode, "CheckerName");
                        }
                        if (ConInf.ServiceCode != null)
                        {
                            await OnGetName(ConInf.ServiceCode, "ServiceName");
                        }
                        if (ConInf.CashCode != null)
                        {
                            await OnGetName(ConInf.CashCode, "CashName");
                        }

                        if (ConInf.Model != null)
                        {
                            await GetProModelName(ConInf.Model);
                        }

                        await GetAmphurData("reg");
                        await GetDistrictData("reg");

                        await GetAmphurData("card");
                        await GetDistrictData("card");
                    }
                }
            }

            IsLoadRefNo = false;
        }

        async Task OnChange(string value, string name)
        {
            //Logger.LogInformation($"{name} value changed to {value}");
            if (!string.IsNullOrEmpty(value.Trim()))
            {

                switch (name)
                {
                    case "RefNo":
                        {
                            ConInf.RefNo = BaseShared.FillRefNo(value.Trim(), 9);
                        }
                        break;
                    case "ContNo":
                        {
                            ConInf.ContractNo = BaseShared.FillRefNo(value.Trim(), 8).ToUpper();
                        }
                        break;
                }
                await FindRefNo(name);
            }
        }
        string Cate = "";
        bool IsKindSelected = false;
        async Task OnModelCate(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;

            IsKindSelected = true;
            PmdData = new List<ProModel>();
            await GetProModelData(value.ToString().Trim());
        }
        bool IsModelLoading = false;
        private async Task GetProModelData(string cate)
        {
            IsModelLoading = true;
            var postBody = new BDProModel { cate = cate };
            var response = await Http.PostAsJsonAsync("BD/ListBDProModel", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    PmdData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProModel>>(Rs.Data.ToString());
                }
            }
            IsModelLoading = false;
        }
        private async Task GetProModelName(string model)
        {
            IsModelLoading = true;
            var postBody = new BDProModel { MODEL = model };
            var response = await Http.PostAsJsonAsync("BD/ListBDProModel", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    var pmd = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProModel>>(Rs.Data.ToString()).FirstOrDefault();
                    if (pmd != null)
                    {
                        ConInf.ModelDesc = pmd.ModelDesc;
                    }
                }
            }
            IsModelLoading = false;
        }

        void UseAddrReg()
        {
            ConInf.Addr1 = ConInf.AddrReg1;
            ConInf.Addr2 = ConInf.AddrReg2;
            ConInf.Addr3 = ConInf.AddrReg3;
            ConInf.Addr4 = ConInf.AddrReg4;
        }

        void UseSaleCodeName()
        {
            ConInf.ServiceCode = ConInf.SaleCode;
            ConInf.ServiceName = ConInf.SaleName;
        }

        async Task OnGetName(string value, string name)
        {
            if (string.IsNullOrEmpty(value.Trim()))
            {
                return;
            }
            var postBody = new BDSaleName { SaleCode = value };
            var response = await Http.PostAsJsonAsync("BD/FindSaleName", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    var Sale = Newtonsoft.Json.JsonConvert.DeserializeObject<BDSaleName>(Rs.Data.ToString());
                    if (Sale != null)
                    {
                        switch (name)
                        {
                            case "SaleName":
                                {
                                    ConInf.SaleName = Sale.SaleName;
                                    ConInf.TeamCode = Sale.TeamName;
                                }
                                break;
                            case "SetupName":
                                {
                                    ConInf.SetupName = Sale.SaleName;
                                }
                                break;
                            case "CheckerName":
                                {
                                    ConInf.CheckerName = Sale.SaleName;
                                }
                                break;
                            case "ServiceName":
                                {
                                    ConInf.ServiceName = Sale.SaleName;
                                }
                                break;
                            case "CashName":
                                {
                                    ConInf.CashName = Sale.SaleName;
                                }
                                break;
                        }
                    }
                }
            }
            StateHasChanged();
        }

        async Task FindModel()
        {
            ExecResult Rs = new ExecResult();

            Rs = await dialogService.OpenAsync<ProModelDialog>($"Search Model",
                  new Dictionary<string, object>(),
                  new DialogOptions() { Width = "800px", Height = "600px" });
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Data != null)
                    {
                        var md = (BDProModel)Rs.Data;
                        ConInf.Model = md.MODEL;
                        ConInf.ModelDesc = md.Des;
                        ConInf.Credit = md.Sales;
                        ConInf.Cash = md.CASH;
                        ConInf.ContractPeriod = md.MODE;
                        ConInf.ContractFirstPeriodAmount = md.credit2;
                        ConInf.Sales = md.CASH;
                        ConInf.ContractPeriodAmount = md.CREDIT;
                        ConInf.ContractDiscount = 0;
                        ConInf.AfterDiscount = ConInf.Credit;
                        ConInf.ContractFirstPay = ConInf.ContractFirstPeriodAmount;
                    }
                }
            }
        }

        async Task OnDelete()
        {
            if (!CheckSave)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "ติ๊ก ยืนยันบันทึก ก่อน");
                return;
            }

            ExecResult Rs = new ExecResult();

            string TitleDialog = $"ลบ สัญญา {ConInf.ContractNo} อ้างอิง {ConInf.RefNo} ข้อมูลนี้ หรือไม่?";

            Rs = await dialogService.OpenAsync<EditorLogDialog>($"{TitleDialog}",
                new Dictionary<string, object>() { { "fromValue", ConInf.RefNo }
                    , { "toValue", "Delete" }
                    , { "editBy", userData.UserID }
                    , { "contractId", ConInf.ContractId }
                    , { "contNo", ConInf.ContractNo }
                    , { "refNo", ConInf.RefNo }
                }, new DialogOptions() { Width = "600px", Height = "260px" });
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    isLoading = true;
                    var postData = new Contract_Info { ContractId = ConInf.ContractId, RefNo = ConInf.RefNo, ContractNo = ConInf.ContractNo, CreatedBy = userData.UserID };
                    var response = await Http.PostAsJsonAsync("Contract/DeleteCont", postData);

                    Rs = new ExecResult();
                    Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                    isLoading = false;
                    if (Rs != null)
                    {
                        if (Rs.IsSuccess)
                        {
                            //NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                            await dialogService.Alert(Rs.Msg, "Success", new AlertOptions() { OkButtonText = "OK" });
                            ConInf = new Contract_Info_New();
                        }
                        else
                        {
                            await dialogService.Alert(Rs.Msg, "Error", new AlertOptions() { OkButtonText = "OK" });
                            //NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                        }
                    }
                }
            }

            /*var confirmationResult = await this.dialogService.Confirm($"ลบ สัญญา {ConInf.ContractNo} อ้างอิง {ConInf.RefNo} ข้อมูลนี้ หรือไม่?"
                , "Delete Confirm"
                , new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
            if (confirmationResult == true)
            {
                isLoading = true;
                var postData = new Contract_Info { ContractId = ConInf.ContractId, RefNo = ConInf.RefNo, ContractNo = ConInf.ContractNo, CreatedBy = userData.UserID };
                var response = await Http.PostAsJsonAsync("Contract/DeleteCont", postData);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                isLoading = false;
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                        ConInf = new Contract_Info_New();
                    }
                    else
                    {
                        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                    }
                }
            }*/
        }
    }
}
