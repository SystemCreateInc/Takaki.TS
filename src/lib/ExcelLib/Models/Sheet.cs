using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelLib.Models
{
    public class Sheet
    {
        public Sheet(int index, string name, object[][] values)
        {
            Index = index;
            Name = name;
            Values = values;
        }

        public int Index { get; set; }
        public string Name { get; set; }
        public object[][] Values { get; set; }
    }
}
