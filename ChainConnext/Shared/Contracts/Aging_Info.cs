using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Contracts
{
    public class Aging_Info : BaseShared
    {
        public int PeroidId { get; set; }
        public string? ContractId { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? AppointDate { get; set; }
        public decimal PeroidAmt { get; set; }
        public decimal PayAmt { get; set; }
        public DateTime? PayDate { get; set; }
        public string? KeepingCode { get; set; }
        public DateTime? KeepingDate { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public decimal DiscAmt { get; set; }
        public bool IsComplete { get; set; }
        public string? RecvID { get; set; }
        public decimal VatAmt { get; set; }
        public decimal CurrBal { get; set; }
        public decimal VatBal { get; set; }
        public int PeroidRem { get; set; }
        public decimal EfectiveAmt { get; set; }
        public decimal EfectiveBal { get; set; }
        public decimal PeroidBal { get; set; }
        public decimal TmpIrr { get; set; }
    }
}
