using Blazored.LocalStorage;
using ChainConnext.Client.AuthProviders;
using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace ChainConnext.Client.Services
{
    public interface IAccountService
    {
        Task<(bool, string)> LoginAsync(LoginRequest model);
        Task<bool> LogoutAsync(LogoutRequest model);
        Task<Authens> GetAuthensAsync(string url);
        Task UpdatePermsAsync();
    }
    public class AccountService : IAccountService
    {
        private readonly AuthenticationStateProvider _customAuthenticationProvider;
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;
        public AccountService(ILocalStorageService localStorageService, AuthenticationStateProvider customAuthenticationProvider, HttpClient httpClient)
        {
            _localStorageService = localStorageService;
            _customAuthenticationProvider = customAuthenticationProvider;
            _httpClient = httpClient;
        }
        public async Task<(bool, string)> LoginAsync(LoginRequest model)
        {
            var postBody = new LoginRequest
            {
                UserName = model.UserName,
                Password = model.Password,
                RememberMe = model.RememberMe,
                Latt = model.Latt,
                Longtt = model.Longtt,
                CurrentURL = model.CurrentURL,
                ClientID = Guid.NewGuid().ToString()
            };

            var response = await _httpClient.PostAsJsonAsync("Authen/CheckLogin", postBody);
            //HttpClient Http = new HttpClient();
            //var response = await Http.PostAsJsonAsync("User/CheckLogin", postBody);
            if (!response.IsSuccessStatusCode)
            {
                return (false, response.RequestMessage.ToString());
            }

            var UserMainData = await response.Content.ReadFromJsonAsync<Authens>();
            if (UserMainData != null)
            {
                if (!string.IsNullOrEmpty(UserMainData.UserID))
                {
                    string Token = Newtonsoft.Json.JsonConvert.SerializeObject(UserMainData);
                    Token = BaseShared.Base64EncodeSession(Token);
                    //string cc = ShareValues.GetTokenUrl();
                    await _localStorageService.SetItemAsync(ShareValues.GetTokenUrl(), Token);
                    (_customAuthenticationProvider as AuthStateProvider).Notify();
                    return (UserMainData.IsAuthen, UserMainData.AuthenMsg);
                }
                else
                {
                    return (false, $"Authen fail({UserMainData.AuthenMsg})");
                }
            }
            else
            {
                return (false, "Authen fail(else)");
            }
        }

        public async Task UpdatePermsAsync()
        {
            string token = await _localStorageService.GetItemAsync<string>(ShareValues.GetTokenUrl());
            if (token != null)
            {
                bool is_change = false;
                Authens? UserData = Newtonsoft.Json.JsonConvert.DeserializeObject<Authens>(BaseShared.Base64DecodeSession(token));
                if (UserData != null)
                {
                    var mn = UserData.MenuList.FirstOrDefault();
                    if (mn != null)
                    {
                        var postBody = new User_Menu { UsrID = UserData.UserID, DataDate = mn.DataDate };

                        var response = await _httpClient.PostAsJsonAsync("Authen/ListUserMenuCheck", postBody);
                        ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                        if (Rs != null)
                        {
                            if (Rs.IsSuccess)
                            {
                                UserData.MenuList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User_Menu>>(Rs.Data.ToString());
                                is_change = true;
                            }
                        }
                    }
                    var prm = UserData.PermsList.FirstOrDefault();
                    if (prm != null)
                    {
                        var postBody = new User_Perms { UsrID = UserData.UserID, DataDate = prm.DataDate };

                        var response = await _httpClient.PostAsJsonAsync("Authen/ListUserPermsCheck", postBody);
                        ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
                        if (Rs != null)
                        {
                            if (Rs.IsSuccess)
                            {
                                UserData.PermsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User_Perms>>(Rs.Data.ToString());
                                is_change = true;
                            }
                        }
                    }
                }
                if (is_change)
                {
                    string Token = Newtonsoft.Json.JsonConvert.SerializeObject(UserData);
                    Token = BaseShared.Base64EncodeSession(Token);
                    //string cc = ShareValues.GetTokenUrl();
                    await _localStorageService.SetItemAsync(ShareValues.GetTokenUrl(), Token);
                    (_customAuthenticationProvider as AuthStateProvider).Notify();
                }
            }
        }

        public async Task<bool> LogoutAsync(LogoutRequest model)
        {
            var postBody = new LoginRequest
            {
                UserID = model.UserID,
                Latt = model.Latt,
                Longtt = model.Longtt
            };
            var response = await _httpClient.PostAsJsonAsync("Logout/CheckLogout", postBody);
            var UserMainData = await response.Content.ReadFromJsonAsync<Authens>();
            if (UserMainData != null)
            {

            }
            await _localStorageService.RemoveItemAsync(ShareValues.GetTokenUrl());
            (_customAuthenticationProvider as AuthStateProvider).Notify();
            return true;
        }

        public async Task<Authens> GetAuthensAsync(string url)
        {
            Authens userData = new Authens();
            string token = await _localStorageService.GetItemAsync<string>(ShareValues.GetTokenUrl());
            if (token != null)
            {
                userData = Newtonsoft.Json.JsonConvert.DeserializeObject<Authens>(BaseShared.Base64DecodeSession(token));
            }
            Version version = typeof(Program).Assembly.GetName().Version;
            userData.AppVersion = version.ToString();
            userData.AppUrl = url;
            return userData;
        }
    }
}
