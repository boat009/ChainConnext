using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using ChainConnext.Shared.Payments;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using ChainConnext.Shared.Supports;
using ChainConnext.Shared.Contracts;
using Radzen;
using System;
using ChainConnext.Client.Services;
using Microsoft.JSInterop;

namespace ChainConnext.Client.Pages.Payments
{
    public partial class PaymentEditor
    {
        [Parameter]
        public string? pContractId { get; set; }
        [Parameter]
        public string? pRefNo { get; set; }
        [Parameter]
        public string? Key { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        Contract_Info ConInf = new Contract_Info();
        Payment_Info PayInfo = new Payment_Info();
        List<Payment_PayBy> payment_PayBies = new List<Payment_PayBy>();
        List<Payment_Place> payment_Places = new List<Payment_Place>();
        List<NPT_Depart> nPT_Departs = new List<NPT_Depart>();
        List<Fortnight_Info> fortnight_Infos_FnNo = new List<Fortnight_Info>();
        List<Fortnight_Info> fortnight_Infos_FnYear = new List<Fortnight_Info>();
        List<Payment_Info> payment_Infos = new List<Payment_Info>();
        List<Payment_Transaction> payment_Transactions = new List<Payment_Transaction>();
        IList<Payment_Info> selectedPayment;
        int paybyId = 0;

        bool IsLoad = false;
        bool IsLoading = false;
        bool FindParametor = true;

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        Authens userData = new Authens();

        decimal SumPay = 0;
        int CountPay = 0;
        string SumPayColor = "black";

        int selectedIndex = 0;

        int Height;
        int Width;

        protected override async Task OnInitializedAsync()
        {
            await GetDimension();

            await ShowBusyDialogWithLink();
        }

        //protected override async Task OnParametersSetAsync()
        //{
        //    //await GetDimension();

        //    //await ShowBusyDialogLoadWithDialog();
        //}

        async Task GetDimension()
        {
            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;
        }

        async Task RunProgress()
        {
            IsLoading = true;

            //await NPTDepartData();
            //await FnNoData();
            //await FnYearData();
            //await PayByListData();
            //await PayPlaceListData();

            //await CheckPermission();

            if (pRefNo != null)
            {
                SearchValue = pRefNo;
                findSelected = 1;
                await Find();
            }

            IsLoading = false;
        }

        async Task RunProgressWithLink()
        {
            IsLoading = true;

            await CheckPermission();

            await NPTDepartData();
            await FnNoData();
            await FnYearData();
            await PayByListData();
            await PayPlaceListData();

            if (pRefNo != null)
            {
                SearchValue = pRefNo;
                findSelected = 1;
                await Find();
            }
            else
            {
                await ContDetail();
            }

            IsLoading = false;
        }

        async Task ShowBusyDialogLoadWithDialog()
        {
            InvokeAsync(async () =>
            {
                // Simulate background task

                await RunProgress();

                // Close the dialog
                dialogService.Close();

                StateHasChanged();
            });

            await BusyDialog("กำลังโหลดข้อมูล...");
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
                var menu = userData.MenuList.Find(x => x.MenuId == 19);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        private async Task PaymentListData()
        {
            if (FindParametor)
            {
                if (pContractId == null)
                {
                    return;
                }
                if (string.IsNullOrEmpty(pContractId.Trim()))
                {
                    return;
                }
            }
            else
            {
                if (ConInf.ContractId == null)
                {
                    return;
                }
                if (string.IsNullOrEmpty(ConInf.ContractId.Trim()))
                {
                    return;
                }
            }

            payment_Infos = new List<Payment_Info>();
            SumPay = 0;
            CountPay = 0;
            SumPayColor = "black";

            pContractId = ConInf.ContractId.Trim();

            IsLoading = true;
            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new Payment_Info { ContractId = pContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Payment/PaymentList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    payment_Infos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment_Info>>(Rs.Data.ToString());
                    SumPay = payment_Infos.Sum(x => x.PayAmt);
                    CountPay = payment_Infos.Count;
                    var p_sum = (SumPay / ConInf.AfterDiscount) * 100;
                    if (p_sum < 50)
                    {
                        SumPayColor = "red";
                    }
                    else if (p_sum >= 50 && p_sum < 90)
                    {
                        SumPayColor = "Orange";
                    }
                    else
                    {
                        SumPayColor = "green";
                    }
                }
            }
            IsLoading = false;
        }

        private async Task BHPaymentListData()
        {
            if (FindParametor)
            {
                if (pContractId == null)
                {
                    return;
                }
                if (string.IsNullOrEmpty(pContractId.Trim()))
                {
                    return;
                }
            }
            else
            {
                if (ConInf.ContractId == null)
                {
                    return;
                }
                if (string.IsNullOrEmpty(ConInf.ContractId.Trim()))
                {
                    return;
                }
            }

            payment_Transactions = new List<Payment_Transaction>();

            pContractId = ConInf.ContractId.Trim();

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new Payment_Transaction { ContractId = pContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Payment/PaymentTransactionList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    payment_Transactions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment_Transaction>>(Rs.Data.ToString());
                }
            }
        }

