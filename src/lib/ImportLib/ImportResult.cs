using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib
{
    public record ImportResult(bool Success, string FileName, long FileSize, long DataCount);
}
