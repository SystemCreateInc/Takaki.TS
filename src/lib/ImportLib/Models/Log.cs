using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib.Models
{
    public record Log
    (
        DateTime ImportDate,
        string Status,
        string FileName,
        int Count,
        string? Terminal,
        string Comment
    );
}
