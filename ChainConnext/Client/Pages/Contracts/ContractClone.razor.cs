using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using ChainConnext.Shared.Payments;
using Microsoft.AspNetCore.Components.Authorization;
using ChainConnext.Shared.Authen;
using Radzen;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.RegularExpressions;
using ChainConnext.Client.Pages.Products;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChainConnext.Client.Pages.Contracts
{
    public partial class ContractClone
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        [Parameter]
        public string? ContractId { get; set; }
        [Parameter]
        public string? Key { get; set; }
        [Parameter]
        public string? ConInfData { get; set; }

        Contract_Info ConInf = new Contract_Info();
        Contract_Info ConInfOrg = new Contract_Info();
        Contract_Info ConInfNew = new Contract_Info();

        List<Payment_Info> payment_Infos = new List<Payment_Info>();
        List<Payment_Info> payment_Infos_Delete = new List<Payment_Info>();

        List<Payment_Info> payment_InfosNew = new List<Payment_Info>();
        List<Payment_Info> payment_Infos_DeleteNew = new List<Payment_Info>();

        IList<Payment_Info>? selectedPayment;
        IList<Payment_Info>? selectedPaymentNew;

        List<Contract_Status> contract_Statuses = new List<Contract_Status>();

        List<Contract_Info> AccStatusData = new List<Contract_Info>();

        bool IsLoading = false;

        string NewCloneRefNo = "";

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        protected override async Task OnParametersSetAsync()
        {
            //await Task.Delay(500);
            IsLoading = true;

            await CheckPermission();

            if (ConInfData == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(ConInfData.Trim()))
            {
                return;
            }

            await ListContractStatus();
            await ListAccStatus();
            //Logger.LogInformation(ConInfData);
            await Task.Run(async () =>
            {
                ConInfOrg.ContractIdLink = Guid.NewGuid().ToString();
                ConInfOrg = Newtonsoft.Json.JsonConvert.DeserializeObject<Contract_Info>(ConInfData);
                ConInf = Newtonsoft.Json.JsonConvert.DeserializeObject<Contract_Info>(ConInfData);
                ConInf.ContractNo = "?" + ConInf.ContractNo.Substring(1, ConInf.ContractNo.Length - 1);
                ConInf.ContractStatus = "?";
                ConInf.CashCode = "99993";
                ConInf.CashName = "สัญญาสวม";
                ConInf.ContractType = "?";
                ConInf.SerialNo = ReplaceNewSerialNo(ConInfOrg.SerialNo);
                ConInf.CashCodeOld = ConInfOrg.CashCode;
                ConInf.CashNameOld = ConInfOrg.CashName;
                ConInf.ContractNoOld = ConInfOrg.ContractNo;
                ConInf.RefNoOld = ConInfOrg.RefNo;
                ConInf.ContractIdLink = ConInfOrg.ContractIdLink;

                ConInfNew = ConInfOrg;
                ConInfNew.ContractId = Guid.NewGuid().ToString();
                ConInfNew.ContractNo = ConInfOrg.ContractNo;//Guid.NewGuid().ToString();

                await NewRefNoClone();
                ConInfNew.TransferDate = DateTime.Now;
                ConInfNew.RefNo = NewCloneRefNo;
                ConInfNew.OpenDate = ConInfOrg.OpenDate;
                ConInfNew.CloseDate = ConInfOrg.CloseDate;
                ConInfNew.EffDate = ConInfOrg.EffDate;
                ConInfNew.ContractStatus = ConInfOrg.ContractStatus;
                ConInfNew.ContractStatusDate = ConInfOrg.ContractStatusDate;
                ConInfNew.ContractNoFrom = ConInf.ContractNo;
                ConInfNew.RefNoFrom = ConInf.RefNo;
                ConInfNew.SerialNo = ConInfOrg.SerialNo;
                ConInfNew.ContractIdLink = ConInfOrg.ContractIdLink;
                ConInfNew.CashCode = ConInfOrg.CashCode;
                ConInfNew.CashName = ConInfOrg.CashName;
                ConInfNew.AccStatusDate = ConInfOrg.LastPayDate;
            });

            await PaymentListData();

            IsLoading = false;
        }

        private string ReplaceNewSerialNo(string value)
        {
            value = value.ToUpper();
            string NewSn = "";
            NewSn = Regex.Replace(value, @"[A-Z]", "?");
            return NewSn;
        }

        private async Task NewRefNoClone()
        {
            var postBody = new Contract_Status();
            var response = await Http.PostAsJsonAsync("BD/NewRefNoClone", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Data != null)
                {
                    NewCloneRefNo = Rs.Data.ToString().Trim();
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
        private async Task ListAccStatus()
        {
            var postBody = new Contract_Info();
            var response = await Http.PostAsJsonAsync("Contract/ListAccStatus", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    AccStatusData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Info>>(Rs.Data.ToString());
                }
            }
        }

        private async Task PaymentListData()
        {
            IsLoading = true;
            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new Payment_Info { ContractId = ConInf.ContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Payment/PaymentList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    payment_Infos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment_Info>>(Rs.Data.ToString());
                    payment_InfosNew = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment_Info>>(Rs.Data.ToString());
                }
            }
            IsLoading = false;
        }

        async Task CheckPermission()
        {
            Authens userData = new Authens();
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
                var menu = userData.MenuList.Find(x => x.MenuPath == path);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        async Task OnChangeFocus(string value, string name)
        {
            await jsRuntime.InvokeVoidAsync("focusInput", name);

            StateHasChanged();
        }

        async Task OnChangeContractDiscount(decimal value, string name)
        {
            await Task.Run(() =>
            {

                ConInfNew.AfterDiscount = ConInfNew.Credit - value;
                ConInfNew.ContractFirstPay = ConInfNew.ContractFirstPeriodAmount - value;
            });
        }

        async Task ConfirmDelete(Payment_Info daTa, string name)
        {
            //var confirmationResult = await this.DialogService.Confirm($"ลบ {daTa.MaterialCode} - {daTa.MaterialName} หรือไม่?"
            //    , "Delete Confirm"
            //    , new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
            //if (confirmationResult == true)
            //{
            //    isLoading = true;

            //    IList<PlaningSearchValues> Ds = new List<PlaningSearchValues>();
            //    Ds.Add(daTa);

            //    if (await ConfirmDeletes(Ds))
            //    {
            //        await GetData();
            //    }

            //    isLoading = false;
            //}

            if (name == "done")
            {
                daTa.Marking = "";
                payment_Infos_Delete.Remove(daTa);
            }
            else
            {
                daTa.Marking = "Delete";
                payment_Infos_Delete.Add(daTa);
            }

        }

        async Task ConfirmDeleteNew(Payment_Info daTa, string name)
        {
            //var confirmationResult = await this.DialogService.Confirm($"ลบ {daTa.MaterialCode} - {daTa.MaterialName} หรือไม่?"
            //    , "Delete Confirm"
            //    , new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
            //if (confirmationResult == true)
            //{
            //    isLoading = true;

            //    IList<PlaningSearchValues> Ds = new List<PlaningSearchValues>();
            //    Ds.Add(daTa);

            //    if (await ConfirmDeletes(Ds))
            //    {
            //        await GetData();
            //    }

            //    isLoading = false;
            //}

            if (name == "done")
            {
                daTa.Marking = "";
                payment_Infos_DeleteNew.Remove(daTa);
            }
            else
            {
                daTa.Marking = "Delete";
                payment_Infos_DeleteNew.Add(daTa);
            }

        }

        async Task OnChangeCashCode(string value, string name)
        {
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
                        if (name == "CashCode")
                        {
                            ConInf.CashCode = Sale.SaleCode;
                            ConInf.CashName = Sale.SaleName;
                        }
                        else
                        {
                            ConInfNew.CashCode = Sale.SaleCode;
                            ConInfNew.CashName = Sale.SaleName;
                        }
                    }
                }
            }
        }

        async Task Save()
        {
            if (ConInfNew.OpenDate == null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "เลือกวันที่เปิดปักษ์ สัญญาใหม่ ด้วย");
                return;
            }
            if (ConInfNew.CloseDate == null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "เลือกวันที่ปิดปักษ์ สัญญาใหม่ ด้วย");
                return;
            }

            IsLoading = true;

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            ExecResult RsCheck = new ExecResult();
            var postBodyCheck = new Contract_Info { ContractNo = ConInfOrg.ContractNo, RefNo = ConInfOrg.RefNo, UserData = userData };
            var responseCheck = await Http.PostAsJsonAsync("Contract/CheckCloneData", postBodyCheck);
            RsCheck = await responseCheck.Content.ReadFromJsonAsync<ExecResult>();
            if (RsCheck != null)
            {
                if (!RsCheck.IsSuccess)
                {
                    await dialogService.Alert(RsCheck.Msg, "Clone ไม่ได้", new AlertOptions() { OkButtonText = "OK" });
                    return;
                }
            }

            ConInf.ContractStatusDate = ConInfNew.AccStatusDate;

            payment_Infos_DeleteNew.ToList().ForEach(c => c.RefNo = ConInfNew.RefNo);
            payment_Infos_DeleteNew.ToList().ForEach(c => c.ContractNo = ConInfNew.ContractNo);

            var postBody = new Contract_Info_Clone 
            {
                ConInfo = ConInf,
                ConInfoNew = ConInfNew,
                PayDataDelete = payment_Infos_Delete,
                PayDataDeleteNew = payment_Infos_DeleteNew,
                UserData = userData,
                CreatedBy = userData.UserID
            };
            var response = await Http.PostAsJsonAsync("Contract/SaveCloneData", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    Rs.Data = postBody;
                    dialogService.Close(Rs);
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
            IsLoading = false;
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
                        ConInfNew.Model = md.MODEL;
                        ConInfNew.ModelDesc = md.Des;
                        ConInfNew.Credit = md.Sales;
                        ConInfNew.Cash = md.CASH;
                        ConInfNew.ContractPeriod = md.MODE;
                        ConInfNew.ContractFirstPeriodAmount = md.credit2;
                        ConInfNew.Sales = md.CASH;
                        ConInfNew.ContractPeriodAmount = md.CREDIT;
                        ConInfNew.ContractDiscount = 0;
                        ConInfNew.AfterDiscount = ConInfNew.Credit;
                        ConInfNew.ContractFirstPay = ConInfNew.ContractFirstPeriodAmount;
                    }
                }
            }
        }
    }
}
