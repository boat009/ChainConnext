using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Supports
{
    public class NPT_Depart
    {
        public int DepID { get; set; }
        public string? DepName { get; set; }
        public string? DepOldCode { get; set; }
        public string? ChanelSale { get; set; }
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? OldCompID { get; set; }
        public bool IsProvice { get; set; }
        public string? DepCodeLogic { get; set; }
        public bool IsCheckSave { get; set; }
        public int CalDepID { get; set; }
        public bool IsAreaCode { get; set; }
        public bool IsSaleArea { get; set; }
        public string? TextSale1 { get; set; }
        public string? TextSale2 { get; set; }
        public string? TextSale3 { get; set; }
        public string? TextSale4 { get; set; }
        public string? TextSale5 { get; set; }
    }
}
