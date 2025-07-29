using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared.Payments;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using Radzen;
using BlazorStrap.V5;
using Radzen.Blazor;

namespace ChainConnext.Client.Pages.Settings
{
    public partial class SetPermission
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        User_Main Um = new User_Main();
        List<User_Main> UmData = new List<User_Main>();
        List<Menu_Main> MenuGroupData = new List<Menu_Main>();
        List<User_Menu> MenuData = new List<User_Menu>();
        List<User_Menu> MainMenuData = new List<User_Menu>();
        List<User_Perms> UmPerms = new List<User_Perms>();
        List<User_Perms> UmMainPerms = new List<User_Perms>();
        List<User_Menu> UserMenuData = new List<User_Menu>();
        IList<User_Menu>? selectedMenu;

        List<User_Menu> MenuSubData = new List<User_Menu>();
        IList<User_Menu>? selectedMenuSub;

        List<User_Menu> MenuSub2Data = new List<User_Menu>();

        RadzenDataGrid<User_Menu> grid;
        bool allowRowSelectOnRowClick = true;

        string MenuGroup = "";

        bool mIsAccess = false;
        bool mIsSave = false;
        bool mIsDelete = false;

        bool mIsAccess2 = false;
        bool mIsSave2 = false;
        bool mIsDelete2 = false;

        bool mIsAccess3 = false;
        bool mIsSave3 = false;
        bool mIsDelete3 = false;

        string UserID = "";
        bool IsSave = false;
        bool IsDelete = false;
        bool IsAccess = true;
        Authens userData = new Authens();

        IEnumerable<string> values;

        bool IsLoading = false;

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            await CheckPermission();

