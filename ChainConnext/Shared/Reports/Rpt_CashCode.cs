using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Reports
{
    public class Rpt_CashCode
    {
        public string? RefNo { get; set; }
        public string? ContNo { get; set; }
        public string? CashCode { get; set; }
        public string? CashName { get; set; }
        public string? BHCode { get; set; }
        public DateTime? DateFrom { get; set; }

        public string? Msg { get; set; } = "";
    }
}