        private async Task PayByListData()
        {
            var postBody = new Payment_PayBy();
            var response = await Http.PostAsJsonAsync("Payment/PayByList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    payment_PayBies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment_PayBy>>(Rs.Data.ToString());
                }
            }
        }
        private async Task PayPlaceListData()
        {
            IsLoad = true;

            var postBody = new Payment_Place { PayById = paybyId };
            var response = await Http.PostAsJsonAsync("Payment/PayPlaceList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    payment_Places = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment_Place>>(Rs.Data.ToString());
                }
            }
            IsLoad = false;
        }

        private async Task NPTDepartData()
        {
            var postBody = new NPT_Depart();
            var response = await Http.PostAsJsonAsync("Support/GetNPTDetpart", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    nPT_Departs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NPT_Depart>>(Rs.Data.ToString());
                }
            }
        }
        private async Task FnNoData()
        {
            var postBody = new NPT_Depart();
            var response = await Http.PostAsJsonAsync("Support/GetFortnightNoWeb", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    fortnight_Infos_FnNo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Fortnight_Info>>(Rs.Data.ToString());
                }
            }
        }
        private async Task FnYearData()
        {
            var postBody = new NPT_Depart();
            var response = await Http.PostAsJsonAsync("Support/GetFortnightYearWebPay", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    fortnight_Infos_FnYear = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Fortnight_Info>>(Rs.Data.ToString());
                }
            }
        }

        async Task Find(Contract_Info? daTa, int findSelected)
        {
            await Task.Run(() => FindData(findSelected));
        }

        async Task FindData(int findSelected)
        {
            IsLoad = true;

            ConInf = new Contract_Info();
            PayInfo = new Payment_Info();
            payment_Infos = new List<Payment_Info>();

            if (!string.IsNullOrEmpty(SearchValue.Trim()))
            {
                Authens userData = new Authens();
                userData = await _accountService.GetAuthensAsync(Navigation.Uri);

                var postBody = new Contract_Info();
                switch (findSelected)
                {
                    case 1:
                        {
                            postBody.RefNo = SearchValue;
                        }
                        break;
                    case 2:
                        {
                            postBody.ContractNo = SearchValue;
                        }
                        break;
                }
                postBody.UserData = userData;
                postBody.CreatedBy = userData.UserID;
                var response = await Http.PostAsJsonAsync("Contract/FindRefNoContNoData", postBody);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    //Logger.LogInformation(Rs.Msg);
                    if (Rs.Rows > 0)
                    {
                        ConInf = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Info>>(Rs.Data.ToString()).FirstOrDefault();
                        await PaymentListData();
                        await BHPaymentListData();
                    }
                }
            }

            IsLoad = false;
        }
        async Task ContDetail()
        {
            if (pContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pContractId))
            {
                return;
            }

            IsLoad = true;
            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new Contract_Info { ContractId = pContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Contract/GetContractDetailData", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    ConInf = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Info>>(Rs.Data.ToString()).FirstOrDefault();
                }
            }

            IsLoad = false;
        }
        int findSelected = 0;
        string SearchValue = "";
        async Task OnChange(string value, string name)
        {
            //Logger.LogInformation($"{name} value changed to {value}");
            if (!string.IsNullOrEmpty(value.Trim()))
            {
                
                switch (name)
                {
                    case "RefNo":
                        {
                            findSelected = 1;
                            SearchValue = BaseShared.FillRefNo(value.Trim(), 9);
                        }
                        break;
                    case "ContNo":
                        {
                            findSelected = 2;
                            SearchValue = BaseShared.FillRefNo(value.Trim(), 8).ToUpper();
                        }
                        break;
                    case "CustName":
                        {
                            findSelected = 4;
                            SearchValue = value.Trim();
                        }
                        break;
                }
                await Find();
            }
        }

        async Task Find()
        {
            if (findSelected > 0)
            {
                FindParametor = false;
                await FindData(findSelected);
            }

            StateHasChanged();
        }

        async Task Save()
        {
            if (ConInf.ContractId == null)
            {
                return;
            }
            IsLoad = true;

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            PayInfo.CreatedBy = userData.UserID;
            PayInfo.UserData = userData;

            var Mresponse = await Http.PostAsJsonAsync("Payment/SaveMiniData", PayInfo);
            ExecResult? MRs = await Mresponse.Content.ReadFromJsonAsync<ExecResult>();
            if (MRs != null)
            {
                if (MRs.IsSuccess)
                {
                    await PaymentListData();
                    //dialogService.Close(MRs);
                }
                else
                {
                    Logger.LogError(MRs.Msg);
                }
            }
            IsLoad = false;
        }
        async Task Clear()
        {
            IsLoad = true;
            await Task.Run(() => {
                ConInf = new Contract_Info();
                payment_Infos = new List<Payment_Info>();
                PayInfo = new Payment_Info();
            });
            IsLoad = false;
        }
        bool visible_del_btn = false;
        async Task OnCellClick(DataGridCellMouseEventArgs<Payment_Info> args)
        {
            IsLoading = true;
            PayInfo = args.Data;

            if (true)
            {
                visible_del_btn = (await _accountService.GetAuthensAsync(Navigation.Uri)).Perms.IsDelete;
            }
            else
            {
                visible_del_btn = false;
            }
            IsLoading = false;
        }

        async Task ConfirmDelete(Payment_Info daTa)
        {
            var confirmationResult = await this.dialogService.Confirm($"ลบ เลขที่ใบเสร็จ {daTa.InvNo} ออกจาก {ConInf.ContractNo} หรือไม่?"
                , "Delete Confirm"
                , new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
            if (confirmationResult == true)
            {
                IsLoading = true;

                daTa.CreatedBy = userData.UserID;
                daTa.RefNo = ConInf.RefNo;
                daTa.ContractNo = ConInf.ContractNo;
                daTa.UserData = userData;

                var response = await Http.PostAsJsonAsync("Payment/DeletePayment", daTa);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                        await Find();
                        await PaymentListData();
                    }
                    else
                    {
                        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                        Logger.LogError(Rs.Msg);
                    }
                }
                IsLoading = false;
            }
        }

        async Task ConfirmDeleteInvoinceABH(Payment_Transaction daTa)
        {
            var confirmationResult = await this.dialogService.Confirm($"ลบ เลขที่ใบเสร็จ {daTa.InvNo} ออกจาก {ConInf.ContractNo} หรือไม่?"
                , "Delete Confirm"
                , new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
            if (confirmationResult == true)
            {
                IsLoading = true;

                daTa.CreatedBy = userData.UserID;
                daTa.RefNo = ConInf.RefNo;
                daTa.ContractNo = ConInf.ContractNo;
                daTa.UserData = userData;

                var response = await Http.PostAsJsonAsync("Payment/DeletePayment", daTa);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                        await Find();
                        await BHPaymentListData();
                    }
                    else
                    {
                        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                        Logger.LogError(Rs.Msg);
                    }
                }
                IsLoading = false;
            }
        }

        void ShowTooltip(ElementReference elementReference, TooltipOptions options = null,string tooltiptext = "") => tooltipService.Open(elementReference, tooltiptext, options);
    }
}
