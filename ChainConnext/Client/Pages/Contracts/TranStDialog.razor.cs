using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Payments;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Contracts
{
    public partial class TranStDialog
    {
        [Parameter]
        public string? pContractId { get; set; }
        [Parameter]
        public string? pRefNo { get; set; }
        [Parameter]
        public string? Key { get; set; }

        bool IsLoading = false;
        List<TranSt> tranSts = new List<TranSt>();

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        Authens userData = new Authens();

        protected override async Task OnInitializedAsync()
        {
            await ShowBusyDialogWithLink();
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

        async Task RunProgressWithLink()
        {
            IsLoading = true;

            await CheckPermission();

            if (pContractId == null)
            {
                return;
            }
            if (pRefNo == null)
            {
                return;
            }

            var postBody = new TranSt { RefNo = pRefNo, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/ListTranSt", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    tranSts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TranSt>>(Rs.Data.ToString());
                }
            }

            IsLoading = false;
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
    }
}
