using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class Branch
    {
        public string? Code { get; set; }
        public string? ComId { get; set; }
        public string? Name { get; set; }
        public string? NameEng { get; set; }
        public string? Addr1 { get; set; }
        public string? Addr2 { get; set; }
        public string? Addr3 { get; set; }
        public string? Addr4 { get; set; }
        public string? Tel { get; set; }
        public string? Fax { get; set; }
        public string? WH1 { get; set; }
        public string? WH2 { get; set; }
        public string? Post { get; set; }
        public string? Password { get; set; }
        public string? socode { get; set; }
        public string? sobranch { get; set; }
        public string? sostatus { get; set; }
        public string? soname { get; set; }
        public string? soaddr1 { get; set; }
        public string? soaddr2 { get; set; }
        public string? soamphur { get; set; }
        public string? soprovince { get; set; }
        public string? sozip { get; set; }
        public string? Region { get; set; }
        public bool Used { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Distance { get; set; }
        public string? CostCenter { get; set; }
        public string? Chanel { get; set; }
    }
}
