using DbLib;
using LargeDistLabelLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LargeDist.Models
{
    public class ItemLargeDistController
    {
        private LargeDistGroup _group;
        private ScanGridController _rootGridController;
        private int _currentItemIndex;
        private ScopeLogger _logger = new ScopeLogger(nameof(BlockLargeDistController));

        private LargeDistProcessingUnit[] Items { get; set; }

        public LargeDistProcessingUnit CurrentItem => Items[_currentItemIndex];

        public ItemLargeDistController(LargeDistGroup group, ScanGridController gridController)
        {
            _group = group;
            _rootGridController = gridController;
            _currentItemIndex = 0;
            Items = CreateItemList();
        }

        public void SetupChartController(ItemChartController chart)
        {
            chart.SetItems(CurrentItem
                .Items
                .GroupBy(x => x.CdBlock, (key, value) => new LargeDistItem(_group, value))
                .ToArray());
        }

        private LargeDistProcessingUnit[] CreateItemList()
        {
            return _rootGridController
                .LargeDistItems
                .SelectMany(x => x.Items)
                .GroupBy(x => new ItemLargeDistKey(x.CdHimban, x.CdJuchuBin, x.CdDistGroup, x.CdShukkaBatch),
                    (key, list) => new LargeDistProcessingUnit(list))
                .ToArray();
        }

        public void MoveNext()
        {
            if (++_currentItemIndex >= Items.Length)
            {
                _currentItemIndex = 0;
            }
        }

        public void MovePrev()
        {
            if (--_currentItemIndex < 0)
            {
                _currentItemIndex = Items.Length - 1;
            }
        }

        public void SaveCurrentItem(Person person)
        {
            CurrentItem.SaveItems(person);
        }

        public bool IsCompleted()
        {
            return Items.All(x => x.IsCompleted);
        }

        public IEnumerable<LargeDistLabel> GetLabelForCurrentItem()
        {
            return CurrentItem.Items
                .GroupBy(x => x.CdBlock)
                .OrderBy(x => x.Key)
                .Select(x =>
                {
                    var item = x.First();
                    var input = new BoxedQuantity(item.NuBoxUnit, x.Sum(x => x.LastInputPiece));
                    return new LargeDistLabel
                    {
                        DtDelivery = item.DtDelivery,
                        CdJuchuBin = item.CdJuchuBin,
                        CdBlock = item.CdBlock ?? "",
                        CdDistGroup = item.CdDistGroup ?? "",
                        NmDistGroup = item.NmDistGroup ?? "",
                        CdShukkaBatch = item.CdShukkaBatch,
                        NmShukkaBatch = item.NmShukkaBatch ?? "",
                        CdHimban = item.CdHimban,
                        CdJan = item.CdGtin13,
                        NmHinSeishikimei = item.NmHinSeishikimei ?? "",
                        NuBoxUnit = item.NuBoxUnit,
                        BoxPs = input.Box,
                        BaraPs = input.Piece,
                        TotalPs = input.Total,
                        Barcode = item.CdGtin13,
                    };
                })
                .ToArray();
        }
    }
}