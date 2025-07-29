using ChainConnext.Shared;
using Microsoft.AspNetCore.Components;
using static System.Net.WebRequestMethods;
using System;
using ChainConnext.Shared.Contracts;
using ChainConnext.Shared.BD;
using System.Net.Http.Json;
using ChainConnext.Client.Services;
using ChainConnext.Shared.Authen;
using Radzen;

namespace ChainConnext.Client.Pages
{
    public partial class ContractNew
    {
        [Parameter]
        public string? ContractId { get; set; }
        [Parameter]
        public string? Key { get; set; }

        Contract_Info ConInf = new Contract_Info();
        List<ProModel> PmdData = new List<ProModel>();

        bool isloaddata = false;
        bool IsLoadRefNo = false;

        Authens userData = new Authens();

        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;

        protected override async Task OnInitializedAsync()
        {
            await CheckPermission();
            isloaddata = true;
            await GetProModelData();
            isloaddata = false;
            StateHasChanged();
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
                var menu = userData.MenuList.Find(x => x.MenuId == 22);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }

        DateTime? value = DateTime.Now;
        bool isLoading = false;
        async Task SubmitAsync(Contract_Info arg)
        {
            ExecResult Rs = new ExecResult();
            Rs.Data = arg;

            ExecResult? MRs = new ExecResult(); //await Mresponse.Content.ReadFromJsonAsync<ExecResult>();
            if (MRs != null)
            {
                Rs.IsSuccess = MRs.IsSuccess;
                Rs.Msg = MRs.Msg;
                dialogService.Close(Rs);
            }
        }

        private async Task GetProModelData()
        {
            if (ShareValues.PmdData.Count > 0)
            {
                PmdData = ShareValues.PmdData;
            }
            else
            {
                var postBody = new ProModel();
                var response = await Http.PostAsJsonAsync("BD/ListProModel", postBody);

                ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                if (Rs != null)
                {
                    if (Rs.Rows > 0)
                    {
                        PmdData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProModel>>(Rs.Data.ToString());
                        if (PmdData != null)
                        {
                            ShareValues.PmdData = PmdData;
                        }
                    }
                }
            }
        }

        async Task FindRefNo(string key)
        {
            IsLoadRefNo = true;

            var postBody = new Contract_Info();
            switch (key)
            {
                case "RefNo":
                    {
                        postBody.RefNo = ConInf.RefNo;
                    }
                    break;
                case "ContNo":
                    {
                        postBody.ContractNo = ConInf.ContractNo;
                    }
                    break;
            }
            postBody.UserData = userData;
            postBody.CreatedBy = userData.UserID;
            var response = await Http.PostAsJsonAsync("Contract/FindSaveNewData", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                Logger.LogInformation(Rs.Msg);
                
                if (Rs.Rows > 0)
                {
                    Logger.LogInformation(Rs.Data.ToString());
                    ConInf = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contract_Info_New>>(Rs.Data.ToString()).FirstOrDefault();
                }
                if (Rs.IsSuccess)
                {
                    NotificationService.Notify(NotificationSeverity.Success, "Success", Rs.Msg);
                }
                else
                {
                    ConInf = new Contract_Info();
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }

            IsLoadRefNo = false;
        }
    }
}
