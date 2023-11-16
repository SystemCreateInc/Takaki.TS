using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExcelLib.Utils
{
    internal class ComWrapper<T> : IDisposable
        where T : class
    {
        private T? _obj;
        public T Obj
        {
            get => _obj!;
            private set => _obj = value;
        }

        public ComWrapper(T obj)
        {
            Obj = obj;
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject(Obj);
            _obj = null;
        }
    }
}
