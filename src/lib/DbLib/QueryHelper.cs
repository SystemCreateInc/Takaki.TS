using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib
{
    public class QueryHelper
    {
        public static string SearchFields(IEnumerable<string> fields, string searchText, DynamicParameters prms, bool withLeadingAnd = true, int nextIndex = 0)
        {
            var result = string.Join(" or ", searchText.Split(new char[] { ' ', '　' })
                .Where(x => x.Length > 0)
                .Select((text, idx) =>
                {
                    var paramName = $"text{idx + nextIndex}";
                    prms.Add(paramName, text);
                    return $"({string.Join(" or ", fields.Select(fld => $"{fld} like '%'+@{paramName}+'%'"))})";
                }).ToList());

            if (result == "")
                return "";

            return withLeadingAnd ? $" and ({result})" : $"({result})";
        }
    }
}
