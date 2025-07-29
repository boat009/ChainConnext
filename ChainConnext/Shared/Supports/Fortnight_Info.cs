using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared.Supports
{
    public class Fortnight_Info
    {
        public int Fortnight_id { get; set; }
        public int Fortnight_no { get; set; }
        public int Fortnight_year { get; set; }
        public int Fortnight_year_TH { get; set; }
        public int Fortnight_action { get; set; }
        public DateTime? Activate_date { get; set; }
        public int Status { get; set; }
        public int DepID { get; set; }
        public string? FnText { get; set; }
        public string? Fortnight_action_Text { get; set; }
    }
}
