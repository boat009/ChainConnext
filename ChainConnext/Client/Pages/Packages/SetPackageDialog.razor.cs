using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Packages;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ChainConnext.Shared;
using Microsoft.JSInterop;

namespace ChainConnext.Client.Pages.Packages
{
    public partial class SetPackageDialog
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        [Parameter]
        public string? packageId { get; set; }
        [Parameter]
        public string? Key { get; set; }

        Package_Main Pm = new Package_Main();
        bool IsLoading = false;

        async Task OnFindEnter(string value)
        {
            Pm.ItemCode = value;

            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = "Enter Notify",
                Detail = $"{Pm.ItemCode}",
                Duration = 5000
            });

            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Info,
                Summary = "Enter Notify",
                Detail = $"{Pm.PackageDesc}",
                Duration = 5000
            });
            await OnChangeFocus(value, "PackageDesc");
        }

        async Task OnChangeFocus(string value, string name)
        {
            //if (name == "ModelName")
            //{
            //    await GetProModelByID(value);
            //}
            if (!string.IsNullOrEmpty(name.Trim()))
            {
                await jsRuntime.InvokeVoidAsync("focusInput", name);
                StateHasChanged();
            }
        }
    }
}
