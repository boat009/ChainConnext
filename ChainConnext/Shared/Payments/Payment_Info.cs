using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Payments
{
    public class Payment_Info : BaseShared
    {
        public string? PaymentId { get; set; }
        public string? OrderId { get; set; }
        public string? ContractId { get; set; }
        public int PayPeroid { get; set; }
        public int PayNum { get; set; } = 1;
        public decimal PayAmt { get; set; }
        public DateTime? PayDate { get; set; } = DateTime.Now.AddDays(-1);
        public DateTime? DocDate { get; set; } = DateTime.Now;
        public DateTime? TransDate { get; set; } = DateTime.Now;
        public string? BookNo { get; set; }
        public string? NumNo { get; set; }
        public int PayTypeId { get; set; }
        public int PayPlaceId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsActive { get; set; } = 1;
        public string? TaxRecvNo { get; set; }
        public string? RecvID { get; set; }
        public string? ContractNoNew { get; set; }
        public int DepID { get; set; }
        public int FnNo { get; set; }
        public int FnYear { get; set; }
        public string? Remark { get; set; }
        public string? InvNo { get; set; }
        public string? FromSource { get; set; }

        public string? CashCode { get; set; }

        public string? CashCodeBH { get; set; }

        public string? CashName { get; set; }

        public string? RecvDate { get; set; }
        public int PayById { get; set; }
        public string? Marking { get; set; }
        public string? RefNo { get; set; }
        public string? ContractNo { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
