using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Payments
{
    public class Payment_PayBy
    {
        public int PayById { get; set; }
        public string? PayByName { get; set; }
        public string? PayByName2 { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsActive { get; set; }
    }
}
