using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.ARs
{
    public class IRR_Contract_Cal
    {
        public decimal Credit { get; set; }
        public decimal Sales { get; set; }
        public int Peroid { get; set; }
        public decimal PeroidAmt { get; set; }
        public decimal FirstPeroidAmt { get; set; }
        public decimal NetCredit { get; set; }
        public decimal Discount { get; set; }

        public decimal DebtAmt { get; set; }
        public decimal SaleAmt { get; set; }
        public decimal DeferredAmt { get; set; }
        public decimal TaxAmt { get; set; }
        public decimal IntRate { get; set; }
        public decimal VatAmt1 { get; set; }
        public decimal VatAmt { get; set; }
        public decimal VatAmtL { get; set; }
        public decimal PeroidAmtL { get; set; }
    }
}
