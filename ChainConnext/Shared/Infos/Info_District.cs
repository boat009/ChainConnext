using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Infos
{
    public class Info_District
    {
        public int District_Id { get; set; }
        public string? District_Code { get; set; }
        public string? District_Name { get; set; }
        public int Amphur_Id { get; set; }
        public int Province_Id { get; set; }
        public int Geo_Id { get; set; }
        public string? Province_Code { get; set; }
        public string? Amphur_Code { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int District_IdBH { get; set; }
    }

}
