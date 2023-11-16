using BackendLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorLib
{
    public interface IProcessorAdapter
    {
        string Capability { get; set; }

        string? Invoke(string[] ar, MessageHandler handler);
    }
}
