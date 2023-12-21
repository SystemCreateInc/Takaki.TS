namespace StowageSvr.HostRequest
{
    public class ResistStowageRequest
    {
        public IEnumerable<long> StowageIds { get; set; } = Enumerable.Empty<long>();
        public bool IsQuantityChange { get; set; }
        public int LargeBoxPs { get; set; }
        public int SmallBoxPs { get; set; }
        public int OtherPs { get; set; }
        public int BlueBoxPs { get; set; }
    }
}
