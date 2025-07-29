using ChainConnext.Client.Services;
using ChainConnext.Shared.Authen;
using Microsoft.JSInterop;
using Radzen;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace ChainConnext.Client.Pages.ARs.Reports
{
    public partial class ARReport
    {
        int selectedIndex = 0;
        DateTime? DateFrom;
        DateTime? DateTo;

        bool IsLoad = false;

        int Height;
        int Width;

        Authens userData = new Authens();
        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        protected override async Task OnInitializedAsync()
        {
            IsLoad = true;

            await CheckPermission();

            DateTime dtm = DateTime.Now.AddMonths(-1);

            DateFrom = new DateTime(dtm.Year, dtm.Month, 1);
            DateTo = DateFrom.Value.AddMonths(1).AddDays(-1);

            //NotificationService.Notify(NotificationSeverity.Success, "Success", DateTo.ToString());

            IsLoad = false;
        }

        async Task CheckPermission()
        {
            var dimension = await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
            Height = dimension.Height;
            Width = dimension.Width;

            userData = await _accountService.GetAuthensAsync(Navigation.Uri);
            if (userData.MenuList.Count > 0)
            {
                var menu = userData.MenuList.Find(x => x.MenuId == 74);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsSave;
                }
            }
        }

        DateTime? DateFromChange(DateTime? value)
        {
            if (value != null)
            {
                DateTo = value.Value.AddMonths(1).AddDays(-1);
            }
            return value;
        }
    }
}
