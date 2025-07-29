using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Imps
{
    public class ImpBHCheck : BaseShared
    {
        public string? ReceiptCode { get; set; }
        public string? PayPlace { get; set; }
        public DateTime? TransDate { get; set; }
        public int BillRecvID { get; set; }
        public string? Remark { get; set; }
        public string? CheckBy { get; set; } = "";
        public bool Result { get; set; }
        public string? ResultMsg { get; set; } = "";
    }
}
