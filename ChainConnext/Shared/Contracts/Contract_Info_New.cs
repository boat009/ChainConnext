using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Contracts
{
    public class Contract_Info_New : Contract_Info
    {
        public int Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CitizenId { get; set; }
        public int CardTypeId { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? CitizenExpireDate { get; set; }
        public string? RefNoNew { get; set; }
        public string? Addr1 { get; set; }
        public string? Addr2 { get; set; }
        public string? Addr3 { get; set; }
        public string? Addr4 { get; set; }
        public string? AddrZip { get; set; }
        public string? AddrReg1 { get; set; }
        public string? AddrReg2 { get; set; }
        public string? AddrReg3 { get; set; }
        public string? AddrReg4 { get; set; }
        public string? AddrRegZip { get; set; }
        public string? TelNo { get; set; }
        public string? MobileNo { get; set; }
        public string? TeamCode { get; set; }
        public string? NTDetail { get; set; }
        public string? ChanelTele { get; set; }
        public string? TypeCode { get; set; }
        public string? ModelCode { get; set; }

        public string? AddrReg21 { get; set; }
        public string? AddrReg31 { get; set; }
        public string? AddrReg41 { get; set; }
        public string? Addr21 { get; set; }
        public string? Addr31 { get; set; }
        public string? Addr41 { get; set; }
    }
}
