using System.Collections.Generic;

namespace ProcessorLib.Models
{
    public class LabelRequest
    {
        public string Address { get; set; } = string.Empty;
        public IEnumerable<string> Datas { get; set; } = Array.Empty<string>();

        public int LabelType { get; set; }
    }
}
