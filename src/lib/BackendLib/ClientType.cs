using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib
{
    public enum ClientType : int
    {
        BE_CLIENTTYPE_UNKNOWN = 0,
        BE_CLIENTTYPE_BACKEND = 1,
        BE_CLIENTTYPE_PROCESSOR = 2,
        BE_CLIENTTYPE_FRONTEND = 3,
    }
}
