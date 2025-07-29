using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BD_ChgCont : BaseShared
    {
        public long id { get; set; }
        public string? Item { get; set; }
        public string? RefNo { get; set; }
        public string? OldRefNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string? ChgBy { get; set; }
        public string? OldName { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public DateTime? EffDate { get; set; }
        public DateTime? OldEffDate { get; set; }
        public string? PreName { get; set; }
        public string? CONTNO { get; set; }
        public string? OLDCONTNO { get; set; }
        public string? posted { get; set; }
        public string? code { get; set; }
        public DateTime? todate { get; set; }
        public string? note { get; set; }
        public string? oldserialno { get; set; }
        public string? serialno { get; set; }
        public string? oldmode { get; set; }
        public string? mode { get; set; }
        public decimal oldcredit { get; set; }
        public decimal credit { get; set; }
        public decimal oldsales { get; set; }
        public decimal sales { get; set; }
        public decimal oldpremium { get; set; }
        public decimal premium { get; set; }
        public decimal oldfirstdisc { get; set; }
        public decimal firstdisc { get; set; }
        public string? tocode { get; set; }
        public DateTime? accdate { get; set; }
        public string? tonote { get; set; }
    }
}
