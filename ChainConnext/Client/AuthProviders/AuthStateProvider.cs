using Blazored.LocalStorage;
using ChainConnext.Client.Services;
using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ChainConnext.Client.AuthProviders
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        public AuthStateProvider(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        private ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                string token = await _localStorageService.GetItemAsync<string>(ShareValues.GetTokenUrl());
                if (token == null)
                {
                    var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity() { }));
                    return anonymous;
                }
                if (string.IsNullOrEmpty(token))
                {
                    var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity() { }));
                    return anonymous;
                }
                string Data = "";
                Authens? UserData = null;
                if (token.StartsWith("["))
                {
                    UserData = Newtonsoft.Json.JsonConvert.DeserializeObject<Authens>(token);
                    UserData.RememberMe = false;
                }
                else
                {
                    try
                    {
                        UserData = Newtonsoft.Json.JsonConvert.DeserializeObject<Authens>(BaseShared.Base64DecodeSession(token));
                    }
                    catch
                    {
                        await _localStorageService.RemoveItemAsync(ShareValues.GetTokenUrl());
                    }
                }
                if (UserData != null)
                {
                    try
                    {
                        Data = Newtonsoft.Json.JsonConvert.SerializeObject(UserData);
                        string PermsData = Newtonsoft.Json.JsonConvert.SerializeObject(UserData.PermsList);
                        string MenuData = Newtonsoft.Json.JsonConvert.SerializeObject(UserData.MenuList);
                        if (!UserData.RememberMe)
                        {
                            if (UserData.LoginDate != null)
                            {
                                if ((DateTime.Now - UserData.LoginDate.Value).TotalHours >= 24)
                                {
                                    await _localStorageService.RemoveItemAsync(ShareValues.GetTokenUrl());
                                    var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity() { }));
                                    return anonymous;
                                }
                            }
                        }
                        if (UserData.IsAuthen)
                        {
                            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, UserData.FullName),
                            new Claim(CustomClaimTypes.UserID, UserData.UserID),
                            new Claim(CustomClaimTypes.UserName, UserData.UserName),
                            new Claim(CustomClaimTypes.FullName, UserData.FullName),
                            new Claim(CustomClaimTypes.Data, Data),
                            new Claim(CustomClaimTypes.Perms, PermsData),
                            new Claim(CustomClaimTypes.Menus, MenuData)
                        };
                            if (UserData.PermsList.Count > 0)
                            {
                                foreach (var item in UserData.PermsList)
                                {
                                    if (item.IsPerms)
                                    {
                                        if (item.PermsName != null)
                                        {
                                            claims.Add(new Claim(ClaimTypes.Role, item.PermsName));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (UserData.Perms.IsSave)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "IsSave"));
                                }
                                if (UserData.Perms.IsDelete)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "IsDelete"));
                                }
                                if (UserData.Perms.IsPrint)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "IsPrint"));
                                }
                                if (UserData.Perms.IsSetting)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "IsSetting"));
                                }
                                if (UserData.Perms.IsReport)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "IsReport"));
                                }
                                if (UserData.Perms.IsImport)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "IsImport"));
                                }
                            }

                            var identity = new ClaimsIdentity(claims, "BlazorApp Support Authentication");

                            var userClaimPrincipal = new ClaimsPrincipal(identity);
                            var loginUser = new AuthenticationState(userClaimPrincipal);
                            return loginUser;
                        }
                        else
                        {
                            var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity() { }));
                            return anonymous;
                        }
                    }
                    catch (Exception ex)
                    {
                        var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity() { }));
                        return anonymous;
                    }
                }
                else
                {
                    var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity() { }));
                    return anonymous;
                }
            }
            catch 
            {
                var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity() { }));
                return anonymous;
            }
        }

        public void Notify()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
