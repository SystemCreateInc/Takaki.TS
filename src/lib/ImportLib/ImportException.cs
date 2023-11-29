using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib
{
    public class ImportException : Exception
    {
        public ImportResult Result { get; private set; }

        public ImportException(string message, ImportResult result) 
            : base(message)
        { 
            Result = result;
        }
    }
}
