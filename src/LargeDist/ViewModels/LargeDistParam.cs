using LargeDist.Models;
using System;

namespace LargeDist.ViewModels
{
    internal record LargeDistParam(DateTime DeliveryDate, Person Person, LargeDistGroup LargeDistGroup, ScanGridController ScanGrid);
}