using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BDProModel : ProModel
    {
        public string? Des { get; set; }
        public decimal OnHand { get; set; }
        public decimal Receive { get; set; }
        public decimal Out { get; set; }
        public DateTime? Upd { get; set; }
        public int ReOrder { get; set; }
        public string? DespA { get; set; }
        public decimal UCost { get; set; }
        public decimal UCost2 { get; set; }
        public string? Unit { get; set; }
        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }
        public string? Grup { get; set; }
        public string? SupNo { get; set; }
        public decimal Price { get; set; }
        public decimal QtyBF { get; set; }
        public string? ProdNo { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Thick { get; set; }
        public decimal Weight { get; set; }
        public decimal WPrice { get; set; }
        public string? Rem { get; set; }
        public string? Shelf { get; set; }
        public string? Packing { get; set; }
        public decimal RatioA { get; set; }
        public decimal RatioB { get; set; }
        public decimal UV { get; set; }
        public decimal UR { get; set; }
        public string? InvDesc { get; set; }
        public decimal Interest { get; set; }
        public string? kind { get; set; }
        public string? kindname { get; set; }
        public string? vatdesc { get; set; }
        public string? status { get; set; }
        public DateTime? stdate { get; set; }
        public decimal rate { get; set; }
        public int formula { get; set; }
        public string? KindDesc { get; set; }
        public string? cate1 { get; set; }
        public int KindID { get; set; }
        public decimal BaseRate { get; set; }
        public string? ProductBrand { get; set; }
        public string? ProductModel { get; set; }
        public string? ProductSku { get; set; }
        public string? segment { get; set; }
        public string? ErpItemCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? ProEndDate { get; set; }
        public DateTime? ProStartDate { get; set; }
        public string? Remark { get; set; }
    }
}
