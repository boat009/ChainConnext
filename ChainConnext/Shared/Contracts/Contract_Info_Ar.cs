using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Contracts
{
    public class Contract_Info_Ar : BaseShared
    {
        public string? RefNo { get; set; }
        public DateTime? EFFDATE { get; set; }
        public int AccStatusData { get; set; }
        public int DebtorAData { get; set; }
        public string? torefno { get; set; }
        public string? tocode { get; set; }
        public string? tonote { get; set; }
        public string? CONTNO { get; set; }
        public string? MODEL { get; set; }
        public string? SERIALNO { get; set; }
        public string? MODE { get; set; }
        public decimal SALES { get; set; }
        public decimal CREDIT { get; set; }
        public decimal premium2 { get; set; }
        public decimal PREMIUM { get; set; }
        public decimal Disc { get; set; }
        public decimal netcredit { get; set; }
        public decimal TOTALPAY { get; set; }
        public string? chanel { get; set; }
        public string? bcode { get; set; }
    }
}
