using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Contracts
{
    public class Contract_Info : BaseShared
    {
        public string? ContractId { get; set; }
        public string? CustomerId { get; set; }
        public string? ContractNo { get; set; }
        public string? RefNo { get; set; }
        public string? RefNoBH { get; set; }
        public int PackageId { get; set; }
        public string? CollectorCode { get; set; }
        public string? ContractSale { get; set; }
        public string? ContractStatus { get; set; }
        public DateTime? ContractStatusDate { get; set; }
        public string? ContractServiceStaff { get; set; }
        public string? ContractRefer { get; set; }
        public string? ContractDriver { get; set; }
        public string? CompCode { get; set; }
        public DateTime? EffDate { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string? BCode { get; set; }
        public string? Chanel { get; set; }
        public string? SerialNo { get; set; }
        public string? BwSerialNo { get; set; }
        public decimal Ur { get; set; }
        public decimal Uv { get; set; }
        public decimal Sales { get; set; }
        public decimal Credit { get; set; }
        public decimal ContractDiscount { get; set; }
        public decimal ContractFirstPeriodAmount { get; set; }
        public string? ContractType { get; set; }
        public int ContractPeriod { get; set; }
        public decimal ContractPeriodAmount { get; set; }
        public DateTime? ContractPayDate { get; set; }
        public int IsTurn { get; set; }
        public string? TurnProduct { get; set; }
        public string? TurnBrandCode { get; set; }
        public string? TurnProductModel { get; set; }
        public int TurnType { get; set; }
        public string? SourceType { get; set; }
        public int Fnno { get; set; }
        public int Fnyear { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsActive { get; set; }
        public DateTime? TransferDate { get; set; }
        public string? HContract { get; set; }
        public int DownType { get; set; }
        public decimal DownValue { get; set; }
        public string? ContractRemark { get; set; }
        public int CustomerRefer { get; set; }
        public string? BHCustomerID { get; set; }
        public decimal Cash { get; set; }
        public string? Model { get; set; }
        public decimal AfterDiscount { get; set; }
        public string? Brand { get; set; }
        public string? ItemCode { get; set; }
        public string? PLoanContID { get; set; }
        public decimal SmPay { get; set; }
        public int ExpPrd { get; set; }
        public decimal ExpAmt { get; set; }
        public int ExpFrm { get; set; }
        public int ExpTo { get; set; }
        public DateTime? LastPayDate { get; set; }
        public decimal LastPayAmt { get; set; }
        public string? TypeDesc { get; set; }
        public string? ModelDesc { get; set; }
        public DateTime? FirstDueDate { get; set; }
        public DateTime? LastDueDate { get; set; }

        public string? CustomerName { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public string? ChanelCode { get; set; }
        public string? ChanelName { get; set; }
        public decimal ContractFirstPay { get; set; }

        public string? SaleCode { get; set; }
        public string? SaleName { get; set; }
        public string? CashCode { get; set; }
        public string? CashName { get; set; }
        public string? ServiceCode { get; set; }
        public string? ServiceName { get; set; }
        public string? CheckerCode { get; set; }
        public string? CheckerName { get; set; }
        public string? SetupCode { get; set; }
        public string? SetupName { get; set; }
        public string? CollectorName { get; set; }

        public DateTime? DueDate { get; set; }
        public string? CaseID { get; set; }
        public string? FillterNo { get; set; }
        public string? ReturnDetail { get; set; }
        public string? TypeCard { get; set; }
        public string? HandNo { get; set; }
        public DateTime? ContractDate { get; set; }
        public int LastPayPeroid { get; set; }
        public int NextPeroid { get; set; }
        public int DueDay { get; set; }
        public DateTime? NextDueDate { get; set; }
        public DateTime? TurnDate { get; set; }
        public DateTime? ChangeDate { get; set; }

        public string? TypeCardDetail { get; set; }
        public string? CaseIDDetail { get; set; }

        public string? Mode { get; set; }

        public string? ContractNoFrom { get; set; }
        public string? RefNoFrom { get; set; }
        public string? AccStatusCode { get; set; }
        public string? AccStatusName { get; set; }
        public DateTime? AccStatusDate { get; set; }
        public string? ContractNoOld { get; set; }
        public string? RefNoOld { get; set; }
        public string? ContractIdLink { get; set; }
        public string? ToNote { get; set; }
        public string? CashCodeOld { get; set; }
        public string? CashNameOld { get; set; }
        public int Title { get; set; } = 1;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MissData { get; set; }
        public int CreditFnNo { get; set; }
        public int CreditFnYear { get; set; }

        public int PayPeroid { get; set; }
        public DateTime? PayDate { get; set; }
        public string? BookNo { get; set; }
        public string? NumNo { get; set; }
        public decimal PayAmt { get; set; }

        public decimal RmPay { get; set; }
        public DateTime? CreditDate { get; set; }

        public string? Memo { get; set; }
        public string? EditContNoRefNoType { get; set; }
    }
}
