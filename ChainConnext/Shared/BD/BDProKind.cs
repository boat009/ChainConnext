using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BDProKind
    {
        public string? code { get; set; }
        public string? name { get; set; }
        public decimal rate { get; set; }
        public int formula { get; set; }
        public string? KindType { get; set; }
        public string? KindTypeDesc { get; set; }
        public string? KindCodeName { get; set; }
        public string? CreateBy { get; set; }
    }
}
