using System;

namespace SyslogCS.Models
{
    public class ModuleInfo
    {
        public string Path { get; set; } = string.Empty;
        public string ExeName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Special { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastWriteTime { get; set; }

    }
}
