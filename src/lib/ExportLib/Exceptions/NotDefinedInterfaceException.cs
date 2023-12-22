using System;

namespace ExportLib.Exceptions
{
    public class NotDefinedInterfaceException : Exception
    {
        public NotDefinedInterfaceException(string message)
            : base(message)
        {
        }
    }
}
