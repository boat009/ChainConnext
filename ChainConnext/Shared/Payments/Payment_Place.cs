using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Payments
{
    public class Payment_Place
    {
        public int PayPlaceId { get; set; }
        public int PayPlaceGroupId { get; set; }
        public string? PayPlaceName { get; set; }
        public string? BillType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsActive { get; set; }
        public string? PayPlaceCode { get; set; }
        public int PayById { get; set; }
        public string? AccNo { get; set; }
        public string? PLoanCode { get; set; }
        public bool IsTcb { get; set; }
        public string? PayPlaceName2 { get;set; }
    }
}
