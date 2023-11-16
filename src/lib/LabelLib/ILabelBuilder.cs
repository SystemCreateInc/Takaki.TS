using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelLib
{
    public interface ILabelBuilder
    {
        IEnumerable<string> Build(ILabelBuilderParam param);
    }
}
