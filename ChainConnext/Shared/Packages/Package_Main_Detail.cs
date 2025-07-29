using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Packages
{
    public class Package_Main_Detail
    {
        public string? PackageDetailId { get; set; }
        public string? PackageId { get; set; }
        public string? ModelCode { get; set; }
        public int Peroid { get; set; }
        public decimal PeroidAmt { get; set; }
        public decimal DiscountAmt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
