using DbLib;
using LargeDist.Models;
using System;
using System.Collections.ObjectModel;

namespace LargeDist.Infranstructures
{
    internal class LargeGroupQueryService
    {
        internal static ObservableCollection<LargeDistGroup> GetAll(DateTime deliveryDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                throw new NotImplementedException();
            }
        }
    }
}