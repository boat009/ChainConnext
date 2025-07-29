using ChainConnext.Shared.BD;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Client.Services;
using ChainConnext.Shared;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Json;
using ChainConnext.Shared.Contracts;
using ChainConnext.Shared.Reports;

namespace ChainConnext.Client.Pages
{
    public partial class ContractList
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        bool IsLoading = false;
        Contract_Info_Find Cont = new Contract_Info_Find();
        List<Contract_Info_Find> ContData = new List<Contract_Info_Find>();

        IList<Contract_Info_Find>? selectedItems;

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
            Cont.UserData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (Cont.UserData.PermsList.Count > 0)
            {
                var perms = Cont.UserData.PermsList.Find(x => x.PermsName == "IsSave");
                if (perms != null)
                {
                    IsSave = perms.IsPerms;
                }
                perms = Cont.UserData.PermsList.Find(x => x.PermsName == "IsDelete");
                if (perms != null)
                {
                    IsDelete = perms.IsPerms;
                }
            }
            //if (Cont.UserData.MenuList.Count > 0)
            //{
            //    var path = Navigation.Uri.Replace(Navigation.BaseUri, "");
            //    var menu = Cont.UserData.MenuList.Find(x => x.MenuId == 19);
            //    if (menu != null)
            //    {
            //        IsAccess = menu.IsAccess;
            //        IsSave = menu.IsSave;
            //        IsDelete = menu.IsDelete;
            //    }
            //}
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

            if (Cont.RefNo != null)
            {
                if (!string.IsNullOrEmpty(Cont.RefNo.Trim()))
                {
                    is_Search = true;
                }
            }
            if (Cont.ContractNo != null)
            {
                if (!string.IsNullOrEmpty(Cont.ContractNo.Trim()))
                {
                    is_Search = true;
                }
            }
            if (Cont.CustomerName != null)
            {
                if (!string.IsNullOrEmpty(Cont.CustomerName.Trim()))
                {
                    is_Search = true;
                }
            }
            if (Cont.CitizenId != null)
            {
                if (!string.IsNullOrEmpty(Cont.CitizenId.Trim()))
                {
                    is_Search = true;
                }
            }
            if (Cont.BranchCode != null)
            {
                if (!string.IsNullOrEmpty(Cont.BranchCode.Trim()))
                {
                    is_Search = true;
                }
            }
            if (Cont.SerialNo != null)
            {
                if (!string.IsNullOrEmpty(Cont.SerialNo.Trim()))
                {
                    is_Search = true;
                }
            }
            if (Cont.EffDate != null)
            {
                is_Search = true;
            }

            if (is_Search)
            {
                var response = await Http.PostAsJsonAsync("Contract/ListFind", Cont);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();

                Logger.LogInformation(Rs.Msg);
                Logger.LogInformation(Rs.JsonData);

                if (Rs != null)
                {
                    //Logger.LogInformation(Rs.Msg);
                    if (Rs.Rows > 0)
                    {
                        ContData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Info_Find>>(Rs.Data.ToString());
                    }
                }

                if (ContData.Count == 0)
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Warning", "ไม่พบข้อมูล");
                }
            }

            IsLoading = false;
        }
        async Task OnCellContextMenu(DataGridCellMouseEventArgs<Contract_Info_Find> args)
        {
            List<Contract_Info_Find> selectedTmpRpt = new List<Contract_Info_Find>() { args.Data };

            Contract_Info_Find tmp = selectedTmpRpt.FirstOrDefault();

            ContextMenuService.Open(args,
                    new List<ContextMenuItem> {
                new ContextMenuItem(){ Text = "ดูข้อมูล", Value = 1, Icon = "info" },
                    },
                async (e) =>
                {
                    //console.Log($"Menu item with Value={e.Value} clicked. Column: {args.Column.Property}, EmployeeID: {args.Data.EmployeeID}");
                    switch (e.Value)
                    {
                        case 1:
                            {
                                await ViewData(tmp, "View From ContextMenu");
                            }
                            break;
                    }
                }
                 );
        }

        async Task ViewData(Contract_Info_Find daTa, string name)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("open", $"contract/{daTa.RefNo}", "_blank");
            }
            catch { }
        }

        async Task OnRowClick(DataGridRowMouseEventArgs<Contract_Info_Find> args)
        {
            await ViewData(args.Data, "View From ContextMenu");
        }
    }
}
