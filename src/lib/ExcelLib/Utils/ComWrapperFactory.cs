using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelLib.Utils
{
    internal static class ComWrapperFactory
    {
        public static ComWrapper<T> Create<T>(T obj)
            where T : class
        {
            return new ComWrapper<T>(obj);
        }
    }
}
