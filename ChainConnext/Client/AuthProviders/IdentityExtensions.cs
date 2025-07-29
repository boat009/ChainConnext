using ChainConnext.Shared;
using ChainConnext.Shared.Authen;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChainConnext.Client
{
    public static class IdentityExtensions
    {
        public static string GetUserID(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.UserID);

            if (claim == null)
                return "";

            return claim.Value;
        }
        public static string GetUserName(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.UserName);

            if (claim == null)
                return "";

            return claim.Value;
        }
        public static string GetFullName(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.FullName);

            if (claim == null)
                return "";

            return claim.Value;
        }
        public static string GetData(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.Data);

            if (claim == null)
                return "";

            return claim.Value;
        }

        public static Authens GetAuthenData(this IIdentity identity)
        {
            Authens Data = new Authens();
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.Data);

            if (claim != null)
            {
                Data = Newtonsoft.Json.JsonConvert.DeserializeObject<Authens>(claim.Value);
            }
            return Data;
        }

        public static string GetName(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.Name);

            return claim?.Value ?? string.Empty;
        }
        public static List<User_Perms> GetPerms(this IIdentity identity)
        {
            List<User_Perms> Data = new List<User_Perms>();
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.Perms);
            if (claim != null)
            {
                Data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User_Perms>>(claim.Value);
            }
            return Data;
        }
        public static List<User_Menu> GetMenus(this IIdentity identity)
        {
            List<User_Menu> Data = new List<User_Menu>();
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.Menus);
            if (claim != null)
            {
                Data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User_Menu>>(claim.Value);
            }
            return Data;
        }
    }
}
