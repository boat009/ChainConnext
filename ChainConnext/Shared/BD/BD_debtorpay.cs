using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BD_debtorpay : BaseShared
    {
        public long id { get; set; }
        public string? docno { get; set; }
        public string? refno { get; set; }
        public string? contno { get; set; }
        public string? name { get; set; }
        public DateTime? duedate { get; set; }
        public string? payperiod { get; set; }
        public DateTime? paydate { get; set; }
        public string? bookno { get; set; }
        public string? invno { get; set; }
        public decimal premium { get; set; }
        public DateTime? vatdate { get; set; }
        public string? vatdesc { get; set; }
        public decimal vat { get; set; }
        public decimal amt { get; set; }
        public decimal ur { get; set; }
        public decimal uv { get; set; }
        public decimal ir { get; set; }
        public decimal br { get; set; }
        public string? cashcode { get; set; }
        public string? cashname { get; set; }
        public string? mode { get; set; }
        public string? model { get; set; }
        public DateTime? effdate { get; set; }
        public string? status { get; set; }
        public DateTime? stdate { get; set; }
        public string? serialno { get; set; }
        public string? posted { get; set; }
        public string? flag { get; set; }
        public DateTime? imdate { get; set; }
        public DateTime? openacc { get; set; }
        public DateTime? closeacc { get; set; }
        public string? iscalc { get; set; }
        public DateTime? calcdate { get; set; }
        public string? calcby { get; set; }
        public string? calctime { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }
        public bool is_red { get; set; }
    }
}
