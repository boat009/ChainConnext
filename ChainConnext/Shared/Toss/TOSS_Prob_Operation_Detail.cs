using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Toss
{
    public class TOSS_Prob_Operation_Detail : BaseShared
    {
        public int refno { get; set; }
        public DateTime? ResponDate { get; set; }
        public string? ProblemDetails { get; set; }
        public string? Custname { get; set; }
    }
}
