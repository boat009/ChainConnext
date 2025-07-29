using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BD_debtoragi : BaseShared
    {
        public long id { get; set; }
        public string? refno { get; set; }
        public decimal premium { get; set; }
        public int unmode { get; set; }
        public decimal balance { get; set; }
        public decimal netbal { get; set; }
        public decimal vatbal { get; set; }
        public decimal vat { get; set; }
        public decimal urbal { get; set; }
        public decimal uv { get; set; }
        public decimal ur { get; set; }
        public DateTime? duedate { get; set; }
        public decimal curr { get; set; }
        public decimal netur { get; set; }
        public DateTime? accdate { get; set; }
        public decimal payamt { get; set; }
        public decimal overamt { get; set; }
        public int no1 { get; set; }
        public int no2 { get; set; }
        public int no3 { get; set; }
        public int no4 { get; set; }
        public decimal period1 { get; set; }
        public decimal period2 { get; set; }
        public decimal period3 { get; set; }
        public decimal period4 { get; set; }
        public decimal period5 { get; set; }
        public decimal period6 { get; set; }
        public decimal period7 { get; set; }
        public decimal inbal { get; set; }
        public string? cate { get; set; }
        public string? flag { get; set; }
        public string? lock_txt { get; set; }
        public DateTime? closedate { get; set; }
        public int no5 { get; set; }
        public int no6 { get; set; }
        public decimal gur1 { get; set; }
        public decimal guv1 { get; set; }
        public decimal gur2 { get; set; }
        public decimal guv2 { get; set; }
        public int no7 { get; set; }
        public int no8 { get; set; }
        public int no9 { get; set; }
        public int no10 { get; set; }
        public decimal gbal1 { get; set; }
        public decimal gbal2 { get; set; }
        public decimal gamt1 { get; set; }
        public decimal gamt2 { get; set; }
        public decimal gnetbal1 { get; set; }
        public decimal gnetbal2 { get; set; }
        public DateTime? paydate { get; set; }
        public string? bcode { get; set; }
        public string? bname { get; set; }
        public string? chanel { get; set; }
        public string? modelkind { get; set; }
        public string? modelkindname { get; set; }
        public string? urflag { get; set; }
        public DateTime? urflagdate { get; set; }
        public decimal urflagamt { get; set; }
        public decimal gperiod0 { get; set; }
        public decimal gperiod1 { get; set; }
        public decimal gperiod2 { get; set; }
        public decimal gperiod3 { get; set; }
        public decimal gperiod4 { get; set; }
        public decimal gperiod5 { get; set; }
        public decimal gperiod6 { get; set; }
        public decimal gperiod7 { get; set; }
        public int gday1 { get; set; }
        public int gday2 { get; set; }
        public DateTime? gdate1 { get; set; }
        public DateTime? gdate2 { get; set; }
        public int gperiod { get; set; }
        public DateTime? gdate3 { get; set; }
        public DateTime? gdate4 { get; set; }
        public string? segment { get; set; }
        public string? defstatus { get; set; }
        public decimal covid0 { get; set; }
        public decimal covid1 { get; set; }
        public decimal covid2 { get; set; }
        public decimal covid3 { get; set; }
        public decimal covid4 { get; set; }
        public decimal covid5 { get; set; }
        public decimal covid6 { get; set; }
        public decimal covid7 { get; set; }
        public string? chanelname { get; set; }
        public string? cashcode { get; set; }
        public string? cashname { get; set; }
    }
}
