using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursorShops.Models
{
    public class TestDI : IMyDI
    {

        public string MyProp { get; set; }

        public string SomeMthod() => "ergerge";

    }

    public interface IMyDI
    {
        string MyProp { get; set; }
        string SomeMthod();
    }
}
