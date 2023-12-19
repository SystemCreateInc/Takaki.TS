using LargeDist.Models;
using System.Collections.Generic;

namespace LargeDist.ViewModels
{
    public record SelectItemDialogParam(IEnumerable<LargeDistItem> Items);
}