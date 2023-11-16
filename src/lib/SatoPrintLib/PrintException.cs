namespace SatoPrintLib
{
    public class PrintException : Exception
    {
        public PrintException(string message, Exception e) : base(message, e) { }
    }
}
