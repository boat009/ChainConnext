using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.StockDB
{
    public class STK_ProductTurnExchange : BaseShared
    {
        public int RecordID { get; set; }
        public DateTime? RecordDate { get; set; }
        public string? CustFName { get; set; }
        public string? CustLName { get; set; }
        public string? CustName { get; set; }
        public string? NewModel { get; set; }
        public string? NewSerial { get; set; }
        public string? OldModel { get; set; }
        public string? OldSerial { get; set; }
        public string? SaleTeam { get; set; }
        public string? SaleRec { get; set; }
        public string? StockRec { get; set; }
        public string? RecStatus { get; set; }
        public string? StockId { get; set; }
        public string? BCode { get; set; }
        public string? Remark { get; set; }
        public DateTime? DocDate { get; set; }
        public string? ContNo { get; set; }
        public string? RefNo { get; set; }
        public string? SaleEmp { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }
        public string? SendStatus { get; set; }
        public DateTime? SendDate { get; set; }
        public string? SendPerson { get; set; }
        public string? ReturnDocumentNo { get; set; }
        public DateTime? ReturnDocumentDate { get; set; }
        public string? IsLock { get; set; }
        public int CheckImage { get; set; }
        public int LinkStorms { get; set; }
        public string? TurnInSticker { get; set; }
    }
}
