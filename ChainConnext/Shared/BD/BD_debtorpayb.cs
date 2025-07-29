using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BD_debtorpayb : BaseShared
    {
        public long id { get; set; }
        public string? docnoref { get; set; }
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
        public string? kind { get; set; }
        public string? code { get; set; }
        public string? posted { get; set; }
        public string? bcode { get; set; }
        public string? flag { get; set; }
        public decimal bfbal { get; set; }
        public decimal disc { get; set; }
        public string? glcode { get; set; }
        public string? norefno { get; set; }
        public string? tored { get; set; }
        public string? toblack { get; set; }
        public DateTime? openacc { get; set; }
        public DateTime? closeacc { get; set; }
        public string? status1 { get; set; }
        public string? status2 { get; set; }
        public string? status3 { get; set; }
        public string? status4 { get; set; }
        public DateTime? imdate { get; set; }
        public DateTime? todate { get; set; }
        public DateTime? stdate1 { get; set; }
        public DateTime? stdate2 { get; set; }
        public DateTime? stdate3 { get; set; }
        public DateTime? stdate4 { get; set; }
    }
}
