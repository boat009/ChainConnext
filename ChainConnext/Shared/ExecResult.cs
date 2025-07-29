using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared
{
    public class ExecResult
    {
        public bool IsSuccess { get; set; } = false;
        public object? Data { get; set; }
        public string? JsonData { get; set; }
        public string? Msg { get; set; } = "";
        public string? ID { get; set; }
        public int Rows { get; set; } = 0;
    }
}
