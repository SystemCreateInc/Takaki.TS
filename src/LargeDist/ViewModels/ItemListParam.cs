using LargeDist.Models;
using System;

namespace LargeDist.ViewModels
{
    public record ItemListParam(DateTime DeliveryDate, Person Person, LargeDistGroup LargeDistGroup, ScanGridController gridController, ItemProgress ItemProgress);
}