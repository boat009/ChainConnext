using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Reports
{
    public class Rpt_Parameter : BaseShared
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? xtodate_from { get; set; }
        public DateTime? xtodate_to { get; set; }
        public DateTime? todate1_from { get; set; }
        public DateTime? todate1_to { get; set; }
        public string? WhereCond { get; set; }
        public string? RefNo { get; set; }
        public string? ContNo { get; set; }
        public string? QueryExc { get; set; } = "";
        public string? JsonSendData { get; set; } = "";
        public string? CustomParametor1 { get; set; } = "";
        public string? CustomParametor2 { get; set; } = "";

        public string? Msg { get; set; } = "";
        public string? Data { get; set; } = "";
        public string? PdfData { get; set; } = "";
        public string? RptName { get; set; } = "";
        public object? DataList { get; set; }
        public List<string> HeaderData { get; set; } = new List<string>();
        public bool IsCashCodeData { get; set; } = false;

        public List<Rpt_CashCode> CashCodeData { get; set; } = new List<Rpt_CashCode>();
    }
}
