using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Imps
{
    public class ImpStatus:BaseShared
    {
        public string? ContractNo { get; set; }
        public string? ContractStatus { get; set; }
        public DateTime? ContractStatusDate { get; set; }
        public string? CurrContractStatus { get; set; }
        public DateTime? CurrContractStatusDate { get; set; }
    }
}
