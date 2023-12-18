using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakakiLib.Models;

namespace DistGroup.Models
{
    public class LargeDist
    {
        public string CdLargeGroup { get; set; } = string.Empty;
        public string NmLargeGroup => NameLoader.GetLargeGroup(CdLargeGroup);

        // 内部結合Key
        public long IdDistGroup { get; set; }

        // リスト順
        public int Sequence { get; set; }
    }
}
