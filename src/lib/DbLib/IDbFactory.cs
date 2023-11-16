using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib
{
    public interface IDbFactory
    {
        IDbConnection Create(string server="(local)");
    }
}
