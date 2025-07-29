using ChainConnext.Shared.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Contracts
{
    public class Contract_Info_Clone: BaseShared
    {
        public Contract_Info ConInfo { get; set; } = new Contract_Info();
        public Contract_Info ConInfoNew { get; set; } = new Contract_Info();

        public List<Payment_Info> PayDataDelete { get; set; } = new List<Payment_Info>();
        public List<Payment_Info> PayDataDeleteNew { get; set; } = new List<Payment_Info>();
        public string? CreatedBy { get; set; }
        public string? Serder { get; set; }
        public string? Remark { get; set; }
    }
}
