using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Authen
{
    public class Authens
    {
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public bool RememberMe { get; set; }
        public bool IsAuthen { get; set; }
        public DateTime? LoginDate { get; set; }
        public string? AuthenMsg { get; set; }
        public string? ClientID { get; set; }
        public string? AppUrl { get; set; }
        public string? AppVersion { get; set; }
        public string? ServerName { get; set; }

        public User_Main Perms { get; set; } = new User_Main();
        public List<User_Perms> PermsList { get; set; } = new List<User_Perms>();
        public List<User_Menu> MenuList { get; set; } = new List<User_Menu>();
    }
}
