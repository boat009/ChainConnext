using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Customers
{
    public class Customer_Address_Type : BaseShared
    {
        public int AddressTypeId { get; set; }
        public string? AddressTypeName { get; set; }
        public string? AddressTypeNameBH { get; set; }
    }
}
