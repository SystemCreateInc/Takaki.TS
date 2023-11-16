using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LabelLib
{
    public class LabelBuilderParam : ILabelBuilderParam
    {
        public string Code { get; private set; }
        public bool IsVertical { get; private set; }

        public LabelBuilderParam(string code, bool isVertical = true)
        {
            Code = code;
            IsVertical = isVertical;
        }
    }
}
