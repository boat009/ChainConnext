
using ChainConnext.Client.Services;
using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Reports;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Radzen;
using System.Globalization;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChainConnext.Client.Pages.ARs
{
    public partial class ARDailyImportCheck
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        Authens userData = new Authens();
        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        int Height;
        int Width;
        bool IsLoad = false;
        DateTime? FindDate = new DateTime(2023, 11, 1);
        bool IsNonSave = false;
        bool IsChange = false;

        List<BD_accstatus> ListDetailMastCont = new List<BD_accstatus>();
        IList<BD_accstatus>? selectedDetailMastCont;

        string StatusRow = "-";
        private double currentPregress = 0;

        protected override async Task OnInitializedAsync()
        {
            IsLoad = true;


            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            await CheckPermission();

            if (IsAccess)
            {
                if (Navigation.BaseUri.Contains("localhost"))
                {
                    FindDate = new DateTime(2023, 11, 1);
                }
                else
                {
                    FindDate = DateTime.Now;
                }
                await GetData();
            }

            IsLoad = false;
        }

        async Task CheckPermission()
        {
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (userData.MenuList.Count > 0)
            {
                var menu = userData.MenuList.Find(x => x.MenuId == 31);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsSave;
                }
            }
        }
        async Task OnDateChange(DateTime? date)
        {
            if (date == null)
            {
                return;
            }
            if (date.Value.ToString("MMyyyy") == FindDate.Value.ToString("MMyyyy"))
            {
                return;
            }
            FindDate = date.Value;
            await GetData();
        }

        async Task OnButtonDateChange(int add_day)
        {
            FindDate = FindDate.Value.AddMonths(add_day);
            await GetData();
        }

        async Task GetData()
        {
            IsLoad = true;
            ListDetailMastCont = new List<BD_accstatus>();

            string toAcc = "";

            if (IsNonSave)
            {
                toAcc = "1";
            }

            var postBody = new BD_accstatus { effdate = FindDate, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/AccStatusEffDateListCheck", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        ListDetailMastCont = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_accstatus>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                    Logger.LogError(Rs.Msg);
                }
            }
            IsLoad = false;
        }

        async Task ShowBusyDialogProgress()
        {
            var confirmationResult = await this.dialogService.Confirm($"นำเข้าข้อมูล หรือไม่?"
                , "Save Confirm"
                , new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
            if (confirmationResult == true)
            {
                InvokeAsync(async () =>
                {
                    // Simulate background task

                    await RunSaveProgress();

                    // Close the dialog
                    dialogService.Close();

                    if (!is_success)
                    {
                        NotificationService.Notify(NotificationSeverity.Error, "Error", sql_msg);
                    }
                    else
                    {
                        NotificationService.Notify(NotificationSeverity.Success, "Success", sql_msg);
                    }

                    StateHasChanged();
                });

                await BusyDialog("กำลังบันทึกข้อมูล...");
            }
            if (is_success)
            {
                await GetData();
            }
        }

        bool is_success = false;
        string sql_msg = "";
        async Task RunSaveProgress()
        {
            IsLoad = true;

            for (int i = 0; i < ListDetailMastCont.Count; i++)
            {
                currentPregress = (i + 1) * 100 / ListDetailMastCont.Count;
                StatusRow = $"{(i + 1)} / {ListDetailMastCont.Count}";

                //BD_accstatus C = ListDetailMastCont[i];
                //if (!C.is_toacc)
                //{
                //    var postBody = new BD_MastCont
                //    {
                //        RefNo = C.RefNo,
                //        CONTNO = C.CONTNO,
                //        CreatedBy = userData.UserID,
                //        UserData = userData
                //    };
                //    var response = await Http.PostAsJsonAsync("BD/AccStatusImport", postBody);

                //    ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                //    if (Rs != null)
                //    {
                //        if (!Rs.IsSuccess)
                //        {
                //            Logger.LogInformation(Rs.Msg);
                //            break;
                //        }
                //        is_success = Rs.IsSuccess;
                //        sql_msg = Rs.Msg;
                //    }
                //}
                //else
                //{
                //    is_success = true;
                //    //if (i == 0)
                //    //{
                //    //   await Task.Delay(10);
                //    //}
                //}

                StateHasChanged();
            }

            await Task.Delay(TimeSpan.FromSeconds(0.5));

            IsLoad = false;

            StateHasChanged();
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

        async Task OnCellContextMenu(DataGridCellMouseEventArgs<BD_accstatus> args)
        {
            selectedDetailMastCont = new List<BD_accstatus>() { args.Data };

            BD_accstatus tmp = selectedDetailMastCont.FirstOrDefault();

            ContextMenuService.Open(args,
                    new List<ContextMenuItem> {
                new ContextMenuItem(){ Text = "ดูข้อมูล", Value = 1, Icon = "info" },
                new ContextMenuItem(){ Text = "บันทึกข้อมูล", Value = 2, Icon = "save" },
                new ContextMenuItem(){ Text = "ลบ", Value = 3, Icon = "delete" },
                    },
                async (e) =>
                {
                    //console.Log($"Menu item with Value={e.Value} clicked. Column: {args.Column.Property}, EmployeeID: {args.Data.EmployeeID}");
                    switch (e.Value)
                    {
                        case 1:
                            {
                                //List<BD_accstatus> ListDetail = new List<BD_accstatus>();
                                //ListDetail = await LoadAccStatusEffDateDetailList(tmp);

                                await dialogService.OpenAsync<ARAccStatusDetailList>($"รายการสัญญา วันที่ {tmp.effdate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}",
                                      new Dictionary<string, object>() { { "pEffdate", tmp.effdate } },
                                      new DialogOptions() { Width = "1300px", Height = "1000px" });
                            }
                            break;
                        case 2:
                            {

                            }
                            break;
                        case 3:
                            {

                            }
                            break;
                    }
                }
                 );
        }

        async Task<List<BD_accstatus>> LoadAccStatusEffDateDetailList(BD_accstatus x)
        {
            IsLoad = true;
            List<BD_accstatus>  ListDetail = new List<BD_accstatus>();

            var postBody = new BD_accstatus { effdate = x.effdate, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/AccStatusEffDateDetailList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        ListDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_accstatus>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                    Logger.LogError(Rs.Msg);
                }
            }
            IsLoad = false;

            return ListDetail;
        }

    }
}
