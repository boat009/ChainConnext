using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Reports
{
    public class Tmp_ReportDaily_Payment:BaseShared
    {
        public string? InvNo { get; set; }
        public string? CashCodeBD { get; set; }
        public string? CashNameBD { get; set; }
        public string? CashCodeBH { get; set; }
        public string? CashCode { get; set; }
        public string? CashName { get; set; }
        public string? RefNo { get; set; }
        public string? ContNo { get; set; }
        public int PayPeriod { get; set; }
        public DateTime? DocDate { get; set; }
        public decimal Amount { get; set; }
        public string? CustName { get; set; }
        public DateTime? DueDate { get; set; }
        public string? PayVat { get; set; }
        public string? AUser { get; set; }
        public string? BookNo { get; set; }
        public string? ReceiptNo { get; set; }
        public decimal DisAmt { get; set; }
        public DateTime? paiddate { get; set; }
        public string? paidloc { get; set; }
        public string? paidby { get; set; }
        public string? paidkind { get; set; }
        public DateTime? paydate { get; set; }
        public string? cqbank { get; set; }
        public string? cqno { get; set; }
        public string? cqdate { get; set; }
        public string? locno { get; set; }
        public string? IsSent { get; set; }
        public bool PayPartial { get; set; }
        public string? TmpNo { get; set; }
        public int TmpID { get; set; }
        public string? CreateBy { get; set; }
        public bool IsMastPay { get; set; }
        public bool IsDup { get; set; }
        public bool CheckMastPay { get; set; }

        public bool IsError { get; set; }
        public DateTime? ErrorDate { get; set; }
        public string? ErrorMsg { get; set; }
        public string? ErrorBy { get; set; }

        public bool CheckInvoiceABH { get; set; }
        public bool CanSave { get; set; }
    }
}
