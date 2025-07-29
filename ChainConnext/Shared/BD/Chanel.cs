using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class Chanel
    {
        public long id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public string? ChanelTag { get; set; }
        public string? ChanelMap { get; set; }
        public bool Used { get; set; }
        public string? aline { get; set; }
        public string? ateam { get; set; }
        public int ChanelDep { get; set; }
        public string? teamno { get; set; }
        public string? CreateBy { get; set; }
        public string? ChanelDepName { get; set; }
    }
}
