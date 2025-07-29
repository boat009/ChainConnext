using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared
{
    public class LoginRequest
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
        public string? Latt { get; set; }
        public string? Longtt { get; set; }
        public string? UserID { get; set; }
        public string? ClientID { get; set; }
        public string? CurrentURL { get; set; }
    }
}
