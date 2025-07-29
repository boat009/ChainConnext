using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Authen;
using ChainConnext.Client.Services;
using Microsoft.JSInterop;
using System.Buffers;
using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using System.Net.Http.Json;
using Radzen;
using ChainConnext.Shared.ARs;
using OfficeOpenXml;
using System.Data;
using FastMember;
using System.Threading.Channels;

namespace ChainConnext.Client.Pages.ARs
{
    public partial class ARCalTest
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        int selectedIndex = 0;
        int selectedIndexTopCal = 0;
        int selectedIndexTop = 0;
        int selectedIndexChg = 0;
        int selectedIndexBill = 0;

        List<FindMode> findModes = new List<FindMode>();
        int findSelected = 1;
        bool IsLoad = false;

        int Height;
        int Width;

        Authens userData = new Authens();
        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        string SearchValue = "";

        BD_Debtora Dba = new BD_Debtora();
        List<BD_Debtora> Dbas = new List<BD_Debtora>();
        List<IRR_Contract_Cal> iRRs = new List<IRR_Contract_Cal>();
        IRR_Contract_Cal iRR = new IRR_Contract_Cal();
        List<IRR_Contract_Cal_Detail> iRRDetails = new List<IRR_Contract_Cal_Detail>();

        protected override async Task OnInitializedAsync()
        {
            IsLoad = true;

            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            await CheckPermission();

            if (IsAccess)
            {
                await GetFindModeData();

                if (Navigation.BaseUri.Contains("localhost"))
                {
                    SearchValue = "640109714";
                }
            }

            IsLoad = false;
        }

        private async Task GetFindModeData()
        {
            var postBody = new FindMode();
            var response = await Http.PostAsJsonAsync("Find/ListFindMode", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Rows > 0)
                {
                    findModes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FindMode>>(Rs.Data.ToString());
                }
            }
        }

        async Task CheckPermission()
        {
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (userData.MenuList.Count > 0)
            {
                var menu = userData.MenuList.Find(x => x.MenuId == 86);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsSave;
                }
            }
        }

        async Task OnChange(string value, string name)
        {
            //Logger.LogInformation($"{name} value changed to {value}");
            switch (name)
            {
                case "Find":
                    {
                        if (!string.IsNullOrEmpty(value.Trim()))
                        {
                            int len = 8;
                            switch (findSelected)
                            {
                                case 1: len = 9; break;
                                case 2: len = 8; break;
                            }
                            SearchValue = BaseShared.FillRefNo(value.Trim(), len).ToUpper();
                            await FindData();
                        }
                    }
                    break;
            }
            StateHasChanged();
        }

        async Task Find(BD_Debtora? daTa, string key)
        {
            await FindData();
        }

        async Task FindTo(string? serch)
        {
            if (serch != null)
            {
                findSelected = 1;
                SearchValue = serch;
                await FindData();
            }
        }

        async Task FindData()
        {
            IsLoad = true;

            Dba = new BD_Debtora();
            bool is_found = false;

            var postBody = new FindMode { FindID = findSelected, FindValue = SearchValue, FindBy = userData.UserID, UserData = userData };
            var response = await Http.PostAsJsonAsync("BD/FindDebtora", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    Dba = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BD_Debtora>>(Rs.Data.ToString()).FirstOrDefault();

                    if (Dba != null)
                    {
                        if (Dba.id > 0)
                        {
                            is_found = true;
                        }
                    }

                }
                if (Rs.IsSuccess)
                {
                    //NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }

            if (!is_found)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "ไม่พบข้อมูล");
            }

            IsLoad = false;

            await CalIRR();
        }

        async Task CalIRR()
        {
            IsLoad = true;
            if (Dba.credit > 0)
            {
                var postBody = new IRR_Contract_Cal 
                {
                    Credit = Convert.ToDecimal(Dba.credit),
                    Sales = Convert.ToDecimal(Dba.sales),
                    Peroid = !string.IsNullOrEmpty(Dba.mode)?Convert.ToInt32(Dba.mode) :0,
                    PeroidAmt = Convert.ToDecimal(Dba.premium),
                    FirstPeroidAmt = Convert.ToDecimal(Dba.premium2),
                    NetCredit = Convert.ToDecimal(Dba.netcredit),
                    Discount = Convert.ToDecimal(Dba.disc)
                };
                var response = await Http.PostAsJsonAsync("AR/IRRCal", postBody);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.Rows > 0)
                    {
                        iRRs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IRR_Contract_Cal>>(Rs.Data.ToString());
                        iRR = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IRR_Contract_Cal>>(Rs.Data.ToString()).FirstOrDefault();
                    }
                }

                response = await Http.PostAsJsonAsync("AR/IRRCalDetail", postBody);
                Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.Rows > 0)
                    {
                        iRRDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IRR_Contract_Cal_Detail>>(Rs.Data.ToString());
                    }
                }
            }
            IsLoad = false;
        }

        async Task ExportExcel()
        {
            IsLoad = true;

            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            using (var reader = ObjectReader.Create(iRRs))
            {
                dt.Load(reader);
            }
            ds.Tables.Add(dt);
            dt = new DataTable();
            using (var reader = ObjectReader.Create(iRRDetails))
            {
                dt.Load(reader);
            }
            ds.Tables.Add(dt);

            ds.Tables[0].TableName = "ตั้งหนี้";
            ds.Tables[1].TableName = "รายงวด";

            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(ds.Tables[i].TableName);
                            ws.Cells["A1"].LoadFromDataTable(ds.Tables[i], true);
                            ws = BaseShared.FormatExccel(ws, dt);
                        }
                        var ms = new System.IO.MemoryStream();
                        pck.SaveAs(ms);
                        await jsRuntime.SaveAs("TestCalIRR.xlsx", pck.GetAsByteArray());
                    }
                }
            }

            IsLoad = false;
        }
    }
}
