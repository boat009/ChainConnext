using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Payments
{
    public class Payment_Transaction: Payment_Info
    {
        public string? PayTransId { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal PayVat { get; set; }
        public decimal PayDisc { get; set; }
    }
}
