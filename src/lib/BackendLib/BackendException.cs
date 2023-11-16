using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib
{
    public class BackendException : Exception
    {
        public BackendException(string message) : base(message)
        {

        }
    }
}
