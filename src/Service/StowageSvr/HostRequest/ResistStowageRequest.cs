namespace StowageSvr.HostRequest
{
    public class ResistStowageRequest
    {
        public IEnumerable<long> StowageIds { get; set; } = Enumerable.Empty<long>();
        public int LargeBoxPs { get; set; }
        public int SmallBoxPs { get; set; }
        public int OtherPs { get; set; }
        public int BlueBoxPs { get; set; }
        public string Person { get; set; } = string.Empty;
        public string PersonName { get; set; } = string.Empty;
    }
}
