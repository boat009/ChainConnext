using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using System.Net.Http.Json;
using ChainConnext.Shared.Toss;
using Radzen;
using ChainConnext.Shared.BD;

namespace ChainConnext.Client.Pages.Contracts
{
    public partial class ContractSerialNo
    {
        [Parameter]
        public string? pContractId { get; set; }
        [Parameter]
        public int pHeight { get; set; }
        [Parameter]
        public int pWidth { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        List<Contract_SerialNoType> SnType = new List<Contract_SerialNoType>();
        List<Contract_SerialNo> contract_SerialNos = new List<Contract_SerialNo>();
        IList<Contract_SerialNo>? selectedSn;

        Contract_SerialNo CSn = new Contract_SerialNo();

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();
        bool DisableSaveSn = true;

        bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            if (pContractId == null)
            {
                DisableSaveSn = true;
            }
            else
            {
                DisableSaveSn = false;
            }
            await CheckPermission();
            await GetSerialNoTypeData();

            await GetSerialNoData();
        }
        bool IsSavePro = false;
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
                var menu2 = userData.MenuList.Find(x => x.MenuId == 79);
                if (menu2 != null)
                {
                    IsSavePro = menu2.IsSave;
                }
            }
        }

        private async Task GetSerialNoTypeData()
        {
            var postBody = new Contract_SerialNoType();
            var response = await Http.PostAsJsonAsync("Contract/ListSerialNoType", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    SnType = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_SerialNoType>>(Rs.Data.ToString());
                }
            }
        }
        private async Task GetSerialNoData()
        {
            if (pContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pContractId.Trim()))
            {
                return;
            }

            isLoading = true;

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            var postBody = new Contract_SerialNo { ContractId = pContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Contract/ListSerialNo", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    contract_SerialNos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_SerialNo>>(Rs.Data.ToString());
                }
            }

            isLoading = false;
        }

        async Task SaveSerialNo()
        {
            if (pContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pContractId.Trim()))
            {
                return;
            }

            if (CSn.SerialNoTypeId == 0)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "ประเภทด้วย");
                return;
            }

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            CSn.ContractId = pContractId;
            CSn.UserData = userData;
            CSn.CreatedBy = userData.UserID;

            var response = await Http.PostAsJsonAsync("Contract/SaveSerialNo", CSn);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    CSn = new Contract_SerialNo();
                    await GetSerialNoData();
                    NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
        }

        async Task OnSnCellClick(DataGridCellMouseEventArgs<Contract_SerialNo> args)
        {
            CSn = args.Data;
        }

        async Task OpenModelDelete(Contract_SerialNo daTa, string key)
        {
            var DRs = await dialogService.Confirm($"ลบ {daTa.SerialNoTypeName} - {daTa.SerialNo} หรือไม่?", $"ยืนยัน ลบ {daTa.SerialNoTypeName} - {daTa.SerialNo}", new ConfirmOptions() { OkButtonText = "ใช่", CancelButtonText = "ไม่" });
            if (DRs.Value)
            {
                daTa.CreatedBy = userData.UserID;
                var response = await Http.PostAsJsonAsync("Contract/DeleteSerialNo", daTa);
                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        await GetSerialNoData();
                        NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                    }
                    else
                    {
                        Logger.LogInformation(Rs.Msg);
                        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                    }
                }
            }
        }
    }
}
