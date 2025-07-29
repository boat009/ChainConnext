using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Customers
{
    public class Customer_Address: Customer_Address_Type
    {
        public string? AddressId { get; set; }
        public string? CustomerId { get; set; }
        public string? ContractId { get; set; }
        public string? AddressTitle { get; set; }
        public string? AddressTitle2 { get; set; }
        public string? AddressTitle3 { get; set; }
        public string? AddressTitle4 { get; set; }
        public string? AddressSubdistrict { get; set; }
        public string? AddressDistrict { get; set; }
        public string? AddressProvince { get; set; }
        public string? AddressZipcode { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? AddressDate { get; set; }
        public string? AddressFull { get; set; }
        public string? AddressSubdistrict1 { get; set; }
        public string? AddressDistrict1 { get; set; }
        public string? AddressProvince1 { get; set; }
    }
}
