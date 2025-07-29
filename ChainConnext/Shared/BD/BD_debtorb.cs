using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BD_debtorb : BaseShared
    {
        public long id { get; set; }
        public string? docno { get; set; }
        public string? refno { get; set; }
        public string? contno { get; set; }
        public int period { get; set; }
        public decimal premium { get; set; }
        public decimal unpaid { get; set; }
        public decimal uv1 { get; set; }
        public decimal uv2 { get; set; }
        public decimal ur1 { get; set; }
        public decimal ur2 { get; set; }
        public decimal amt { get; set; }
        public decimal bal { get; set; }
        public decimal paid1 { get; set; }
        public string? lock_txt { get; set; }
        public decimal amt1 { get; set; }
    }
}
