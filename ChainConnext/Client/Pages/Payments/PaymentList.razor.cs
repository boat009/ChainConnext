using ChainConnext.Client.Services;
using ChainConnext.Shared;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Payments;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Payments
{
    public partial class PaymentList
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        bool IsLoading = false;
        BD_InvoiceABH Bd = new BD_InvoiceABH();
        List<BD_InvoiceABH> bD_Invoices = new List<BD_InvoiceABH>();

        int Height;
        int Width;

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        protected override async Task OnInitializedAsync()
        {
            await GetDimension();

            await CheckPermission();
        }

        async Task CheckPermission()
        {
            Bd.UserData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (Bd.UserData.PermsList.Count > 0)
            {
                var perms = Bd.UserData.PermsList.Find(x => x.PermsName == "IsSave");
                if (perms != null)
                {
                    IsSave = perms.IsPerms;
                }
                perms = Bd.UserData.PermsList.Find(x => x.PermsName == "IsDelete");
                if (perms != null)
                {
                    IsDelete = perms.IsPerms;
                }
            }
            if (Bd.UserData.MenuList.Count > 0)
            {
                var path = Navigation.Uri.Replace(Navigation.BaseUri, "");
                var menu = Bd.UserData.MenuList.Find(x => x.MenuId == 19);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        async Task GetDimension()
        {
            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;
        }

        async Task Search()
        {
            //await ShowBusyDialogLoad();
            await RunProgress();
        }

        async Task ShowBusyDialogLoad()
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

        async Task RunProgress()
        {
            IsLoading = true;

            bool is_Search = false;

            if (Bd.RefNo != null)
            {
                if (!string.IsNullOrEmpty(Bd.RefNo.Trim()))
                {
                    is_Search = true;
                }
            }
            if (Bd.ContNo != null)
            {
                if (!string.IsNullOrEmpty(Bd.ContNo.Trim()))
                {
                    is_Search = true;
                }
            }
            if (Bd.InvNo != null)
            {
                if (!string.IsNullOrEmpty(Bd.InvNo.Trim()))
                {
                    is_Search = true;
                }
            }

            if (is_Search)
            {
                var response = await Http.PostAsJsonAsync("BD/ListPayTrans", Bd);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    //Logger.LogInformation(Rs.Msg);
                    if (Rs.Rows > 0)
                    {
                        bD_Invoices = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_InvoiceABH>>(Rs.Data.ToString());
                    }
                }

                if (bD_Invoices.Count == 0)
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Warning", "ไม่พบข้อมูล");
                }
            }

            IsLoading = false;
        }
    }
}
