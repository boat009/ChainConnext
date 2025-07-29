using ChainConnext.Shared;
using ChainConnext.Shared.BD;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages
{
    public partial class EditorLogDialog
    {
        [Parameter]
        public string? fromValue { get; set; }
        [Parameter]
        public string? toValue { get; set; }
        [Parameter]
        public string? editBy { get; set; }
        [Parameter]
        public string? contractId { get; set; }
        [Parameter]
        public string? contNo { get; set; }
        [Parameter]
        public string? refNo { get; set; }

        Editor_Log editor = new Editor_Log();

        bool IsLoading = false;

        protected override void OnParametersSet()
        {
            editor.FromValue = fromValue;
            editor.ToValue = toValue;
            editor.EditBy = editBy;
            editor.ContractId = contractId;
            editor.ContNo = contNo;
            editor.RefNo = refNo;

            base.OnParametersSet();
        }

        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Editor Notify",
                Detail = $"กรอก เหตุผล ด้วย",
                Duration = 5000
            });
            //NotificationService.Notify(NotificationSeverity.Error, "Error", $"InvalidSubmit: {JsonSerializer.Serialize(args, new JsonSerializerOptions() { WriteIndented = true })}");
        }

        async Task OnSubmit(Editor_Log model)
        {
            IsLoading = true;

            var response = await Http.PostAsJsonAsync("EditorLog/Save", model);
            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.IsSuccess)
                {
                    dialogService.Close(Rs);
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
            IsLoading = false;
        }
    }
}
