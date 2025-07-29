using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared
{
    public class FindMode : BaseShared
    {
        public int FindID { get; set; }
        public string? FindName { get; set; }
        public string? FindValue { get; set; }
        public string? FindBy { get; set; }
    }
}
