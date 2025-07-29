using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.ARs
{
    public class IRR_Contract_Cal_Detail : IRR_Contract_Cal
    {
        public decimal RemAmt { get; set; }
        public decimal TaxRemAmt { get; set; }
        public decimal IntAmt { get; set; }
        public decimal IntRemAmt { get; set; }
        public decimal PrntAmt { get; set; }
        public decimal CutPrntAmt { get; set; }
        public decimal CutAmt { get; set; }
    }
}
