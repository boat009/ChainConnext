using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Contracts
{
    public partial class ContractReNew
    {
        [Parameter]
        public string? ContractId { get; set; }
        [Parameter]
        public string? Key { get; set; }
        [Parameter]
        public string? ConInfData { get; set; }

        Contract_Info ConInf = new Contract_Info();
        Contract_Info ConInfNew = new Contract_Info();

        string Sender = "";
        string Remark = "";

        bool IsLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            await Task.Run(()=>
            {
                if (ConInfData == null)
                {
                    return;
                }
                if (string.IsNullOrEmpty(ConInfData.Trim()))
                {
                    return;
                }
                //Logger.LogInformation(ConInfData);
                ConInf = Newtonsoft.Json.JsonConvert.DeserializeObject<Contract_Info>(ConInfData);

                ConInfNew = Newtonsoft.Json.JsonConvert.DeserializeObject<Contract_Info>(ConInfData);
                ConInfNew.SerialNo = "";
                ConInfNew.ChangeDate = DateTime.Now;
            }
            );
        }

        async Task OnChangeFocus(string value, string name)
        {
            await jsRuntime.InvokeVoidAsync("focusInput", name);
            StateHasChanged();
        }

        async Task OnSave()
        {
            if (ConInfNew.ChangeDate == null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "เลือก วันที่ ด้วย");
                return;
            }
            if (string.IsNullOrEmpty(Sender))
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "กรอกข้อมูล ผู้ส่งเครื่อง ด้วย");
                return;
            }
            if (string.IsNullOrEmpty(Remark))
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "กรอกข้อมูล หมายเหตุ ด้วย");
                return;
            }
            IsLoading = true;

            var postBodySn = new Contract_SerialNo { SerialNo = ConInfNew.SerialNo };
            var responseSn = await Http.PostAsJsonAsync("Contract/CheckSerialNo", postBodySn);
            ExecResult? RsSn = await responseSn.Content.ReadFromJsonAsync<ExecResult>();
            if (RsSn != null)
            {
                if (RsSn.Rows > 0)
                {
                    if (Convert.ToBoolean(RsSn.ID))
                    {
                        NotificationService.Notify(NotificationSeverity.Error, "Error", $"เลขเครื่อง <b>{ConInfNew.SerialNo}</b> มีในระบบแล้ว");

                        IsLoading = false;
                        return;
                    }
                }
            }

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new Contract_Info_Clone
            {
                ConInfo = ConInf,
                ConInfoNew = ConInfNew,
                Serder = Sender,
                Remark = Remark,
                UserData = userData,
                CreatedBy = userData.UserID
            };
            var response = await Http.PostAsJsonAsync("Contract/SaveChangeData", postBody);

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
    }
}
