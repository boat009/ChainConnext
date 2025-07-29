using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BD_MastPay : BaseShared
    {
        public string? RefNo { get; set; }
        public string? CONTNO { get; set; }
        public string? PAYPERIOD { get; set; }
        public DateTime? PAYDATE { get; set; }
        public string? BOOKNO { get; set; }
        public string? RECEIPTNO { get; set; }
        public string? COMPANY { get; set; }
        public decimal PREMIUM { get; set; }
        public DateTime? ENTERDATE { get; set; }
        public string? CashCode { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string? Remark { get; set; }
        public decimal Disc { get; set; }
        public DateTime? OpenDate { get; set; }
        public string? PayVat { get; set; }
        public DateTime? CloseDate { get; set; }
        public string? InvNo { get; set; }
        public string? Verified { get; set; }
        public string? Printed { get; set; }
        public decimal VAT { get; set; }
        public string? PrvCode { get; set; }
        public string? CrFlag { get; set; }
        public decimal Ur { get; set; }
        public decimal Ur1 { get; set; }
        public decimal Uv { get; set; }
        public decimal Uv1 { get; set; }
        public decimal Balance { get; set; }
        public string? isturn { get; set; }
        public string? toacc { get; set; }
        public DateTime? toaccdate { get; set; }
        public DateTime? paiddate { get; set; }
        public string? paidloc { get; set; }
        public string? paidby { get; set; }
        public string? paidkind { get; set; }
        public DateTime? docdate { get; set; }
        public string? cqbank { get; set; }
        public DateTime? cqdate { get; set; }
        public string? cqno { get; set; }
        public string? locno { get; set; }
        public string? cashcodebh { get; set; }
        public string? comefrom { get; set; }
        public string? cashname { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? scode { get; set; }
        public string? bcode { get; set; }
        public string? chanel { get; set; }
    }
}
