using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Customers
{
    public class Customer_Info : BaseShared
    {
        public string? CustomerId { get; set; }
        public string? CitizenId { get; set; }
        public DateTime? CitizenStartDate { get; set; }
        public DateTime? CitizenExpireDate { get; set; }
        public string? CompanyName { get; set; }
        public string? AuthorizedName { get; set; }
        public string? AuthorizeId { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Sex { get; set; }
        public int Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? RegisterDate { get; set; }
        public string? AccountingStatus { get; set; }
        public int CustomerType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsActive { get; set; } = 1;
        public int CardTypeId { get; set; } = 1;
        public string? BHCustomerID { get; set; }
        public string? FirstNameEn { get; set; }
        public string? LastNameEn { get; set; }
        public string? PrefixName { get; set; }
        public string? PLoanCustomerID { get; set; }
        public string? CustomerName { get; set; }
        public string? ContractId { get; set; }
    }
}
