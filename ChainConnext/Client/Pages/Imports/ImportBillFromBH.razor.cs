using ChainConnext.Shared.Reports;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Authen;
using Radzen;
using System.Net.Http.Json;
using ChainConnext.Shared;
using System.Data;
using ChainConnext.Shared.BD;
using Radzen.Blazor;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml.Linq;
using Microsoft.JSInterop;
using ChainConnext.Client.Services;
using OfficeOpenXml;
using System.Reflection;
using ChainConnext.Shared.Customers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ChainConnext.Shared.Payments;

namespace ChainConnext.Client.Pages.Imports
{
    public partial class ImportBillFromBH
    {

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        Rpt_Parameter Rpt = new Rpt_Parameter();
        List<Tmp_ReportDaily_Payment> tmp_Reports = new List<Tmp_ReportDaily_Payment>();
        List<Rpt_CashCode> CashCodeData = new List<Rpt_CashCode>();

        List<Tmp_ReportDaily_Payment> tmp_Reports_TmpList = new List<Tmp_ReportDaily_Payment>();

        DataTable dtRptTmp = new DataTable();
        List<string> HeadData = new List<string>();

        int selectedIndex = 0;

        string TitlePage = "";

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();

        bool IsLoading = false;
        bool IsData = false;
        bool IsExport = false;

        private double currentPregress = 0;

        int Height;
        int Width;

        bool IsSaveDisable = true;

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            await CheckPermission();

            if (IsAccess)
            {
                if (Navigation.BaseUri.Contains("localhost") || Navigation.BaseUri.Contains("uat."))
                {
                    Rpt.DateFrom = new DateTime(2023, 11, 6);
                    Rpt.ForTest = true;
                    if (Rpt.ForTest)
                    {
                        await ListTmpNo();
                    }
                }
                else
                {
                    Rpt.DateFrom = DateTime.Now;
                }
            }

            IsLoading = false;
        }

