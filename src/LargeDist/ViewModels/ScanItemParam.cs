using LargeDist.Models;
using System;

namespace LargeDist.ViewModels
{
    public record ScanItemParam(DateTime DeliveryDate, Person Person, LargeDistGroup LargeDistGroup);
}