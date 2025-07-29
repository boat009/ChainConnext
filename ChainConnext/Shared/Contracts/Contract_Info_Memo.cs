using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Contracts
{
    public class Contract_Info_Memo : BaseShared
    {
        public string? ContractId { get; set; }
        public string? Memo { get; set; }
        public string? CreatedBy { get; set; }
    }
}
