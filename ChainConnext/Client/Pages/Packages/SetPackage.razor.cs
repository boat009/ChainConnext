using ChainConnext.Client.Pages.Settings;
using ChainConnext.Client.Services;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Json;
using ChainConnext.Shared.Packages;

namespace ChainConnext.Client.Pages.Packages
{
    public partial class SetPackage
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        bool IsMainLoading = false;
        bool IsDetailLoading = false;
        List<Package_Main> mainData = new List<Package_Main>();
        List<Package_Main_Detail> detailData = new List<Package_Main_Detail>();

        IList<Package_Main>? selectedMain;
        IList<Package_Main_Detail>? selectedDetail;

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();

        bool IsMainSelected = false;

        int Height;
        int Width;

        bool IsLoading = false;

        BDProModel pmdTmp = new BDProModel();

        protected override async Task OnParametersSetAsync()
        {

            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            await CheckPermission();

            if (IsAccess)
            {
                await GetMainData();
            }

        }

        async Task CheckPermission()
        {
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (userData.MenuList.Count > 0)
            {
                var path = Navigation.Uri.Replace(Navigation.BaseUri, "");
                var menu = userData.MenuList.Find(x => x.MenuId == 84);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        private async Task<List<Package_Main_Detail>> GetProModelData(string packageId)
        {
            IsDetailLoading = true;

            List<Package_Main_Detail> data = new List<Package_Main_Detail>();

            var postBody = new Package_Main_Detail { PackageId = packageId };
            var response = await Http.PostAsJsonAsync("Package/DetailList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Package_Main_Detail>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
            IsDetailLoading = false;
            return data;
        }

        private async Task GetMainData()
        {
            IsMainLoading = true;
            var postBody = new Package_Main();
            var response = await Http.PostAsJsonAsync("Package/MainList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        mainData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Package_Main>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
            IsMainLoading = false;
        }

        async Task OnDetailCellClick(DataGridCellMouseEventArgs<Package_Main_Detail> args)
        {
            
        }
        async Task OnMainCellClick(DataGridCellMouseEventArgs<Package_Main> args)
        {
            IsMainSelected = true;
        }

        async Task OpenDetailEdit(Package_Main_Detail? daTa, string key)
        {
            if (daTa == null)
            {
                daTa = new Package_Main_Detail();
            }
        }
        async Task OpenDetailDelete(Package_Main_Detail daTa, string key)
        {
            
        }
        async Task OpenMainEdit(Package_Main? daTa, string key)
        {
            if (daTa == null)
            {
                daTa = new Package_Main();
            }

            ExecResult Rs = await dialogService.OpenAsync<SetPackageDialog>($"{key}",
                  new Dictionary<string, object>() { { "packageId", daTa.PackageId }, { "Key", key } },
                  new DialogOptions() { Width = "1000px", Height = "600px" });

            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    
                }
            }
        }
        async Task OpenMainDelete(Package_Main daTa, string key)
        {
            
        }

        async Task OnCellContextMenu(DataGridCellMouseEventArgs<Package_Main> args)
        {
            ContextMenuService.Open(args,
                    new List<ContextMenuItem> {
                new ContextMenuItem(){ Text = "แก้ไข", Value = 1, Icon = "edit" },
                new ContextMenuItem(){ Text = "ลบ", Value = 2, Icon = "delete" },
                    },
                async (e) =>
                {
                    switch (e.Value)
                    {
                        case 1:
                            {
                                await OpenMainEdit(args.Data, "Edit");
                            }
                            break;
                        case 2:
                            {
                                await OpenMainDelete(args.Data, "Delete");
                            }
                            break;
                    }
                }
                 );
        }

        async Task ExportToExcel()
        {
            IsLoading = true;

            //List<BDProModel> data = new List<BDProModel>();
            //data = await GetProModelData("");

            //if (data.Count > 0)
            //{
            //    Type fieldsType = typeof(BDProModel);
            //    PropertyInfo[] props = fieldsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //    DataTable dt = new DataTable();
            //    foreach (PropertyInfo prop in props)
            //    {
            //        dt.Columns.Add(prop.Name);
            //    }

            //    using (ExcelPackage pck = new ExcelPackage())
            //    {
            //        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
            //        ws.Cells["A1"].LoadFromCollection<BDProModel>(data, true);
            //        ws = BaseShared.FormatExccel(ws, dt);
            //        var ms = new System.IO.MemoryStream();
            //        pck.SaveAs(ms);
            //        await jsRuntime.SaveAs("ProModel.xlsx", pck.GetAsByteArray());
            //    }
            //}
            IsLoading = false;
        }
    }
}
