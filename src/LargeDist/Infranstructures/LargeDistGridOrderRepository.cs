using DbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeDist.Infranstructures
{
    public static class LargeDistGridOrderRepository
    {
        private const string KEY = "LargeDistGridOrder";

        public static int?[] Get()
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                var text = new Settings(tr).Get(KEY, "", Environment.MachineName);
                var ar = text
                    .Split(",")
                    .Select(x => (int?)(int.TryParse(x, out var value) ? value : null))
                    .ToArray();

                tr.Commit();

                if (ar.Length != 18)
                {
                    // デフォルト
                    ar = Enumerable.Range(1, 18).Select(x => x).Cast<int?>().ToArray();
                }

                return ar;
            }
        }

        public static void Save(IEnumerable<int?> indexes)
        {
            var gridOrders = string.Join(",", indexes.Select(x => x.ToString()));
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                new Settings(tr).Set(KEY, gridOrders, Environment.MachineName);
                tr.Commit();
            }
        }
    }
}
