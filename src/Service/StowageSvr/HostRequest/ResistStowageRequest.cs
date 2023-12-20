namespace StowageSvr.HostRequest
{
    public class ResistStowageRequest
    {
        public string Block { get; set; } = string.Empty;
        public string DistGroup { get; set; } = string.Empty;
        public string CountCustomer { get; set; } = string.Empty;
        public string CountDist { get; set; } = string.Empty;
        public string TdCode { get; set; } = string.Empty;

        public bool IsQuantityChange { get; set; }
        public int ThickBoxPs { get; set; }
        public int WeakBoxPs { get; set; }
        public int OtherPs { get; set; }
        public int BlueBoxPs { get; set; }
    }
}
