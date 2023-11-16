using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchBoxLib
{
    public class SearchQueryParam
    {
        internal string? WhereSql = null;
        internal DynamicParameters prms = new DynamicParameters();

        public string GetSearchWhere(string where = "")
        {
            if (string.IsNullOrEmpty(WhereSql))
            {
                return where;
            }

            if (string.IsNullOrEmpty(where))
            {
                return WhereSql;
            }
            else
            {
                return $"{where} and {WhereSql}";
            }
        }

        public DynamicParameters GetSearchParameters()
        {
            return prms.ParameterNames.Any() ? prms : new DynamicParameters();
        }

        // 最終検索パラメータのIndex
        public int NextParameterIndex
        { 
            get
            {
                if(int.TryParse(prms.ParameterNames.LastOrDefault(x => x.Contains("text"))?.Replace("text", string.Empty), out int index))
                {
                    return index + 1;
                }
                else
                {
                    return 0;
                }
            }
        } 
    }
}
