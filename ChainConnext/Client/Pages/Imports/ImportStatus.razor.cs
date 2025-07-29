using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.Contracts;
using ChainConnext.Client.Services;
using Microsoft.JSInterop;
using Radzen;
using Microsoft.AspNetCore.Components.Forms;
using System;
using ChainConnext.Shared.Reports;
using ChainConnext.Shared;
using System.Net.Http.Json;
using System.Data;
using OfficeOpenXml;
using System.Diagnostics.Contracts;
using ExcelDataReader;
using System.IO;
using Newtonsoft.Json;
using ChainConnext.Shared.Imps;
using ChainConnext.Shared.BD;
using System.Reflection;

namespace ChainConnext.Client.Pages.Imports
{
    public partial class ImportStatus
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        List<ImpStatus> ContData = new List<ImpStatus>();
        IList<ImpStatus>? selectedData;

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

        DataTable dtCheckHeader = new DataTable();

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
                var menu = userData.MenuList.Find(x => x.MenuId == 24);
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
            var response = await Http.PostAsJsonAsync("FileExam/ImpStatus", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                Logger.LogInformation(Rs.Msg);
                if (Rs.JsonData != null)
                {
                    dtCheckHeader = BaseShared.JsonToDataTable(Rs.JsonData);
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

            completionMessage = "กำลัง แปลงข้อมูลวันที่";
            StateHasChanged();

            List<ImpStatus> TmpData = new List<ImpStatus>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                currentPregress = (i + 1) * 100 / dt.Rows.Count;
                StatusRow = $"{(i + 1)} / {dt.Rows.Count}";

                if (!string.IsNullOrEmpty(dt.Rows[i]["วันที่มีผล"].ToString().Trim()))
                {
                    dt.Rows[i]["วันที่มีผล"] = BaseShared.ConvertFromOADate(dt.Rows[i]["วันที่มีผล"].ToString().Trim());
                }

                ImpStatus C = new ImpStatus();
                C.ContractNo = dt.Rows[i]["เลขสัญญา"].ToString().Trim();
                C.ContractStatus = dt.Rows[i]["สถานะที่เปลี่ยน"].ToString().Trim();
                if (!string.IsNullOrEmpty(dt.Rows[i]["วันที่มีผล"].ToString().Trim()))
                {
                    C.ContractStatusDate = Convert.ToDateTime(dt.Rows[i]["วันที่มีผล"].ToString().Trim());
                }

                var postBody = new ImpStatus { ContractNo = C.ContractNo,UserData = userData };
                var response = await Http.PostAsJsonAsync("Contract/PreContractStatusContractNo", postBody);
                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.IsSuccess)
                    {
                        if (Rs.Data != null)
                        {
                            var curr = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ImpStatus>>(Rs.Data.ToString());
                            if (curr != null)
                            {
                                if (curr.Count > 0)
                                {
                                    C.CurrContractStatus = curr.FirstOrDefault().ContractStatus;
                                    C.CurrContractStatusDate = curr.FirstOrDefault().ContractStatusDate;
                                }
                            }
                        }
                    }
                }

                TmpData.Add(C);

                StateHasChanged();
            }

            ContData = TmpData;

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
                ContData = new List<ImpStatus>();
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
                            if (ContData.Count > 0)
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
                            await jsRuntime.SaveAs("FileExamImportStatus.xlsx", pck.GetAsByteArray());
                        }
                    }
                }
            }

            IsLoading = false;
        }

        async Task ShowBusyDialogProgress()
        {
            var confirmationResult = await this.dialogService.Confirm($"บันทึก ข้อมูลสถานะ หรือไม่?"
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
                    NotificationService.Notify(NotificationSeverity.Error, "Error", sql_msg);
                }
                else
                {
                    completionMessage = "Save Complete!";
                    NotificationService.Notify(NotificationSeverity.Success, "Success", sql_msg);
                }

                StateHasChanged();
            });

                await BusyDialog("กำลังบันทึกข้อมูล...");
            }
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

            for (int i = 0; i < ContData.Count; i++)
            {
                currentPregress = (i + 1) * 100 / ContData.Count;
                StatusRow = $"{(i + 1)} / {ContData.Count}";

                ImpStatus C = ContData[i];

                var postBody = new Contract_Info 
                { 
                    ContractNo = C.ContractNo,
                    ContractStatus = C.ContractStatus,
                    ContractStatusDate = C.ContractStatusDate,
                    CreatedBy = userData.UserID,
                    UserData = userData
                };
                var response = await Http.PostAsJsonAsync("Contract/UpdateContractStatusContractNo", postBody);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (!Rs.IsSuccess)
                    {
                        Logger.LogInformation(Rs.Msg);
                        break;
                    }
                    is_success = Rs.IsSuccess;
                    sql_msg = Rs.Msg;
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
            if (ContData.Count > 0)
            {
                Type fieldsType = typeof(ImpStatus);
                PropertyInfo[] props = fieldsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                DataTable dt = new DataTable();
                foreach (PropertyInfo prop in props)
                {
                    dt.Columns.Add(prop.Name);
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                    ws.Cells["A1"].LoadFromCollection<ImpStatus>(ContData, true);
                    ws = BaseShared.FormatExccel(ws, dt);
                    var ms = new System.IO.MemoryStream();
                    pck.SaveAs(ms);
                    await jsRuntime.SaveAs("ImportStatus.xlsx", pck.GetAsByteArray());
                }
            }
        }
    }
}
