using LargeDist.Models;
using System;
using System.Collections.Generic;

namespace LargeDist.ViewModels
{
    internal record LargeDistItemListParam(DateTime DeliveryDate, LargeDistGroup LargeDistGroup, IEnumerable<LargeDistItem> LargeDistItems);
}