using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picking.Models
{
    public class DistItemSeq : DistBase
    {
        // 明細
        private List<DistDetail>? _details = new List<DistDetail>();
        public List<DistDetail>? Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }
    }
}
