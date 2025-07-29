using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BD_InvoiceABH : BaseShared
    {
        public long id { get; set; }
        public string? InvNo { get; set; }
        public int Item { get; set; }
        public string? cashcodebh { get; set; }
        public string? cashnamebh { get; set; }
        public string? CashCode { get; set; }
        public string? RefNo { get; set; }
        public string? ContNo { get; set; }
        public string? PayPeriod { get; set; }
        public DateTime? DocDate { get; set; }
        public string? CashName { get; set; }
        public decimal Amount { get; set; }
        public string? CustName { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Flag { get; set; }
        public string? PayVat { get; set; }
        public string? Complete { get; set; }
        public string? AUser { get; set; }
        public decimal VAT { get; set; }
        public string? Cash { get; set; }
        public string? BookNo { get; set; }
        public string? ReceiptNo { get; set; }
        public string? Printed { get; set; }
        public decimal DisAmt { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public DateTime? paiddate { get; set; }
        public string? paidloc { get; set; }
        public string? paidby { get; set; }
        public string? paidkind { get; set; }
        public DateTime? paydate { get; set; }
        public string? cqbank { get; set; }
        public string? cqno { get; set; }
        public DateTime? cqdate { get; set; }
        public string? locno { get; set; }
        public string? doctime { get; set; }
        public string? okperiod { get; set; }
        public string? okdata { get; set; }
        public string? tomastpay { get; set; }
        public string? ispartial { get; set; }
    }
}
