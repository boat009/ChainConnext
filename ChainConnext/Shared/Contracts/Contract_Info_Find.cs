using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Contracts
{
    public class Contract_Info_Find: Contract_Info
    {
        public string? CitizenId { get; set; }
        public DateTime? EffDateTo { get; set; }
    }
}
