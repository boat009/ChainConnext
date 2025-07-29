using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using ChainConnext.Shared.Authen;
using Microsoft.AspNetCore.Components.Authorization;
using ChainConnext.Shared.Payments;
using ChainConnext.Shared.Supports;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Radzen;
using BlazorStrap.V5;
using Microsoft.AspNetCore.Components.Web;

namespace ChainConnext.Client.Pages.Contracts
{
    public partial class ContractInfo
    {
        [Parameter]
        public Contract_Info? pConInf { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        [Parameter]
        public Contract_Info ConInfEmp { get; set; } = new Contract_Info();
        List<MasterErr> masterErrsN = new List<MasterErr>();
        List<MasterErr> masterErrsR = new List<MasterErr>();

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public EventCallback<Contract_Info> OnSaveEmp { get; set; }
        [Parameter]
        public EventCallback<Contract_Info> OnSaveDue { get; set; }

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        bool DisableSave = true;

        Authens userData = new Authens();

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            Logger.LogInformation("ContractInfo OnInitializedAsync");

            //if (pConInf == null)
            //{
            //    return;
            //}

            await CheckPermission();

            await ErrsNData();
            await ErrsRData();

            IsLoading = false;
        }
        bool IsSaveEmp = false;
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
                var menu2 = userData.MenuList.Find(x => x.MenuId == 77);
                if (menu2 != null)
                {
                    IsSaveEmp = menu2.IsSave;
                }
            }
            if (pConInf != null)
            {
                if (pConInf.ContractId == null)
                {
                    DisableSave = true;
                }
                else
                {
                    DisableSave = false;
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

        //protected override async Task OnParametersSetAsync()
        //{
        //    await GetContractEmpData();
        //}

        private async Task GetContractEmpData()
        {
            if (pConInf == null)
            {
                return;
            }
            if (pConInf.ContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pConInf.ContractId.Trim()))
            {
                return;
            }

            var postBody = new Contract_Info { ContractId = pConInf.ContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Contract/GetContractEmp", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    ConInfEmp = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Info>>(Rs.Data.ToString()).FirstOrDefault();
                }
            }
        }
        async Task OnCodeChange(string value, string name)
        {
            switch (name)
            {
                case "SaleName":
                    {
                        ConInfEmp.SaleCode = value.ToUpper();
                    }
                    break;
                case "SetupName":
                    {
                        ConInfEmp.SetupCode = value.ToUpper();
                    }
                    break;
                case "CheckerName":
                    {
                        ConInfEmp.CheckerCode = value.ToUpper();
                    }
                    break;
                case "ServiceName":
                    {
                        ConInfEmp.ServiceCode = value.ToUpper();
                    }
                    break;
                case "CashName":
                    {
                        ConInfEmp.CashCode = value.ToUpper();
                    }
                    break;
                case "CollectorName":
                    {
                        ConInfEmp.CollectorCode= value.ToUpper();
                    }
                    break;
            }
        }

        async Task OnGetName(string value, string name)
        {
            IsLoading = true;

            switch (name)
            {
                case "SaleName":
                    {
                        ConInfEmp.SaleName = "";
                    }
                    break;
                case "SetupName":
                    {
                        ConInfEmp.SetupName = "";
                    }
                    break;
                case "CheckerName":
                    {
                        ConInfEmp.CheckerName = "";
                    }
                    break;
                case "ServiceName":
                    {
                        ConInfEmp.ServiceName = "";
                    }
                    break;
                case "CashName":
                    {
                        ConInfEmp.CashName = "";
                    }
                    break;
                case "CollectorName":
                    {
                        ConInfEmp.CollectorName = "";
                    }
                    break;
            }
            var postBody = new BDSaleName { SaleCode = value, SFindMode = name };
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
                                    ConInfEmp.SaleName = Sale.SaleName;
                                }
                                break;
                            case "SetupName":
                                {
                                    ConInfEmp.SetupName = Sale.SaleName;
                                }
                                break;
                            case "CheckerName":
                                {
                                    ConInfEmp.CheckerName = Sale.SaleName;
                                }
                                break;
                            case "ServiceName":
                                {
                                    ConInfEmp.ServiceName = Sale.SaleName;
                                }
                                break;
                            case "CashName":
                                {
                                    ConInfEmp.CashCode = Sale.SaleCode;
                                    ConInfEmp.CashName = Sale.SaleName;
                                }
                                break;
                            case "CollectorName":
                                {
                                    ConInfEmp.CollectorName = Sale.SaleName;
                                }
                                break;
                        }
                    }
                }
            }
            IsLoading = false;
            StateHasChanged();
        }
        async Task SaleCodeEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await OnGetName(ConInfEmp.SaleCode, "SaleName");
            }
        }
        async Task SetupCodeEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await OnGetName(ConInfEmp.SetupCode, "SetupName");
            }
        }
        async Task CheckerCodeEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await OnGetName(ConInfEmp.CheckerCode, "CheckerName");
            }
        }
        async Task ServiceCodeEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await OnGetName(ConInfEmp.ServiceCode, "ServiceName");
            }
        }
        async Task CashCodeEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await OnGetName(ConInfEmp.CashCode, "CashName");
            }
        }
        async Task CollectorCodeEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await OnGetName(ConInfEmp.CollectorCode, "CollectorName");
            }
        }

        async Task Save()
        {
            
            if (pConInf == null)
            {
                return;
            }

            ConInfEmp.ContractId = pConInf.ContractId;
            ConInfEmp.ContractNo = pConInf.ContractNo;
            ConInfEmp.RefNo = pConInf.RefNo;
            ConInfEmp.UserData = userData;
            ConInfEmp.UpdatedBy = userData.UserID;

            await OnSaveEmp.InvokeAsync(ConInfEmp);

            //IsLoading = true;

            

            //var response = await Http.PostAsJsonAsync("Contract/SaveEmp", ConInfEmp);

            //ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            //if (Rs != null)
            //{
            //    if (Rs.IsSuccess)
            //    {
            //        NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
            //    }
            //    else
            //    {
            //        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
            //    }
            //}

            //IsLoading = false;
        }

        async Task OnDueDateChange(DateTime? date)
        {
            ConInfEmp.CreditFnNo = 0;
            ConInfEmp.CreditFnYear = 0;
            var postBody = new BD_CreditTip {  DueDate = date };
            var response = await Http.PostAsJsonAsync("BD/CreditTripGetFn", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    var creditTip = Newtonsoft.Json.JsonConvert.DeserializeObject<BD_CreditTip>(Rs.Data.ToString());
                    if (creditTip != null)
                    {
                        ConInfEmp.CreditFnNo = creditTip.periodno;
                        ConInfEmp.CreditFnYear = creditTip.ayear;
                    }
                }
            }
        }

        async Task SaveDue()
        {
            if (pConInf == null)
            {
                return;
            }
            pConInf.UserData = userData;
            pConInf.UpdatedBy = userData.UserID;
            pConInf.CreditFnNo = ConInfEmp.CreditFnNo;
            pConInf.CreditFnYear = ConInfEmp.CreditFnYear;
            pConInf.DueDate = ConInfEmp.DueDate;

            await OnSaveDue.InvokeAsync(pConInf);

            //IsLoading = true;

            //var response = await Http.PostAsJsonAsync("Contract/SaveDue", pConInf);

            //ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            //if (Rs != null)
            //{
            //    if (Rs.IsSuccess)
            //    {
            //        NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
            //    }
            //    else
            //    {
            //        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
            //    }
            //}

            //IsLoading = false;
        }
    }
}
