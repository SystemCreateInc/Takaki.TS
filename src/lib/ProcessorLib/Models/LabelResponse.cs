namespace ProcessorLib.Models
{
    public class LabelResponse
    {
        public IEnumerable<string> LabelData { get; internal set; } = Array.Empty<string>();
        public int LabelType { get; internal set; }
        public PrinterType PrinterType { get; internal set; } = PrinterType.Unknown;
    }
}
