using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BD_ChgModel : BaseShared
    {
        public string? ContNO { get; set; }
        public DateTime? DocDate { get; set; }
        public string? SerialNo { get; set; }
        public string? Model { get; set; }
        public string? Serder { get; set; }
        public byte[]? Remark { get; set; }
        public string? OldSerialNo { get; set; }
        public string? OldModel { get; set; }
        public string? SModel { get; set; }
        public string? SMth { get; set; }
        public string? SYear { get; set; }
        public string? SRun { get; set; }
        public string? OldSModel { get; set; }
        public string? OldSMth { get; set; }
        public string? OldSYear { get; set; }
        public string? OldSRun { get; set; }
        public string? RefNo { get; set; }
        public string? RemarkText { get; set; }
    }
}
