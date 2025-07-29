using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Customers
{
    public class Customer_Guar
    {
        public int Guarid { get; set; }
        public string? CustomerId { get; set; }
        public int ContractId { get; set; }
        public int GuarType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
