using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchBoxLib
{
    public class SearchBoxResult
    {

        public SearchBoxResult(IEnumerable<string> fields, string text)
        {
            Fields = fields;
            Text = text;
        }

        public IEnumerable<string> Fields { get; set; }

        public string Text { get; set; }
    }
}
