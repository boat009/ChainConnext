using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class TranSt:BaseShared
    {
        public string? TranNo { get; set; }
        public string? ContNo { get; set; }
        public string? Status { get; set; }
        public DateTime? StDate { get; set; }
        public DateTime? ChgDate { get; set; }
        public string? Auser { get; set; }
        public string? RefNo { get; set; }
    }
}
