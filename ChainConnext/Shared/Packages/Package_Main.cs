using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Packages
{
    public class Package_Main
    {
        public string? PackageId { get; set; }
        public string? ItemCode { get; set; }
        public string? BrandCode { get; set; }
        public string? CatCode { get; set; }
        public string? CatSub1 { get; set; }
        public string? CatSub2 { get; set; }
        public string? ModelGroup { get; set; }
        public string? PackageDesc { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal PackageDiscount { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
