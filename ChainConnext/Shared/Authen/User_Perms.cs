using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Authen
{
    public class User_Perms: User_Perms_Main
    {
        public int PermsId { get; set; }
        public string? UsrID { get; set; }
        public bool IsPerms { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? DataDate { get; set; }
    }
}
