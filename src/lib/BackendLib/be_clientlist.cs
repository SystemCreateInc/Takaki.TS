using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct be_clientlist
    {
        public short count;
        public be_regist[]    client;
    };
}
