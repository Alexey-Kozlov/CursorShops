using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursorShops.Models
{
    public class CounterModel
    {
        public string ID { get; set; }
        public string CounterName { get; set; }
        public string Period { get; set; }
        public string LastCounter { get; set; }
        public string NowCounter { get; set; }
        public string Difference { get; set; }
        public string RowNumber { get; set; }
    }
}
