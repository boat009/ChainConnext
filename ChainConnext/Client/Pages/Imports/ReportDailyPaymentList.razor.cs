using ChainConnext.Client.Pages.Contracts;
using ChainConnext.Client.Pages.Payments;
using ChainConnext.Client.Services;
using ChainConnext.Shared;
using ChainConnext.Shared.Reports;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChainConnext.Client.Pages.Imports
{
    public partial class ReportDailyPaymentList
    {
        [Parameter]
        public List<Tmp_ReportDaily_Payment> TmpRpt { get; set; } = new List<Tmp_ReportDaily_Payment>();

        [Parameter]
        public List<string> TmpRptHeadData { get; set; } = new List<string>();
        [Parameter]
        public int Height { get; set; }
        [Parameter]
        public int Width { get; set; }

        bool IsLoading = false;
        IList<Tmp_ReportDaily_Payment>? selectedTmpRpt;

        //[Parameter]
        //public EventCallback<Tmp_ReportDaily_Payment> OnDoInfo { get; set; }
        [Parameter]
        public EventCallback<Tmp_ReportDaily_Payment> OnDoSave { get; set; }
        [Parameter]
        public EventCallback<Tmp_ReportDaily_Payment> OnDoDelete { get; set; }

        async Task ViewData(Tmp_ReportDaily_Payment daTa, string name)
        {
            //ExecResult Rs = new ExecResult();
            //Rs = await dialogService.OpenAsync<PaymentEditor>($"{daTa.RefNo} - {daTa.ContNo} - {daTa.CustName}",
            //      new Dictionary<string, object>() { { "pRefNo", daTa.RefNo } },
            //      new DialogOptions() { Width = "1300px", Height = "800px" });
            //if (Rs != null)
            //{
            //    if (Rs.IsSuccess)
            //    {

            //    }
            //}
            await jsRuntime.InvokeVoidAsync("open", $"payment/{daTa.RefNo}", "_blank");
        }

        async Task OnCellContextMenu(DataGridCellMouseEventArgs<Tmp_ReportDaily_Payment> args)
        {
            selectedTmpRpt = new List<Tmp_ReportDaily_Payment>() { args.Data };

            Tmp_ReportDaily_Payment tmp = selectedTmpRpt.FirstOrDefault();

            if (tmp.CanSave)
            {
                ContextMenuService.Open(args,
                    new List<ContextMenuItem> {
                new ContextMenuItem(){ Text = "ดูข้อมูล", Value = 1, Icon = "info" },
                new ContextMenuItem(){ Text = "บันทึกข้อมูล", Value = 2, Icon = "save" },
                new ContextMenuItem(){ Text = "ยกเลิกใบเสร็จ", Value = 3, Icon = "delete" },
                    },
                async (e) =>
                {
                    //console.Log($"Menu item with Value={e.Value} clicked. Column: {args.Column.Property}, EmployeeID: {args.Data.EmployeeID}");
                    switch (e.Value)
                    {
                        case 1:
                            {
                                //await OnDoInfo.InvokeAsync(tmp);
                                await ViewData(tmp, "View From ContextMenu");
                            }
                            break;
                        case 2:
                            {
                                await OnDoSave.InvokeAsync(tmp);
                            }
                            break;
                        case 3:
                            {
                                await OnDoDelete.InvokeAsync(tmp);
                            }
                            break;
                    }
                }
                 );
            }
            else
            {
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
                                //await OnDoInfo.InvokeAsync(tmp);
                                await ViewData(tmp, "View From ContextMenu");
                            }
                            break;
                        case 2:
                            {
                                await OnDoSave.InvokeAsync(tmp);
                            }
                            break;
                        case 3:
                            {
                                await OnDoDelete.InvokeAsync(tmp);
                            }
                            break;
                    }
                }
                 );
            }
        }
    }
}
