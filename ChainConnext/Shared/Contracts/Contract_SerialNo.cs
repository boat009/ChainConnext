using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Contracts
{
    public class Contract_SerialNo: Contract_SerialNoType
    {
        public string? SerialNoId { get; set; }
        public string? ContractId { get; set; }
        public string? SerialNo { get; set; }
        public int SerialNoTypeId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
