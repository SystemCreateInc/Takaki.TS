namespace ProcessorLib.Models
{
    public class PrintLabelRequest
    {
        public string Address { get; set; } = string.Empty;
        public int Labelregno { get; set; }
        public int LabelType { get; set; }
        public bool reprint { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}
