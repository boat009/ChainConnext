using BlazorStrap.V5;
using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared.Contracts;
using ChainConnext.Shared.TSRApps;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using System;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ChainConnext.Client.Pages.Settings
{
    public partial class SetCreditAreaDialog
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        [Parameter]
        public int ID { get; set; }
        [Parameter]
        public string? Key { get; set; }
        CArea cArea = new CArea();
        List<CArea> deps = new List<CArea>();

        bool IsLoading = false;

        string UserID = "";
        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            await CheckPermission();

            await ListCAreaDepData();

            await ListCAreaData();

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
                var menu = userData.MenuList.Find(x => x.MenuId == 16);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        private async Task ListCAreaDepData()
        {
            var postBody = new CArea();
            var response = await Http.PostAsJsonAsync("BD/ListCAreaDep", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        deps = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CArea>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
        }

        private async Task ListCAreaData()
        {
            if (ID == 0)
            {
                return;
            }
            var postBody = new CArea { ID = ID };
            var response = await Http.PostAsJsonAsync("BD/ListCArea", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        cArea = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CArea>>(Rs.Data.ToString()).FirstOrDefault();
                    }
                }
                else
                {
                    Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
        }

        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            //NotificationService.Notify(NotificationSeverity.Error, "Error", $"InvalidSubmit: {JsonSerializer.Serialize(args, new JsonSerializerOptions() { WriteIndented = true })}");
        }

        async Task OnSubmit(CArea model)
        {
            model.CreateBy = userData.UserID;
            var response = await Http.PostAsJsonAsync("BD/SaveCArea", model);
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
        }

        async Task FindEmp()
        {
            var postBody = new TSRApp_EmpData { empid = cArea.EmpId };
            var response = await Http.PostAsJsonAsync("TSRApp/FindEmp", postBody);
            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Data != null)
                {
                    TSRApp_EmpData emp = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TSRApp_EmpData>>(Rs.Data.ToString()).FirstOrDefault();
                    cArea.Name = emp.empname;
                }
            }
        }
    }
}
