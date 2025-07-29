using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Cms
{
    public class Cms_Card_Trans : BaseShared
    {
        public int TransID { get; set; }
        public string? ContNo { get; set; }
        public string? RefNo { get; set; }
        public string? CustName { get; set; }
        public int Peroid { get; set; }
        public decimal PeroidAmt { get; set; }
        public string? AreaFrom { get; set; }
        public string? CashCodeFrom { get; set; }
        public string? CashNameFrom { get; set; }
        public string? EmpCodeFrom { get; set; }
        public string? AreaTo { get; set; }
        public string? CashCodeTo { get; set; }
        public string? CashNameTo { get; set; }
        public string? EmpCodeTo { get; set; }
        public string? CreateBy { get; set; }
        public int TransType { get; set; }
        public DateTime? NoticeDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsCardReturn { get; set; }
        public string? BfApproveTo { get; set; }

        public bool Result { get; set; }
        public string? ResultMsg { get; set; } = "";

    }
}
