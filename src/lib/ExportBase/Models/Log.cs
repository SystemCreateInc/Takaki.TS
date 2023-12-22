using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportBase.Models
{
    public record Log
    (
        DateTime? ExportDate,
        string Status,
        string FileName,
        int Count,
        string? Terminal,
        string Comment
    );
}
