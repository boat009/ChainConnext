using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Infos
{
    public class Info_Zipcode
    {
        public int id { get; set; }
        public string? district_code { get; set; }
        public string? zipcode { get; set; }
        public int District_Id { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
