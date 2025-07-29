using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.BD
{
    public class BD_CreditTip
    {
        public long id { get; set; }
        public int Item { get; set; }
        public string? tipno { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public int ayear { get; set; } = DateTime.Now.AddYears(543).Year;
        public int periodno { get; set; } = 1;
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public bool active { get; set; }
        public DateTime? DueDate { get; set; }
        public string? LogBy { get; set; }
    }
}
