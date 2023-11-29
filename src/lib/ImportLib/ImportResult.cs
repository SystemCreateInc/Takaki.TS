using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib
{
    public class ImportResult
    {
        public bool Success { get; init; }
        public long FileSize { get; init; }
        public int DataCount { get; init; }

        public ImportResult(bool success, long fileSize, int dataCount)
        {
            Success = success;
            FileSize = fileSize;
            DataCount = dataCount;
        }
    }
}
