using System;
using System.IO;

namespace SyslogCS.Models
{
    public class LogFileInfo
    {
        public StreamWriter? Writer { get; set; }
        public string LogRoot { get; set; } = string.Empty;
        public string LogPath { get; set; } = string.Empty;
        public string LogFilePath { get; set; } = string.Empty;
        public DateTime CurrentDate { get; set; } = DateTime.Now;
    }
}
