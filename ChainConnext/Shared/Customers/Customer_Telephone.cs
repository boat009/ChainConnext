using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Customers
{
    public class Customer_Telephone : Customer_Telephone_Type
    {
        public string? TelId { get; set; }
        public string? CustomerId { get; set; }
        public string? TelNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? ContractId { get; set; }
    }
}
