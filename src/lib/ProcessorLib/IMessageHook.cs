using BackendLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorLib
{
    public interface IMessageHook
    {
        // trueでこれ以降処理はしない
        bool Hook(string[] ar, MessageHandler handler);
    }
}
