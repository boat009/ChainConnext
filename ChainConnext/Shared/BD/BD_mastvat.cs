using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BD_mastvat : BaseShared
    {
        public string? RefNo { get; set; }
        public string? CONTNO { get; set; }
        public string? PayPeriod { get; set; }
        public string? VatNo { get; set; }
        public int ITEM { get; set; }
        public string? PERIOD { get; set; }
        public DateTime? DOCDATE { get; set; }
        public string? CUSTNAME { get; set; }
        public decimal VAT { get; set; }
        public decimal AMOUNT { get; set; }
        public string? No { get; set; }
        public string? LName { get; set; }
        public string? PreName { get; set; }
        public string? Lock { get; set; }
        public string? Bcode { get; set; }
        public string? Ctype { get; set; }
        public string? crnote { get; set; }
        public string? accname { get; set; }
        public string? amttext { get; set; }
        public string? modelname { get; set; }
        public string? model { get; set; }
        public string? serialno { get; set; }
        public decimal total { get; set; }
        public string? status { get; set; }
        public DateTime? transdate { get; set; }
        public decimal premium { get; set; }
        public string? taxid { get; set; }
        public string? citizenid { get; set; }
    }
}