        private async Task ListTmpNo()
        {

            var postBody = new Tmp_ReportDaily_Payment { UserData = userData };
            var response = await Http.PostAsJsonAsync("BH/RptImpPaymentListTmpNo", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Data != null)
                {
                    tmp_Reports_TmpList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Tmp_ReportDaily_Payment>>(Rs.Data.ToString());
                }
            }
        }

        async Task CheckPermission()
        {
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            Rpt.UserData = userData;

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
                var menu = userData.MenuList.Find(x => x.MenuId == 12);
                if (menu != null)
                {
                    TitlePage = menu.MenuDesc;
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        async Task ShowBusyDialog()
        {
            InvokeAsync(async () =>
            {
                // Simulate background task

                await Find();

                // Close the dialog
                dialogService.Close();

                //await RunProgress();
                //await ShowBusyDialogProgress();
            });

            //await BusyDialog("Loading...");
            await BusyDialog();
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

        string TabTxt0 = "ทั้งหมด";
        string TabTxt1 = "เต็มงวด";
        string TabTxt2 = "บางส่วน";
        string TabTxt3 = "ตัดสด";
        string TabTxt4 = "งวดซ้ำ";
        string TabTxt5 = "Error";
        string TabTxt6 = "สัญญา X";
        string TabTxt7 = "ยังไม่บันทึก";
        string TabTxt8 = "บันทึกแล้ว";
        async Task Find()
        {
            if (Rpt.DateFrom == null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "เลือกวันที่ ด้วย");
                return;
            }
            IsData = false;
            IsLoading = true;

            Rpt.IsCashCodeData = false;

            if (Rpt.ForTest)
            {

            }

            ExecResult Rs = new ExecResult();

            var response = await Http.PostAsJsonAsync("BH/RptImpPayment", Rpt);

            Rs = await response.Content.ReadFromJsonAsync<ExecResult>();

            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    tmp_Reports = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Tmp_ReportDaily_Payment>>(Rs.Data.ToString());
                    if (tmp_Reports.Count > 0)
                    {
                        IsData = true;
                        IsExport = true;
                        IsSaveDisable = false;

                        TabTxt0 = $"ทั้งหมด ({tmp_Reports.Count})";
                        TabTxt1 = $"เต็มงวด ({tmp_Reports.FindAll(x => x.PayPartial == false && x.CanSave == true).Count})";
                        TabTxt2 = $"บางส่วน ({tmp_Reports.FindAll(x => x.PayPartial == true && x.CanSave == true).Count})";
                        TabTxt3 = $"ตัดสด ({tmp_Reports.FindAll(x => x.PayVat == "1" && x.CanSave == true).Count})";
                        TabTxt4 = $"งวดซ้ำ ({tmp_Reports.FindAll(x => x.CheckMastPay == true && x.CanSave == true).Count})";
                        TabTxt6 = $"สัญญา X ({tmp_Reports.FindAll(x => x.CanSave == false).Count})";
                        TabTxt7 = $"ยังไม่บันทึก ({tmp_Reports.FindAll(x => x.CheckInvoiceABH == false && x.CanSave == true).Count})";
                        TabTxt8 = $"บันทึกแล้ว ({tmp_Reports.FindAll(x => x.CheckInvoiceABH == true && x.CanSave == true).Count})";
                    }
                    else
                    {
                        IsData = false;
                        IsExport = false;
                    }
                }
            }
            if(!IsData)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "ไม่พบข้อมูล");
            }

            IsLoading = false;
            StateHasChanged();
        }
        string StatusRow = "-";
        string StatusCashCode = "-";
        bool is_success = false;
        string sql_msg = "";
        async Task RunProgress()
        {
            //OpenModal();
            IsLoading = true;

            //for (int i = 0; i <= 50; i++)
            //{
            //    currentPregress = i*2;
            //    //currentPregress = i * 10;
            //    await Task.Delay(100);
            //    StateHasChanged();
            //}
            List<Tmp_ReportDaily_Payment> tmpRpts = new List<Tmp_ReportDaily_Payment>();
            tmpRpts = GetTmpDataByTab(selectedIndex);

            for (int i = 0; i < tmpRpts.Count; i++)
            {
                Tmp_ReportDaily_Payment tmp = tmpRpts[i];
                tmp.UserData = userData;
                tmp.CreateBy = userData.UserID;

                if (Navigation.BaseUri.Contains("localhost"))
                {
                    tmp.ForTest = true;
                }

                currentPregress = (i + 1) * 100 / tmpRpts.Count;
                StatusRow = $"{(i + 1)} / {tmpRpts.Count}";

                await OnSavePayment(tmp);

                StateHasChanged();
            }

            IsLoading = false;
            //CloseModal();
            StateHasChanged();
        }

        async Task<bool> OnSavePayment(Tmp_ReportDaily_Payment tmp)
        {
            if (tmp.CanSave)
            {
                tmp.UserData = userData;
                tmp.CreateBy = userData.UserID;

                var response = await Http.PostAsJsonAsync("BH/RptPaymentSave", tmp);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    tmp.IsError = !Rs.IsSuccess;
                    tmp.ErrorMsg = Rs.Msg;
                    tmp.ErrorBy = userData.UserID;
                    tmp.UserData = userData;

                    var responseerror = await Http.PostAsJsonAsync("BH/RptPaymentErrorSave", tmp);
                    ExecResult? RsErr = await responseerror.Content.ReadFromJsonAsync<ExecResult>();

                    if (!Rs.IsSuccess)
                    {
                        is_success = Rs.IsSuccess;
                        Logger.LogInformation(Rs.Msg);
                    }
                    else
                    {
                        is_success = Rs.IsSuccess;
                        sql_msg = Rs.Msg;
                    }
                }
            }
            else
            {
                await Task.Delay(100);
            }
            return is_success;
        }

        async Task ConfirmSave()
        {
            string SaveTxt = "";
            switch (selectedIndex)
            {
                case 1: SaveTxt = TabTxt1; break;
                case 2: SaveTxt = TabTxt2; break;
                case 3: SaveTxt = TabTxt3; break;
                case 4: SaveTxt = TabTxt4; break;
                case 5: SaveTxt = TabTxt5; break;
                case 6: SaveTxt = TabTxt6; break;
                case 7: SaveTxt = TabTxt7; break;
                case 8: SaveTxt = TabTxt8; break;
                default: SaveTxt = TabTxt0; break;
            }

            var confirmationResult = await this.dialogService.Confirm($"บันทึก {SaveTxt} หรือไม่?"
                , "Save Confirm"
                , new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
            if (confirmationResult == true)
            {
                await ShowBusyDialogProgress();
            }
        }

        async Task ConfirmSave1(Tmp_ReportDaily_Payment tmp)
        {
            var confirmationResult = await this.dialogService.Confirm($"บันทึก {tmp.InvNo} หรือไม่?"
                , "Save Confirm"
                , new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
            if (confirmationResult == true)
            {
                IsLoading = true;
                if (await OnSavePayment(tmp))
                {
                    NotificationService.Notify(NotificationSeverity.Success, "Success", sql_msg);
                    await Find();
                }
                else
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", sql_msg);
                }
                IsLoading = false;
                StateHasChanged();
            }
        }

        async Task ConfirmDelete1(Tmp_ReportDaily_Payment tmp)
        {
            var confirmationResult = await this.dialogService.Confirm($"ยกเลิก {tmp.InvNo} หรือไม่?"
                , "Cancel Confirm"
                , new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
            if (confirmationResult == true)
            {
                IsLoading = true;

                var postBody = new Payment_Info
                {
                    CreatedBy = userData.UserID,
                    RefNo = tmp.RefNo,
                    ContractNo = tmp.ContNo,
                    UserData = userData,
                    PayPeroid = tmp.PayPeriod,
                    PayDate = tmp.paydate,
                    InvNo = tmp.InvNo
                };
                var response = await Http.PostAsJsonAsync("Payment/DeletePayment", postBody);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                        await Find();
                    }
                    else
                    {
                        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                        Logger.LogError(Rs.Msg);
                    }
                }

                IsLoading = false;
                StateHasChanged();
            }
        }

        async Task ShowBusyDialogProgress()
        {
            InvokeAsync(async () =>
            {
                // Simulate background task

                await RunProgress();

                // Close the dialog
                dialogService.Close();

                TabTxt5 = $"Error({tmp_Reports.FindAll(x => x.IsError == true).Count})";

                IsSaveDisable = true;

                if ((tmp_Reports.FindAll(x => x.IsError == true).Count) > 0)
                {
                    //NotificationService.Notify(NotificationSeverity.Error, "Error", sql_msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = sql_msg, Duration = 5000 });
                }
                else
                {
                    NotificationService.Notify(NotificationSeverity.Success, "Success", sql_msg);
                }

                StateHasChanged();
            });

            await BusyDialog("กำลังบันทึกข้อมูล...");
        }

        async Task ShowBusyDialogExportExcel()
        {
            InvokeAsync(async () =>
            {
                // Simulate background task

                await RunExpoerExcel();

                // Close the dialog
                dialogService.Close();
            });

            await BusyDialog("กำลัง Export Excel...");
        }

        List<Tmp_ReportDaily_Payment> GetTmpDataByTab(int idex)
        {
            List<Tmp_ReportDaily_Payment> tmpRpts = new List<Tmp_ReportDaily_Payment>();
            switch (idex)
            {
                case 1:
                    {
                        tmpRpts = tmp_Reports.FindAll(x => x.PayPartial == false && x.CanSave == true);
                    }
                    break;
                case 2:
                    {
                        tmpRpts = tmp_Reports.FindAll(x => x.PayPartial == true && x.CanSave == true);
                    }
                    break;
                case 3:
                    {
                        tmpRpts = tmp_Reports.FindAll(x => x.PayVat == "1" && x.CanSave == true);
                    }
                    break;
                case 4:
                    {
                        tmpRpts = tmp_Reports.FindAll(x => x.CheckMastPay == true && x.CanSave == true);
                    }
                    break;
                case 5:
                    {
                        tmpRpts = tmp_Reports.FindAll(x => x.IsError == true);
                    }
                    break;
                case 6:
                    {
                        tmpRpts = tmp_Reports.FindAll(x => x.CanSave == false);
                    }
                    break;
                case 7:
                    {
                        tmpRpts = tmp_Reports.FindAll(x => x.CheckInvoiceABH == false && x.CanSave == true);
                    }
                    break;
                case 8:
                    {
                        tmpRpts = tmp_Reports.FindAll(x => x.CheckInvoiceABH == true && x.CanSave == true);
                    }
                    break;
                default:
                    {
                        tmpRpts = tmp_Reports;
                    }
                    break;
            }
            return tmpRpts;
        }

        async Task RunExpoerExcel()
        {
            if (tmp_Reports.Count > 0)
            {
                Type fieldsType = typeof(Tmp_ReportDaily_Payment);
                PropertyInfo[] props = fieldsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                DataTable dt = new DataTable();
                foreach (PropertyInfo prop in props)
                {
                    dt.Columns.Add(prop.Name);
                }

                List<Tmp_ReportDaily_Payment> tmpRpts = new List<Tmp_ReportDaily_Payment>();
                tmpRpts = GetTmpDataByTab(selectedIndex);

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                    ws.Cells["A1"].LoadFromCollection<Tmp_ReportDaily_Payment>(tmpRpts, true);
                    ws = BaseShared.FormatExccel(ws, dt);
                    var ms = new System.IO.MemoryStream();
                    pck.SaveAs(ms);
                    await jsRuntime.SaveAs("ReportDeailyPayment.xlsx", pck.GetAsByteArray());
                }
            }
        }

        void OnTabChange(int index)
        {
            List<Tmp_ReportDaily_Payment> tmpRpts = new List<Tmp_ReportDaily_Payment>();
            tmpRpts = GetTmpDataByTab(index);

            if (index == 6)
            {
                IsData = false;
                
            }
            else
            {
                if (tmpRpts.Count > 0)
                {
                    IsData = true;
                }
                else
                {
                    IsData = false;
                }
            }
            if (tmpRpts.Count > 0)
            {
                IsExport = true;
            }
            else
            {
                IsExport = false;
            }
        }
    }
}
