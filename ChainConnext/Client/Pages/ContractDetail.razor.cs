using Blazored.LocalStorage;
using BlazorStrap.V5;
using ChainConnext.Client.Pages.Contracts;
using ChainConnext.Client.Pages.Products;
using ChainConnext.Client.Services;
using ChainConnext.Client.Shared;
using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Contracts;
using ChainConnext.Shared.Customers;
using ChainConnext.Shared.Payments;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System;
using System.Net.Http.Json;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChainConnext.Client.Pages
{
    public partial class ContractDetail
    {
        [Parameter]
        public string? pContractId { get; set; }
        [Parameter]
        public string? pRefNo { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        int selectedIndex = 0;
        int selectedIndexTop = 0;
        int selectedIndexChg = 0;
        int selectedIndexBill = 0;

        DateTime? value = DateTime.Now;
        List<FindMode> findModes = new List<FindMode>();
        int findSelected = 1;
        bool IsLoad = false;

        bool isloaddata = false;
        string UserID = "";

        bool EnableBt = false;

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        bool IsClone = false;
        bool IsChange = false;
        Authens userData = new Authens();

        Contract_Info ConInf = new Contract_Info();
        Contract_Info ConInfOrg = new Contract_Info();

        List<Contract_Status> contract_Statuses = new List<Contract_Status>();
        List<Customer_Title> customer_Titles = new List<Customer_Title>();

        int Height;
        int Width;

        protected override async Task OnInitializedAsync()
        {
            //await base.OnInitializedAsync();

            //Logger.LogInformation("OnInitializedAsync");

            isloaddata = true;

            //await CheckDefaultValues();

            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            await CheckPermission();

            await GetFindModeData();
            await ListContractStatus();
            await GetTitleData();

            isloaddata = false;

            if (pRefNo != null)
            {
                await ShowBusyDialogWithLink();

                //SearchValue = pRefNo;
                //findSelected = 1;
                //await FindData();
            }

            StateHasChanged();
        }

        async Task ShowBusyDialogWithLink()
        {
            InvokeAsync(async () =>
            {
                // Simulate background task

                await RunProgressWithLink();

                // Close the dialog
                dialogService.Close();

                StateHasChanged();
            });

            await BusyDialog("กำลังโหลดข้อมูล...");
        }

        async Task RunProgressWithLink()
        {
            isloaddata = true;

            if (pRefNo != null)
            {
                SearchValue = pRefNo;
                findSelected = 1;
                await FindData();
            }

            isloaddata = false;
        }

        async Task BusyDialog(string message)
        {
            await dialogService.OpenAsync("", ds =>
            {
                RenderFragment content = b =>
                {
                    b.OpenElement(0, "RadzenRow");

                    b.OpenElement(1, "RadzenColumn");
                    b.AddAttribute(2, "Size", "12");

                    b.AddContent(3, message);

                    b.CloseElement();
                    b.CloseElement();
                };
                return content;
            }, new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;width:auto", CloseDialogOnEsc = false });
        }
        /*
        async Task CheckDefaultValues()
        {
            string Key = ShareValues.GetTokenUrl() + "_DefaultValues";
            try
            {
                UserDefaultValues? userDefault;
                userDefault = await localStorage.GetItemAsync<UserDefaultValues>(Key);
                if (userDefault != null)
                {
                    findSelected = userDefault.ContFindMode;
                }
            }
            catch
            {
                await localStorage.RemoveItemAsync(Key);
            }
        }
        async Task SetDefaultValues()
        {
            string Key = ShareValues.GetTokenUrl() + "_DefaultValues";
            try
            {
                bool is_change = false;
                UserDefaultValues? userDefault;
                userDefault = await localStorage.GetItemAsync<UserDefaultValues>(Key);
                if (userDefault != null)
                {
                    if (userDefault.ContFindMode != findSelected)
                    {
                        userDefault.ContFindMode = findSelected;
                        is_change = true;
                    }
                }
                else
                {
                    userDefault = new UserDefaultValues();
                    userDefault.ContFindMode = findSelected;
                    is_change = true;
                }
                if (is_change)
                {
                    await localStorage.SetItemAsync(Key, userDefault);
                }
            }
            catch
            {
                await localStorage.RemoveItemAsync(Key);
            }
        }
        */
        bool IsSaveFn = false;
        bool IsEditRefNo = false;
        bool IsEditRefContNo = false;
        bool IsSaveMemo = false;
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
                var menu = userData.MenuList.Find(x => x.MenuGroup == "Contract" && x.MenuId == 17);
                if (menu != null)
                {
                    IsClone = menu.IsAccess;
                }
                var menu2 = userData.MenuList.Find(x => x.MenuGroup == "Contract" && x.MenuId == 18);
                if (menu2 != null)
                {
                    IsChange = menu2.IsAccess;
                }
                var menu3 = userData.MenuList.Find(x => x.MenuId == 76);
                if (menu3 != null)
                {
                    IsAccess = menu3.IsAccess;
                    IsSaveFn = menu3.IsSave;
                }
                else
                {
                    IsAccess = false;
                }
                var menu4 = userData.MenuList.Find(x => x.MenuId == 81);
                if (menu4 != null)
                {
                    IsEditRefNo = menu4.IsSave;
                }
                var menu5 = userData.MenuList.Find(x => x.MenuId == 82);
                if (menu5 != null)
                {
                    IsEditRefContNo = menu5.IsSave;
                }
                var menu6 = userData.MenuList.Find(x => x.MenuId == 97);
                if (menu6 != null)
                {
                    IsSaveMemo = menu6.IsSave;
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
                    if (customer_Titles != null)
                    {
                        if (customer_Titles.Count > 0)
                        {
                            ConInf.Title = customer_Titles[0].TitleID;
                        }
                    }
                }
            }
        }

        private async Task GetFindModeData()
        {
            var postBody = new FindMode();
            var response = await Http.PostAsJsonAsync("Find/ListFindMode", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    findModes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FindMode>>(Rs.Data.ToString());
                }
            }
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

        string SearchValue = "";
        async Task OnChange(string value, string name)
        {
            //Logger.LogInformation($"{name} value changed to {value}");
            switch (name)
            {
                case "Find":
                    {
                        if (!string.IsNullOrEmpty(value.Trim()))
                        {
                            int len = 8;
                            switch (findSelected)
                            {
                                case 1: len = 9; break;
                                case 2: len = 8; break;
                            }
                            SearchValue = BaseShared.FillRefNo(value.Trim(), len).ToUpper();
                            await FindData();
                        }
                    }
                    break;
                case "BranchCode":
                    {
                        if (ConInf.BranchCode != null)
                        {
                            if (!string.IsNullOrEmpty(ConInf.BranchCode.Trim()))
                            {
                                isloaddata = true;
                                ConInf.BranchName = "";
                                var postBody = new Branch { Code = ConInf.BranchCode.Trim() };
                                var response = await Http.PostAsJsonAsync("BD/ListBranch", postBody);

                                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                                if (Rs != null)
                                {
                                    if (Rs.Rows > 0)
                                    {
                                        var md = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Branch>>(Rs.Data.ToString()).FirstOrDefault();
                                        if (md != null)
                                        {
                                            ConInf.BranchName = md.Name;
                                        }
                                    }
                                }
                                isloaddata = false;
                            }
                        }
                    }
                    break;
                case "ChanelCode":
                    {
                        if (ConInf.ChanelCode != null)
                        {
                            if (!string.IsNullOrEmpty(ConInf.ChanelCode.Trim()))
                            {
                                isloaddata = true;
                                ConInf.ChanelName = "";
                                var postBody = new Chanel { code = ConInf.ChanelCode.Trim() };
                                var response = await Http.PostAsJsonAsync("BD/ListChanel", postBody);

                                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                                if (Rs != null)
                                {
                                    if (Rs.Rows > 0)
                                    {
                                        var md = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Chanel>>(Rs.Data.ToString()).FirstOrDefault();
                                        if (md != null)
                                        {
                                            ConInf.ChanelName = md.name;
                                        }
                                    }
                                }
                                isloaddata = false;
                            }
                        }
                    }
                    break;
            }
            //StateHasChanged();
        }
        async Task FindEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                if (string.IsNullOrEmpty(SearchValue.Trim()))
                {
                    return;
                }
                //SearchValue = FindSearchValue;
                await OnChange(SearchValue, "Find");
            }
        }
        string FindSearchValue = "";
        void OnFindChange(string value, string name)
        {
            FindSearchValue = value;
        }

        void OnFindInput(ChangeEventArgs e)
        {
            FindSearchValue = (string)e.Value;
        }

        async Task EnterFocus(KeyboardEventArgs e, string fousID)
        {
            if (e.Key == "Enter")// Covers: Enter + NumPad Enter + Mobile keyboard Go to(enter)
            {
                await jsRuntime.InvokeVoidAsync("focusInput", fousID);
                StateHasChanged();
            }
        }

        async Task OnChangeFocus(string value, string name)
        {
            //if (name == "ModelName")
            //{
            //    await GetProModelByID(value);
            //}
            //if(!string.IsNullOrEmpty(name.Trim()))
            //{
            //    await jsRuntime.InvokeVoidAsync("focusInput", name);
            //    StateHasChanged();
            //}
        }

        private async Task GetProModelByID(string model)
        {
            if (string.IsNullOrEmpty(model.Trim()))
            {
                return;
            }
            var postBody = new ProModel { MODEL = model };
            var response = await Http.PostAsJsonAsync("BD/GetProModelByID", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    var md = Newtonsoft.Json.JsonConvert.DeserializeObject<ProModel>(Rs.Data.ToString());
                    if (md != null)
                    {
                        ConInf.ModelDesc = md.ModelDesc2;
                    }
                }
            }
        }

        void ShowContextMenuWithItems(MouseEventArgs args)
        {
            ContextMenuService.Open(args,
                new List<ContextMenuItem> {
                new ContextMenuItem(){ Text = "Context menu item 1", Value = 1, Icon = "home" },
                new ContextMenuItem(){ Text = "Context menu item 2", Value = 2, Icon = "search" },
                new ContextMenuItem(){ Text = "Context menu item 3", Value = 3, Icon = "info" },
             }, OnMenuItemClick);
        }

        void OnMenuItemClick(MenuItemEventArgs args)
        {
            Logger.LogInformation($"Menu item with Value={args.Value} clicked");
            if (!args.Value.Equals(3) && !args.Value.Equals(4))
            {
                ContextMenuService.Close();
            }
        }

        async Task OpenEdit(Contract_Info? daTa, string key)
        {
            if (daTa == null)
            {
                daTa = new Contract_Info();
            }
            ExecResult Rs = new ExecResult();
            daTa.ContractId = Guid.NewGuid().ToString();
            Rs = await dialogService.OpenAsync<ContractNew>($"{key} : Contract",
                  new Dictionary<string, object>() { { "ContractId", daTa.ContractId }, { "Key", key } },
                  new DialogOptions() { Width = "1300px", Height = "1000px" });
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    
                }
            }
        }

        async Task OpenClone(Contract_Info? daTa, string key)
        {
            if (ConInf.RefNo == null)
            {
                return;
            }
            if (daTa == null)
            {
                daTa = ConInf;
            }

            ExecResult RsCheck = new ExecResult();
            var postBody = new Contract_Info { ContractNo = daTa.ContractNo, RefNo = daTa.RefNo, UserData = userData };
            var response = await Http.PostAsJsonAsync("Contract/CheckCloneData", postBody);
            RsCheck = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (RsCheck != null)
            {
                if (!RsCheck.IsSuccess)
                {
                    await dialogService.Alert(RsCheck.Msg, "Clone ไม่ได้", new AlertOptions() { OkButtonText = "OK" });
                    return;
                }
            }

            string Data = Newtonsoft.Json.JsonConvert.SerializeObject(daTa);
            ExecResult Rs = new ExecResult();
            daTa.ContractId = Guid.NewGuid().ToString();
            Rs = await dialogService.OpenAsync<ContractClone>($"{key} : {daTa.RefNo} - {daTa.ContractNo} - {daTa.CustomerName}",
                  new Dictionary<string, object>() { { "ContractId", daTa.ContractId }, { "Key", key }, { "ConInfData", Data } },
                  new DialogOptions() { Width = "1024px", Height = "768px" });
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Data != null)
                    {
                        NotificationService.Notify(NotificationSeverity.Success, "Success", "โคลนสัญญาเสร็จแล้ว");

                        Contract_Info_Clone RsData = (Contract_Info_Clone)Rs.Data;
                        if (RsData != null)
                        {
                            if (RsData.ConInfo != null)
                            {
                                if (RsData.ConInfo.RefNo != null)
                                {
                                    findSelected = 1;
                                    SearchValue = RsData.ConInfo.RefNo;

                                    await FindData();
                                }
                            }
                        }
                    }
                }
            }
        }
        async Task OpenReNew(Contract_Info? daTa, string key)
        {
            if (ConInf.RefNo == null)
            {
                return;
            }
            if (daTa == null)
            {
                daTa = ConInf;
            }
            string Data = Newtonsoft.Json.JsonConvert.SerializeObject(daTa);
            ExecResult Rs = new ExecResult();
            daTa.ContractId = Guid.NewGuid().ToString();
            Rs = await dialogService.OpenAsync<ContractReNew>($"{key} : {daTa.RefNo} - {daTa.ContractNo} - {daTa.CustomerName}",
                  new Dictionary<string, object>() { { "ContractId", daTa.ContractId }, { "Key", key }, { "ConInfData", Data } },
                  new DialogOptions() { Width = "1024px", Height = "768px" });
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    
                }
            }
        }

        //async Task Find(Contract_Info? daTa, string key)
        //{
        //    await Task.Run(FindData);
        //}

        async Task FindData()
        {
            IsLoad = true;

            ConInf = new Contract_Info();
            payment_Infos = new List<Payment_Info>();
            paytrans_Infos = new List<Payment_Transaction>();
            ConInfEmp = new Contract_Info();
            bD_ChgConts = new List<BD_ChgCont>();
            bD_ChgModels = new List<BD_ChgModel>();
            ConInfOrg = new Contract_Info();

            EnableBt = false;
            Authens userData = new Authens();
            //var authState = await authenticationState;
            //var user = authState.User;
            //if (user.Identity.IsAuthenticated)
            //{
            //    userData = Newtonsoft.Json.JsonConvert.DeserializeObject<Authens>(user.Identity.GetData());
            //    userData.AppUrl = Navigation.Uri;
            //    if (userData != null)
            //    {
            //        UserID = userData.UserID;
            //    }
            //}
            bool is_found = false;
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new FindMode { FindID = findSelected, FindValue = SearchValue, FindBy = userData.UserID, UserData = userData };
            var response = await Http.PostAsJsonAsync("Contract/FindData", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    ConInf = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Info>>(Rs.Data.ToString()).FirstOrDefault();
                    ConInfOrg = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Info>>(Rs.Data.ToString()).FirstOrDefault();


                    if (ConInf != null)
                    {
                        if (ConInf.ContractId != null)
                        {
                            if (!string.IsNullOrEmpty(ConInf.ContractId.Trim()))
                            {
                                if (ConInf.RmPay == 0)
                                {
                                    ConInf.RmPay = ConInf.AfterDiscount - ConInf.SmPay;
                                }
                                //Console.WriteLine(ConInf.MissData);
                                EnableBt = true;
                                is_found = true;
                            }
                        }
                    }
                    
                }
            }
            IsLoad = false;

            if (!is_found)
            {
                //Logger.LogInformation("ไม่พบข้อมูล");
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "ไม่พบข้อมูล");
            }
            else
            {
                selectedIndex = 0;
                StateHasChanged();
                await LoadTab(selectedIndex);
            }

            //await SetDefaultValues();

            //IsLoad = false;
        }
        bool IsLoadSaveFnDate = false;
        async Task SaveFnDate()
        {
            if (ConInf.RefNo == null)
            {
                return;
            }
            IsLoadSaveFnDate = true;

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            ConInf.UserData = userData;
            ConInf.UpdatedBy = userData.UserID;

            var response = await Http.PostAsJsonAsync("Contract/WebSave", ConInf);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    //await FindData();
                    int tmpselectedIndex = selectedIndex;

                    selectedIndex = 0;
                    StateHasChanged();
                    selectedIndex = tmpselectedIndex;
                    StateHasChanged();
                    NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }

            IsLoadSaveFnDate = false;
        }

        async Task FindModel()
        {
            if (ConInf.RefNo == null)
            {
                return;
            }
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
                        ConInf.AfterDiscount = ConInf.Credit - ConInf.ContractDiscount;
                        ConInf.ContractFirstPay = ConInf.ContractFirstPeriodAmount - ConInf.ContractDiscount;
                    }
                }
            }
        }

        async Task FindModelNonDlg()
        {
            if (ConInf.RefNo == null)
            {
                return;
            }
            IsLoad = true;
            var postBody = new BDProModel { MODEL = ConInf.Model };
            var response = await Http.PostAsJsonAsync("BD/GetProModelFindModel", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        var md = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BDProModel>>(Rs.Data.ToString()).FirstOrDefault();
                        if (md != null)
                        {
                            ConInf.ModelDesc = md.Des;
                            ConInf.Credit = md.Sales;
                            ConInf.Cash = md.CASH;
                            ConInf.ContractPeriod = md.MODE;
                            ConInf.ContractFirstPeriodAmount = md.credit2;
                            ConInf.Sales = md.CASH;
                            ConInf.ContractPeriodAmount = md.CREDIT;
                            ConInf.AfterDiscount = ConInf.Credit - ConInf.ContractDiscount;
                            ConInf.ContractFirstPay = ConInf.ContractFirstPeriodAmount - ConInf.ContractDiscount;
                        }
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
            IsLoad = false;
        }

        async Task OnChangeContractDiscount(decimal value, string name)
        {
            await Task.Run(() =>
            {

                ConInf.AfterDiscount = ConInf.Credit - value;
                ConInf.ContractFirstPay = ConInf.ContractFirstPeriodAmount - value;
            });
        }

        async Task ShowStatusLog()
        {
            if (ConInf.RefNo == null)
            {
                return;
            }
            ExecResult Rs = new ExecResult();

            Rs = await dialogService.OpenAsync<TranStDialog>($"{ConInf.RefNo} | {ConInf.ContractNo} | {ConInf.CustomerName}",
            new Dictionary<string, object>() { { "pContractId", ConInf.ContractId }, { "Key", "ShowStatusLog" }, { "pRefNo", ConInf.RefNo } },
                  new DialogOptions() { Width = "550px", Height = "600px" });
        }

        async Task ShowSerialNo()
        {
            if (ConInf.RefNo == null)
            {
                return;
            }
            ExecResult Rs = new ExecResult();

            Rs = await dialogService.OpenAsync<ContractSerialNo>($"{ConInf.RefNo} | {ConInf.ContractNo} | {ConInf.CustomerName}",
            new Dictionary<string, object>() { { "pContractId", ConInf.ContractId } },
                  new DialogOptions() { Width = "600px", Height = "500px" });
        }

        async Task LoadTab(int index)
        {
            Logger.LogInformation("LoadTab");
            if (!EnableBt)
            {
                return;
            }
            Logger.LogInformation("LoadTab EnableBt = true");
            IsLoad = true;
            switch (index)
            {
                case 0:
                    {
                        await LoadTabBill(selectedIndexBill);
                    }
                    break;
                case 1:
                    {
                        await GetContractEmpData();
                    }
                    break;
                case 6:
                    {
                        await LoadTabChg(selectedIndexChg);
                    }
                    break;
            }
            IsLoad = false;
        }

        async Task LoadTabBill(int index)
        {
            if (!EnableBt)
            {
                return;
            }
            switch (index)
            {
                case 0: await PaymentListData(); break;
                case 1: await PayTransListData(); break;
            }
        }
        async Task LoadTabChg(int index)
        {
            switch (index)
            {
                case 0: await ListCloneLogData(); break;
                case 1: await ListChgModelLogData(); break;
            }
        }

        List<Payment_Info> payment_Infos = new List<Payment_Info>();
        List<Payment_Transaction> paytrans_Infos = new List<Payment_Transaction>();
        Contract_Info ConInfEmp = new Contract_Info();
        List<BD_ChgCont> bD_ChgConts = new List<BD_ChgCont>();
        List<BD_ChgModel> bD_ChgModels = new List<BD_ChgModel>();
        private async Task PaymentListData()
        {
            if (ConInf == null)
            {
                return;
            }
            if (ConInf.ContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(ConInf.ContractId.Trim()))
            {
                return;
            }
            payment_Infos = new List<Payment_Info>();

            if (!EnableBt)
            {
                return;
            }

            var postBody = new Payment_Info { ContractId = ConInf.ContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Payment/PaymentList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    payment_Infos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment_Info>>(Rs.Data.ToString());
                }
            }
        }
        private async Task PayTransListData()
        {
            if (ConInf == null)
            {
                return;
            }
            if (ConInf.ContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(ConInf.ContractId.Trim()))
            {
                return;
            }

            paytrans_Infos = new List<Payment_Transaction>();

            if (!EnableBt)
            {
                return;
            }

            var postBody = new Payment_Transaction { ContractId = ConInf.ContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Payment/PaymentTransactionList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    paytrans_Infos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment_Transaction>>(Rs.Data.ToString());
                }
            }
        }

        private async Task GetContractEmpData()
        {
            if (ConInf == null)
            {
                return;
            }
            if (ConInf.ContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(ConInf.ContractId.Trim()))
            {
                return;
            }

            ConInfEmp = new Contract_Info();

            if (!EnableBt)
            {
                return;
            }

            var postBody = new Contract_Info { ContractId = ConInf.ContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Contract/GetContractEmp", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    ConInfEmp = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Info>>(Rs.Data.ToString()).FirstOrDefault();
                    if (ConInfEmp != null)
                    {
                        if (ConInfEmp.CreditFnNo == 0)
                        {
                            if (ConInfEmp.DueDate != null)
                            {
                                var postBodyD = new BD_CreditTip { DueDate = ConInfEmp.DueDate };
                                var responseD = await Http.PostAsJsonAsync("BD/CreditTripGetFn", postBodyD);

                                ExecResult? RsD = await responseD.Content.ReadFromJsonAsync<ExecResult>();
                                if (RsD != null)
                                {
                                    //Logger.LogInformation(Rs.Msg);
                                    if (RsD.Rows > 0)
                                    {
                                        var creditTip = Newtonsoft.Json.JsonConvert.DeserializeObject<BD_CreditTip>(RsD.Data.ToString());
                                        if (creditTip != null)
                                        {
                                            ConInfEmp.CreditFnNo = creditTip.periodno;
                                            ConInfEmp.CreditFnYear = creditTip.ayear;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        async Task OnContSaveEmp(Contract_Info conInfEmp)
        {
            IsLoad = true;

            var response = await Http.PostAsJsonAsync("Contract/SaveEmp", conInfEmp);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    await GetContractEmpData();
                    NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }

            IsLoad = false;
        }
        async Task OnContSaveDue(Contract_Info pConInf)
        {
            IsLoad = true;

            var response = await Http.PostAsJsonAsync("Contract/SaveDue", pConInf);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    await GetContractEmpData();
                    NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }

            IsLoad = false;
        }

        private async Task ListCloneLogData()
        {
            if (ConInf == null)
            {
                return;
            }
            if (ConInf.ContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(ConInf.ContractId.Trim()))
            {
                return;
            }

            bD_ChgConts = new List<BD_ChgCont>();

            if (!EnableBt)
            {
                return;
            }

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new BD_ChgCont { RefNo = ConInf.RefNo, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/ListChgCont", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    bD_ChgConts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_ChgCont>>(Rs.Data.ToString());
                }
            }
        }
        private async Task ListChgModelLogData()
        {
            if (ConInf == null)
            {
                return;
            }
            if (ConInf.ContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(ConInf.ContractId.Trim()))
            {
                return;
            }

            bD_ChgModels = new List<BD_ChgModel>();

            if (!EnableBt)
            {
                return;
            }

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new BD_ChgModel { RefNo = ConInf.RefNo, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/BDListChangeModel", postBody);
            //var response = await Http.PostAsJsonAsync("BD/ListChgCont", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    bD_ChgModels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_ChgModel>>(Rs.Data.ToString());
                }
            }
        }

        async Task SaveMemo()
        {
            IsLoad = true;
            IsLoadSaveFnDate = true;

            var postBody = new Contract_Info_Memo { ContractId = ConInf.ContractId, Memo = ConInf.Memo, CreatedBy = userData.UserID };

            var response = await Http.PostAsJsonAsync("Contract/SaveMemo", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }

            IsLoad = false;
            IsLoadSaveFnDate = false;
        }

        async Task OnEditRefNoContNo(string Cmd)
        {
            string TitleDialog = "";
            if (Cmd == "RefNo")
            {
                TitleDialog = $"เปลี่ยนอ้างอิงจาก {ConInfOrg.RefNo} ไป {ConInf.RefNo}";
            }
            else if (Cmd == "ContNo")
            {
                TitleDialog = $"เปลี่ยนเลขสัญญาจาก {ConInfOrg.ContractNo} ไป {ConInf.ContractNo}";
            }
            if (!string.IsNullOrEmpty(TitleDialog.Trim()))
            {
                ExecResult Rs = new ExecResult();

                if (Cmd == "RefNo")
                {
                    Rs = await dialogService.OpenAsync<EditorLogDialog>($"{TitleDialog}",
                new Dictionary<string, object>() { { "fromValue", ConInfOrg.RefNo }
                    , { "toValue", ConInf.RefNo }
                    , { "editBy", userData.UserID }
                    , { "contractId", ConInf.ContractId }
                    , { "contNo", ConInfOrg.ContractNo }
                    , { "refNo", ConInfOrg.RefNo }
                }, new DialogOptions() { Width = "550px", Height = "260px" });
                }
                else if (Cmd == "ContNo")
                {
                    Rs = await dialogService.OpenAsync<EditorLogDialog>($"{TitleDialog}",
                new Dictionary<string, object>() { { "fromValue", ConInfOrg.ContractNo }
                    , { "toValue", ConInf.ContractNo }
                    , { "editBy", userData.UserID }
                    , { "contractId", ConInf.ContractId }
                    , { "contNo", ConInfOrg.ContractNo }
                    , { "refNo", ConInfOrg.RefNo }
                }, new DialogOptions() { Width = "550px", Height = "260px" });
                }
                
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        //660085047
                        var postBody = new Contract_Info{ ContractId = ConInf.ContractId,
                            EditContNoRefNoType = Cmd,
                            ContractNo = ConInf.ContractNo,
                            RefNo = ConInf.RefNo,
                            CreatedBy = userData.UserID, 
                            UserData = userData };

                        var response = await Http.PostAsJsonAsync("Contract/EditRefNoContNo", postBody);

                        ExecResult? RsE = await response.Content.ReadFromJsonAsync<ExecResult>();
                        if (RsE != null)
                        {
                            if (RsE.IsSuccess)
                            {
                                NotificationService.Notify(NotificationSeverity.Success, "Success", RsE.Msg);
                            }
                            else
                            {
                                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = RsE.Msg, Duration = 5000 });
                            }
                        }
                    }
                }
            }
        }

        //string message;
        //void OnInputn(ChangeEventArgs e)
        //{
        //    message = (string)e.Value;
        //}

        //public void Enter(KeyboardEventArgs e)
        //{
        //    if (e.Code == "Enter" || e.Code == "NumpadEnter")
        //    {
        //        NotificationService.Notify(new NotificationMessage
        //        {
        //            Severity = NotificationSeverity.Info,
        //            Summary = "Enter Notify",
        //            Detail = $"{message}",
        //            Duration = 5000
        //        });
        //    }
        //}

        async Task OnFindData(ExecResult Rs)
        {
            //NotificationService.Notify(new NotificationMessage
            //{
            //    Severity = NotificationSeverity.Info,
            //    Summary = "Enter Notify",
            //    Detail = $"{value}",
            //    Duration = 5000
            //});
            if (Rs == null)
            {
                return;
            }
            findSelected = Convert.ToInt32(Rs.ID);
            SearchValue = Rs.Data.ToString().Trim();
            await OnChange(SearchValue, "Find");
        }
        async Task OnFindEnter(ExecResult Rs)
        {
            //NotificationService.Notify(new NotificationMessage
            //{
            //    Severity = NotificationSeverity.Info,
            //    Summary = "Enter Notify",
            //    Detail = $"{value}",
            //    Duration = 5000
            //});
            if (Rs == null)
            {
                return;
            }
            findSelected = Convert.ToInt32(Rs.ID);
            SearchValue = Rs.Data.ToString().Trim();
            await OnChange(SearchValue, "Find");
        }
        bool IsLoading = false;
        async Task CheckPermMenu()
        {
            try
            {
                IsLoading = true;
                await _accountService.UpdatePermsAsync();

                //await CheckPermission();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        public async Task Logout()
        {
            //await GetGeolocation();
            string UserID = "";
            var authState = await authenticationState;
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                var UserData = Newtonsoft.Json.JsonConvert.DeserializeObject<Authens>(user.Identity.GetData());
                if (UserData != null)
                {
                    UserID = UserData.UserID;
                }
            }

            var postBody = new LogoutRequest { UserID = UserID };

            //var response = await Http.PostAsJsonAsync("Authen/Logout", postBody);
            //var UserMainData = await response.Content.ReadFromJsonAsync<Authens>();
            //if (UserMainData != null)
            //{
            //    //sidebar1Expanded = false;
            await _accountService.LogoutAsync(postBody);
            Navigation.NavigateTo("login");
            //}
            //Navigation.NavigateTo("logout");
        }
    }
}
