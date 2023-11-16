using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib
{
    public interface ISettings
    {
        void Set<T>(string key, T data);
        string Get(string key, string defvalue = "");
        int GetInt(string key, int defvalue = 0);
    }
}
