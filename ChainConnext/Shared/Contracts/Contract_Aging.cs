using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Contracts
{
    public class Contract_Aging : BaseShared
    {
        public string? ContractId { get; set; }
        public decimal ContractAmt { get; set; }
        public decimal DiscountAmt { get; set; }
        public decimal PeroidAmt { get; set; }
        public int Peroid { get; set; }
        public decimal VatPeroid { get; set; }
        public decimal PeroidVat { get; set; }
        public decimal PeroidVat1Amt { get; set; }
        public decimal PeroidVatAllAmt { get; set; }
        public decimal SaleAmt { get; set; }
        public decimal InterestAmt { get; set; }
        public decimal IRR { get; set; }
        public decimal FirstPeroidAmt { get; set; }
        public decimal UpPeroidAmt { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
