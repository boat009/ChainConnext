using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Authen
{
    public class User_Main
    {
        public string? UsrID { get; set; }
        public string? EmpName { get; set; }
        public bool IsSave { get; set; } = false;
        public bool IsDelete { get; set; } = false;
        public bool IsPrint { get; set; } = false;
        public bool IsSetting { get; set; } = false;
        public bool IsReport { get; set; } = false;
        public bool IsImport { get; set; } = false;
        public DateTime? LoginDate { get; set; }
    }
}