            if (IsAccess)
            {
                await ListUserData();
                await ListMenuGroup();
                await ListMainMenu();
                await ListMainPerms();
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
                var menu = userData.MenuList.Find(x => x.MenuId == 13);
                if (menu != null)
                {
                    IsAccess = menu.IsAccess;
                    IsSave = menu.IsSave;
                    IsDelete = menu.IsDelete;
                }
            }
        }
        bool IsLoadUmData = false;
        private async Task ListUserData()
        {
            IsLoadUmData = true;
            var postBody = new User_Main();
            var response = await Http.PostAsJsonAsync("Authen/ListUser", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    UmData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User_Main>>(Rs.Data.ToString());
                }
            }
            IsLoadUmData = false;
            StateHasChanged();
        }

        private async Task ListMenuGroup()
        {
            var postBody = new Menu_Main();
            var response = await Http.PostAsJsonAsync("Authen/ListMenuGroup", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    MenuGroupData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Menu_Main>>(Rs.Data.ToString());
                }
            }
        }
        private async Task ListMainPerms()
        {
            var postBody = new User_Perms();
            var response = await Http.PostAsJsonAsync("Authen/ListMainPerms", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    UmMainPerms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User_Perms>>(Rs.Data.ToString());
                }
            }
        }
        private async Task ListMainMenu()
        {
            var postBody = new User_Menu();
            var response = await Http.PostAsJsonAsync("Authen/ListMenu", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    MainMenuData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User_Menu>>(Rs.Data.ToString());
                }
            }
        }
        async Task OnUserChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;

            //Logger.LogInformation($"{name} value changed to {str}");
            mIsAccess = false;
            mIsSave = false;
            mIsDelete = false;

            MainMenuData.ForEach(x => x.IsAccess = false);
            MainMenuData.ForEach(x => x.IsSave = false);
            MainMenuData.ForEach(x => x.IsDelete = false);

            StateHasChanged();

            await GetUserData(value.ToString().Trim());
            await GetUserPermsData(value.ToString().Trim());
        }

        private async Task GetUserData(string usrId)
        {
            Um = new User_Main();
            User_Main postBody = new User_Main();
            if (string.IsNullOrEmpty(usrId))
            {

                postBody = new User_Main { UsrID = userData.UserID };
            }
            else
            {
                postBody = new User_Main { UsrID = usrId };
            }

            var response = await Http.PostAsJsonAsync("Authen/ListUser", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    Um = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User_Main>>(Rs.Data.ToString()).FirstOrDefault();
                }
            }
        }
        private async Task GetUserPermsData(string usrId)
        {
            UserMenuData = new List<User_Menu>();
            User_Main postBody = new User_Main();
            if (string.IsNullOrEmpty(usrId))
            {

                postBody = new User_Main { UsrID = userData.UserID };
            }
            else
            {
                postBody = new User_Main { UsrID = usrId };
            }

            var response = await Http.PostAsJsonAsync("Authen/ListUserPerms", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Rows > 0)
                {
                    UmPerms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User_Perms>>(Rs.Data.ToString());
                    List<string> perms = new List<string>();
                    foreach (var item in UmPerms)
                    {
                        if (item.IsPerms)
                        {
                            perms.Add(item.PermsName);
                        }
                    }
                    values = perms;
                }
            }

            var postBodyUser = new User_Menu { UsrID = Um.UsrID };
            var responseUser = await Http.PostAsJsonAsync("Authen/ListUserMenu", postBodyUser);

            ExecResult? RsUser = await responseUser.Content.ReadFromJsonAsync<ExecResult>();
            if (RsUser != null)
            {
                //Logger.LogInformation(RsUser.Msg);
                if (RsUser.Rows > 0)
                {
                    UserMenuData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User_Menu>>(RsUser.Data.ToString());
                }
                if (UserMenuData != null)
                {
                    foreach (var item in UserMenuData)
                    {
                        var menu = MainMenuData.FindAll(x => x.MenuId == item.MenuId);
                        if (menu.Count > 0)
                        {
                            foreach (var mn in menu)
                            {
                                mn.IsAccess = item.IsAccess;
                                mn.IsSave = item.IsSave;
                                mn.IsDelete = item.IsDelete;
                            }
                        }
                    }
                }
            }

            GetMenuData(MenuGroup.Trim());
        }

        void OnMenuGroupChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;

            //Logger.LogInformation($"{name} value changed to {str}");

            mIsAccess = false;
            mIsAccess2 = false;
            mIsAccess3 = false;

            mIsSave = false;
            mIsSave2 = false;
            mIsSave3 = false;

            mIsDelete = false;
            mIsDelete2 = false;
            mIsDelete3 = false;

            GetMenuData(value.ToString().Trim());
        }
        bool isLoading = false;
        void GetMenuData(string menuGroup)
        {
            isLoading = true;

            MenuSubData = new List<User_Menu>();
            MenuSub2Data = new List<User_Menu>();

            MenuData = MainMenuData.FindAll(x=>x.MenuGroup == menuGroup);

            MenuData.ForEach(x => x.UsrID = Um.UsrID);
            MenuData.ForEach(x => x.CreatedBy = userData.UserID);

            //var postBodyUser = new User_Menu { UsrID = Um.UserID };
            //var responseUser = await Http.PostAsJsonAsync("Authen/ListUserMenu", postBodyUser);

            //if (UserMenuData != null)
            //{
            //    var fillter = UserMenuData.FindAll(x => x.MenuGroup == menuGroup);
            //    foreach (var item in fillter)
            //    {
            //        var menu = MenuData.FindAll(x => x.MenuId == item.MenuId);
            //        if (menu.Count > 0)
            //        {
            //            foreach (var mn in menu)
            //            {
            //                mn.IsAccess = item.IsAccess;
            //                mn.IsSave = item.IsSave;
            //                mn.IsDelete = item.IsDelete;
            //            }
            //        }
            //    }
            //}

            isLoading = false;
        }

        async Task Save()
        {
            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            UmMainPerms.ForEach(x=>x.UsrID = Um.UsrID);
            UmMainPerms.ForEach(x => x.CreatedBy = userData.UserID);

            MainMenuData.ForEach(x => x.UsrID = Um.UsrID);
            MainMenuData.ForEach(x => x.CreatedBy = userData.UserID);

            if (values != null)
            {
                foreach (var item in values)
                {
                    foreach (var pm in UmMainPerms)
                    {
                        if (pm.PermsName.Trim() == item.Trim())
                        {
                            pm.IsPerms = true;
                        }
                    }
                }
            }

            //if (MenuSubData.Count > 0)
            //{
            //    foreach (var item in MenuSubData)
            //    {
            //        var check = MenuData.FindAll(x=>x.MenuId == item.MenuId);
            //        if (check.Count > 0)
            //        {
            //            var up = MenuData.Find(x => x.MenuId == item.MenuId);
            //            if (up != null)
            //            {
            //                up.IsAccess = item.IsAccess;
            //                up.IsSave = item.IsSave;
            //                up.IsDelete = item.IsDelete;
            //            }
            //        }
            //        else
            //        {
            //            MenuData.Add(item);
            //        }
            //    }
            //}
            //if (MenuSub2Data.Count > 0)
            //{
            //    foreach (var item in MenuSub2Data)
            //    {
            //        var check = MenuData.FindAll(x => x.MenuId == item.MenuId);
            //        if (check.Count > 0)
            //        {
            //            var up = MenuData.Find(x => x.MenuId == item.MenuId);
            //            if (up != null)
            //            {
            //                up.IsAccess = item.IsAccess;
            //                up.IsSave = item.IsSave;
            //                up.IsDelete = item.IsDelete;
            //            }
            //        }
            //        else
            //        {
            //            MenuData.Add(item);
            //        }
            //    }
            //}

            userData.PermsList = UmMainPerms;
            userData.MenuList = MainMenuData;

            var response = await Http.PostAsJsonAsync("Authen/SavePerms", userData);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Success Summary", Detail = Rs.Msg, Duration = 4000 });
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = Rs.Msg, Duration = 4000 });
                }
            }
        }
        string MunuSubTitle = "เมนูของ";
        void OnCellMenuDataClick(DataGridCellMouseEventArgs<User_Menu> args)
        {
            if (!args.Data.IsAccess)
            {
                MenuSubData = new List<User_Menu>();
                return;
            }
            MenuSubData = MainMenuData.FindAll(x => x.MainMenuId == args.Data.MenuId);

            if (MenuSubData.Count > 0)
            {
                MunuSubTitle = $"เมนูของ > " + args.Data.MenuDesc;
                isLoading = true;

                MenuSubData.ForEach(x => x.UsrID = Um.UsrID);
                MenuSubData.ForEach(x => x.CreatedBy = userData.UserID);

                //if (UserMenuData != null)
                //{
                //    var fillter = UserMenuData.FindAll(x => x.MainMenuId == args.Data.MenuId);
                //    foreach (var item in fillter)
                //    {
                //        var menu = MenuSubData.FindAll(x => x.MenuId == item.MenuId);
                //        if (menu.Count > 0)
                //        {
                //            foreach (var mn in menu)
                //            {
                //                mn.IsAccess = item.IsAccess;
                //                mn.IsSave = item.IsSave;
                //                mn.IsDelete = item.IsDelete;
                //            }
                //        }
                //    }
                //}

                isLoading = false;
            }
        }
        string MunuSub2Title = "เมนูของ";
        void OnCellMenuSubDataClick(DataGridCellMouseEventArgs<User_Menu> args)
        {
            if (!args.Data.IsAccess)
            {
                MenuSub2Data = new List<User_Menu>();
                return;
            }
            MenuSub2Data = MainMenuData.FindAll(x => x.MainMenuId == args.Data.MenuId);

            if (MenuSub2Data.Count > 0)
            {
                MunuSub2Title = $"{MunuSubTitle} > " + args.Data.MenuDesc;
                isLoading = true;

                MenuSub2Data.ForEach(x => x.UsrID = Um.UsrID);
                MenuSub2Data.ForEach(x => x.CreatedBy = userData.UserID);

                //if (UserMenuData != null)
                //{
                //    var fillter = UserMenuData.FindAll(x=>x.MainMenuId == args.Data.MenuId);
                //    foreach (var item in fillter)
                //    {
                //        var menu = MenuSub2Data.FindAll(x => x.MenuId == item.MenuId);
                //        if (menu.Count > 0)
                //        {
                //            foreach (var mn in menu)
                //            {
                //                mn.IsAccess = item.IsAccess;
                //                mn.IsSave = item.IsSave;
                //                mn.IsDelete = item.IsDelete;
                //            }
                //        }
                //    }
                //}

                isLoading = false;
            }
        }
    }
}
