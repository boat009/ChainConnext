using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Payments
{
    public class Payment_Place_Group
    {
        public int PayPlaceGroupId { get; set; }
        public string? PayPlaceGroupName { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsActive { get; set; }
    }
}
