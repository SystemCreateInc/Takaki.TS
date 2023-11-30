using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib.Models
{
    public class TargetImportFile
    {
        public string ImportFilePath { get; set; } = string.Empty;

        public long? ImportFileSize { get; set; }

        public DateTime? ImportFileLastWriteDateTime { get; set; }

        public bool Selected = false;
    }
}
