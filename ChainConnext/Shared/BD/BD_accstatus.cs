using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BD_accstatus : BaseShared
    {
        public long id { get; set; }
        public string? refno { get; set; }
        public string? contno { get; set; }
        public string? name { get; set; }
        public string? status1 { get; set; }
        public string? status2 { get; set; }
        public string? status3 { get; set; }
        public string? status4 { get; set; }
        public string? status5 { get; set; }
        public string? status6 { get; set; }
        public DateTime? stdate1 { get; set; }
        public DateTime? stdate2 { get; set; }
        public DateTime? stdate3 { get; set; }
        public DateTime? stdate4 { get; set; }
        public DateTime? stdate5 { get; set; }
        public DateTime? stdate6 { get; set; }
        public string? payperiod1 { get; set; }
        public string? payperiod2 { get; set; }
        public string? payperiod3 { get; set; }
        public string? payperiod4 { get; set; }
        public decimal credit { get; set; }
        public decimal sales { get; set; }
        public decimal paid { get; set; }
        public decimal balance { get; set; }
        public decimal disc { get; set; }
        public string? contstatus { get; set; }
        public DateTime? contstdate { get; set; }
        public string? serialno { get; set; }
        public string? mode { get; set; }
        public string? todebtor { get; set; }
        public decimal premium { get; set; }
        public decimal premium2 { get; set; }
        public string? model { get; set; }
        public decimal netcredit { get; set; }
        public decimal nextcredit { get; set; }
        public decimal firstdisc { get; set; }
        public string? sended { get; set; }
        public int items { get; set; }
        public string? bcode { get; set; }
        public string? salecode { get; set; }
        public string? docno1 { get; set; }
        public string? docno2 { get; set; }
        public string? docno3 { get; set; }
        public string? docno4 { get; set; }
        public string? docno5 { get; set; }
        public string? docno6 { get; set; }
        public string? oldserialno { get; set; }
        public string? status7 { get; set; }
        public DateTime? stdate7 { get; set; }
        public string? oldcontno { get; set; }
        public string? oldrefno { get; set; }
        public string? docno7 { get; set; }
        public DateTime? effdate { get; set; }
        public string? posted { get; set; }
        public string? flag { get; set; }
        public string? freeze { get; set; }
        public string? shift { get; set; }
        public string? mark { get; set; }
        public decimal bfbal { get; set; }
        public string? st2post { get; set; }
        public string? st3post { get; set; }
        public string? st4post { get; set; }
        public string? updcode { get; set; }
        public string? islate { get; set; }
        public DateTime? openacc { get; set; }
        public DateTime? closeacc { get; set; }
        public string? tocode { get; set; }
        public string? torefno { get; set; }
        public string? tocontno { get; set; }
        public DateTime? todate { get; set; }
        public string? tonote { get; set; }
        public string? fromrefno { get; set; }
        public string? fromcontno { get; set; }
        public string? chanel { get; set; }
        public string? status8 { get; set; }
        public DateTime? stdate8 { get; set; }
        public bool is_todebtor { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        public int c_all { get; set; }
        public int c_todebtor_1 { get; set; }
        public int c_todebtor_0 { get; set; }
    }
}
