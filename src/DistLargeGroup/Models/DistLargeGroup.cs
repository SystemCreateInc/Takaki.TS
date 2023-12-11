using Prism.Mvvm;

namespace DistLargeGroup.Models
{
    public class DistLargeGroup
    {
        public long IdLargeGroup { get; set; }
        public string CdKyoten { get; set; } = string.Empty;
        public string CdLargeGroup { get; set; } = string.Empty;
        public string CdLargeGroupName { get; set; } = string.Empty;
        public DateTime DtTekiyoKaishi { get; set; }
        public DateTime DtTekiyoMuko { get; set; }
        public string CdHenkosha { get; set; } = string.Empty;
        public string NmHenkosha { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
