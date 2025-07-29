using ChainConnext.Client.Services;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.Cms;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using Radzen;
using System.Data;
using System.Net.Http.Json;
using System.Reflection;
using ChainConnext.Shared.Imps;

namespace ChainConnext.Client.Pages.Imports
{
    public partial class ImportBHCheck
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        List<ImpBHCheck> CmsData = new List<ImpBHCheck>();
        IList<ImpBHCheck>? selectedData;

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();

        bool IsLoading = false;
        bool IsData = false;

        int Height;
        int Width;

        string TitlePage = "";

        string StatusRow = "-";
        private double currentPregress = 0;
        bool IsSaveDisable = true;

        int progress;
        bool showProgress;
        bool showComplete;
        string completionMessage;
        bool cancelUpload = false;

        bool IsExport = false;

        int selectedIndex = 0;

        DataTable dtCheckHeader = new DataTable();
        DataSet dsCheckHeader = new DataSet();

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            await CheckPermission();


            if (IsAccess)
            {
                await GetExamFile();
                if (Navigation.BaseUri.Contains("localhost"))
                {

                }
            }

            IsLoading = false;
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
                var menu = userData.MenuList.Find(x => x.MenuId == 87);
                if (menu != null)
                {
                    TitlePage = menu.MenuDesc;
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        private async Task GetExamFile()
        {
            var postBody = new ExecResult();
            var response = await Http.PostAsJsonAsync("FileExam/ImpBHCheck", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                Logger.LogInformation(Rs.Msg);
                if (Rs.JsonData != null)
                {
                    dtCheckHeader = BaseShared.JsonToDataSet(Rs.JsonData).Tables[0];
                    dsCheckHeader = BaseShared.JsonToDataSet(Rs.JsonData);
                }
            }
        }

        async Task ShowBusyDialog(DataTable dt)
        {
            InvokeAsync(async () =>
            {
                // Simulate background task

                await RunProgress(dt);

                // Close the dialog
                dialogService.Close();

                //await RunProgress();
                //await ShowBusyDialogProgress();
            });

            await BusyDialog("Loading...");
        }

        async Task RunProgress(DataTable dt)
        {
            //OpenModal();
            //contracts = new List<Contract_Info>();
            IsLoading = true;

            //for (int i = 0; i <= 50; i++)
            //{
            //    currentPregress = i * 2;
            //    //currentPregress = i * 10;
            //    await Task.Delay(100);
            //    StateHasChanged();
            //}

            completionMessage = "กำลัง Format Data";
            StateHasChanged();

            dt = BaseShared.FData(dt);

            List<ImpBHCheck> TmpData = new List<ImpBHCheck>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                currentPregress = (i + 1) * 100 / dt.Rows.Count;
                StatusRow = $"{(i + 1)} / {dt.Rows.Count}";
                try
                {
                    ImpBHCheck C = new ImpBHCheck();
                    if (!string.IsNullOrEmpty(dt.Rows[i]["ReceiptCode"].ToString().Trim()))
                    {
                        C.ReceiptCode = dt.Rows[i]["ReceiptCode"].ToString().Trim();
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["ช่องทาง"].ToString().Trim()))
                    {
                        if (dt.Rows[i]["ช่องทาง"].ToString().Trim().Contains("-"))
                        {
                            List<string> payplace = dt.Rows[i]["ช่องทาง"].ToString().Trim().Split('-').ToList();
                            if (payplace.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(payplace[0].Trim()))
                                {
                                    C.PayPlace = Convert.ToInt32(payplace[0].Trim()).ToString("00000");
                                }
                            }
                        }
                        else
                        {
                            C.PayPlace = Convert.ToInt32(dt.Rows[i]["ช่องทาง"].ToString().Trim()).ToString("00000");
                        }
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["วันที่โอนเงิน/ส่งเงิน"].ToString().Trim()))
                    {
                        C.TransDate = Convert.ToDateTime(dt.Rows[i]["วันที่โอนเงิน/ส่งเงิน"].ToString().Trim());
                        if (C.TransDate.Value.Year > 2500)
                        {
                            C.TransDate = C.TransDate.Value.AddYears(-543);
                        }
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["รายการ"].ToString().Trim()))
                    {
                        if (dt.Rows[i]["ช่องทาง"].ToString().Trim().Contains("-"))
                        {
                            List<string> payplace = dt.Rows[i]["รายการ"].ToString().Trim().Split('-').ToList();
                            if (payplace.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(payplace[0].Trim()))
                                {
                                    C.BillRecvID = Convert.ToInt32(payplace[0].Trim());
                                }
                            }
                        }
                        else
                        {
                            C.BillRecvID = Convert.ToInt32(dt.Rows[i]["รายการ"].ToString().Trim());
                        }
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["หมายเหตุ"].ToString().Trim()))
                    {
                        C.Remark = dt.Rows[i]["หมายเหตุ"].ToString().Trim();
                    }

                    TmpData.Add(C);

                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    var alert = dialogService.Alert(ex.Message, "Format Data Error", new AlertOptions() { OkButtonText = "OK" });
                    break;
                }
            }

            CmsData = TmpData;

            IsLoading = false;
            //CloseModal();
            StateHasChanged();

            //Logger.LogInformation(JsonConvert.SerializeObject(contracts));
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

        async Task CompleteUpload(UploadCompleteEventArgs args)
        {
            if (!args.Cancelled)
            {
                IsSaveDisable = true;
                CmsData = new List<ImpBHCheck>();
                //DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(args.RawResponse);
                //Logger.LogInformation(args.JsonResponse.ToString());
                //Logger.LogInformation(args.RawResponse);

                //var bytes = Convert.FromBase64String(args.RawResponse);
                //using (var stream = new MemoryStream(bytes))
                //{
                //    using (var reader = ExcelReaderFactory.CreateReader(stream))
                //    {
                //        var result = reader.AsDataSet(
                //            new ExcelDataSetConfiguration()
                //            {
                //                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                //                {
                //                    UseHeaderRow = true
                //                }
                //            }
                //            );
                //        DataTable dt = result.Tables[0];
                //        Logger.LogInformation(BaseShared.DataTableToJson(dt));
                //    }
                //}

                //await ShowBusyDialog();
                ExecResult Rs = new ExecResult();
                Rs = Newtonsoft.Json.JsonConvert.DeserializeObject<ExecResult>(args.RawResponse);
                if (Rs != null)
                {
                    if (Rs.JsonData != null)
                    {
                        //DataSet ds = new DataSet();
                        //ds.ReadXml(Rs.JsonData, XmlReadMode.IgnoreSchema);
                        //DataTable dt = ds.Tables[0];
                        DataTable dt = new DataTable();
                        dt = BaseShared.JsonToDataTable(Rs.JsonData);

                        if (BaseShared.CheckHeaderData(dt, dtCheckHeader))
                        {
                            await RunProgress(dt);
                            completionMessage = "Upload Complete!";
                            tmpRpts = CmsData;
                            if (tmpRpts.Count > 0)
                            {
                                IsData = true;
                                IsSaveDisable = false;
                            }
                            else
                            {
                                IsData = false;
                            }
                            StateHasChanged();
                        }
                        else
                        {
                            completionMessage = "Upload Cancelled! : ไฟล์ Excel ไม่ตรง";
                            NotificationService.Notify(NotificationSeverity.Error, "Error", $"ไฟล์ Excel ไม่ตรง");
                        }
                        //Logger.LogInformation(BaseShared.DataTableToJson(dt));
                        //await ShowBusyDialog(dt);
                    }
                }
            }
            else
            {
                completionMessage = "Upload Cancelled!";
            }

            showProgress = false;
            showComplete = true;
        }

        void TrackProgress(UploadProgressArgs args)
        {
            showProgress = true;
            showComplete = false;
            progress = args.Progress;

            // cancel upload
            args.Cancel = cancelUpload;

            // reset cancel flag
            cancelUpload = false;
            if (args.Progress == 100)
            {
                foreach (var file in args.Files)
                {
                    //NotificationService.Notify(NotificationSeverity.Info, "Info", $"Uploaded: {file.Name} / {file.Size} bytes");
                    System.IO.FileInfo existingFile = new System.IO.FileInfo(file.Name);
                    if (existingFile.Extension.ToLower().Trim() == ".xlsx")
                    {
                        //using (var stream = File.Open(file.Name, FileMode.Open, FileAccess.Read))
                        //{
                        //    using (var reader = ExcelReaderFactory.CreateReader(stream))
                        //    {
                        //        var result = reader.AsDataSet(
                        //            new ExcelDataSetConfiguration()
                        //            {
                        //                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        //                {
                        //                    UseHeaderRow = true
                        //                }
                        //            }
                        //            );
                        //        DataTable dt = result.Tables[0];
                        //        Logger.LogInformation(BaseShared.DataTableToJson(dt));
                        //    }
                        //}
                    }
                    else
                    {
                        NotificationService.Notify(NotificationSeverity.Error, "Error", $"ไฟล์ Excel(.xlsx) เท่านั้น");
                    }
                }
            }
        }

        void CancelUpload()
        {
            cancelUpload = true;
        }

        private async void UploadError(UploadErrorEventArgs args)
        {
            cancelUpload = true;
            await dialogService.Alert(args.Message, "File Upload Error", new AlertOptions() { OkButtonText = "Ok" });
        }

        async Task OnChange(InputFileChangeEventArgs e)
        {
            //NotificationService.Notify(NotificationSeverity.Info, "Info", e.File.Name);
        }

        void OnComplete(UploadCompleteEventArgs args)
        {
            //NotificationService.Notify(NotificationSeverity.Info, "Info", $"Server response: {args.RawResponse}");
        }

        async Task FileExamDownload()
        {
            IsLoading = true;

            if (dtCheckHeader.Columns.Count > 0)
            {
                DataTable dt = dtCheckHeader;
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        using (ExcelPackage pck = new ExcelPackage())
                        {
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                            ws.Cells["A1"].LoadFromDataTable(dt, true);
                            ws = BaseShared.FormatExccel(ws, dt);
                            var ms = new System.IO.MemoryStream();
                            pck.SaveAs(ms);
                            await jsRuntime.SaveAs("FileExamImportBHCheck.xlsx", pck.GetAsByteArray());
                        }
                    }
                }
            }

            IsLoading = false;
        }

        async Task ShowBusyDialogProgress()
        {
            var confirmationResult = await this.dialogService.Confirm($"บันทึก ข้อมูล หรือไม่?"
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

                    IsSaveDisable = true;

                    if (!is_success)
                    {
                        NotificationService.Notify(NotificationSeverity.Error, "Error", sql_msg, 5000);
                    }
                    else
                    {
                        completionMessage = "Save Complete!";
                        NotificationService.Notify(NotificationSeverity.Success, "Success", sql_msg);
                    }
                    IsLoading = false;
                    StateHasChanged();
                });

                await BusyDialog("กำลังบันทึกข้อมูล...");
            }
            IsLoading = false;
            StateHasChanged();
        }

        bool is_success = false;
        string sql_msg = "";
        async Task RunSaveProgress()
        {
            //OpenModal();
            //contracts = new List<Contract_Info>();
            IsLoading = true;

            completionMessage = "Saving!";

            //for (int i = 0; i <= 50; i++)
            //{
            //    currentPregress = i * 2;
            //    //currentPregress = i * 10;
            //    await Task.Delay(100);
            //    StateHasChanged();
            //}

            for (int i = 0; i < CmsData.Count; i++)
            {
                currentPregress = (i + 1) * 100 / CmsData.Count;
                StatusRow = $"{(i + 1)} / {CmsData.Count}";

                ImpBHCheck C = CmsData[i];
                C.CheckBy = userData.UserID;

                if (string.IsNullOrEmpty(C.ResultMsg.Trim()))
                {

                    var response = await Http.PostAsJsonAsync("FileExam/ImpBHCheckSave", C);

                    ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                    if (Rs != null)
                    {
                        C.Result = Rs.IsSuccess;
                        C.ResultMsg = Rs.Msg;

                        if (!Rs.IsSuccess)
                        {
                            Logger.LogInformation(Rs.Msg);
                        }
                        is_success = Rs.IsSuccess;
                        sql_msg = Rs.Msg;
                    }
                }

                //await Task.Delay(200);

                StateHasChanged();
            }

            IsLoading = false;
            IsSaveDisable = true;

            //CloseModal();
            StateHasChanged();

            //Logger.LogInformation(JsonConvert.SerializeObject(contracts));
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

        async Task RunExpoerExcel()
        {
            if (CmsData.Count > 0)
            {
                Type fieldsType = typeof(Cms_Card_Trans);
                PropertyInfo[] props = fieldsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                DataTable dt = new DataTable();
                foreach (PropertyInfo prop in props)
                {
                    dt.Columns.Add(prop.Name);
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                    ws.Cells["A1"].LoadFromCollection<ImpBHCheck>(CmsData, true);
                    ws = BaseShared.FormatExccel(ws, dt);
                    var ms = new System.IO.MemoryStream();
                    pck.SaveAs(ms);
                    await jsRuntime.SaveAs("ImportBHCheck.xlsx", pck.GetAsByteArray());
                }
            }
        }
        List<ImpBHCheck> tmpRpts = new List<ImpBHCheck>();
        void OnTabChange(int index)
        {

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

        List<ImpBHCheck> GetTmpDataByTab(int idex)
        {
            List<ImpBHCheck> tmpRpts = new List<ImpBHCheck>();
            switch (idex)
            {
                case 1:
                    {
                        tmpRpts = CmsData.FindAll(x => x.Result == false && !string.IsNullOrEmpty(x.ResultMsg));
                    }
                    break;
                default:
                    {
                        tmpRpts = CmsData;
                    }
                    break;
            }
            return tmpRpts;
        }
    }
}
