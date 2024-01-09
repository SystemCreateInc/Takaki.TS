using DbLib;
using DryIoc;
using LargeDistLabelLib;
using System.Collections.Generic;
using System.Linq;

namespace LargeDist.Models
{
    public class BlockLargeDistController
    {
        private readonly LargeDistGroup _group;
        private readonly ScanGridController _rootGridController;
        private readonly LargeDistProcessingUnit[] _items;
        private readonly ScopeLogger _logger = new (nameof(BlockLargeDistController));
        private int _currentItemIndex;

        public LargeDistProcessingUnit CurrentItem => _items[_currentItemIndex];

        public BlockLargeDistController(LargeDistGroup group, ScanGridController gridController)
        {
            _group = group;
            _rootGridController = gridController;
            _currentItemIndex = 0;
            _items = CreateBlockedItem();
        }

        public void SetupGridController(ScanGridController gridController)
        {
            foreach (var items in CurrentItem.Items.Where(x => !x.IsCompleted).GroupBy(x => x.GridPosition))
            {
                gridController.SetAt(items.Key, new LargeDistItem(_group, items));
            }
        }

        private LargeDistProcessingUnit[] CreateBlockedItem()
        {
            return _rootGridController
                .LargeDistItems
                .SelectMany(x =>
                {
                    foreach (var item in x.Items)
                    {
                        item.GridPosition = x.GridPosition;
                        item.ScanOrder = (int)x.ScanOrder!;
                    }
                    return x.Items;
                })
                .GroupBy(x => new BlockLargeDistKey(x.CdBlock, x.CdJuchuBin, x.CdDistGroup, x.CdShukkaBatch))
                .OrderBy(x => x.Key.CdBlock)
                .ThenBy(x => x.Key.CdJuchuBin)
                .ThenBy(x => x.Key.CdDistGroup)
                .ThenBy(x => x.Key.CdShukkaBatch)
                .Select(x => new LargeDistProcessingUnit(x))
                .ToArray();
        }

        public void MoveNext()
        {
            int indexCache = _currentItemIndex;

            do
            {
                if (++_currentItemIndex >= _items.Length)
                {
                    _currentItemIndex = 0;
                }

                if (!CurrentItem.IsCompleted)
                {
                    break;
                }
            }
            while (_currentItemIndex != indexCache);
        }

        public void MovePrev()
        {
            int indexCache = _currentItemIndex;

            do
            {
                if (--_currentItemIndex < 0)
                {
                    _currentItemIndex = _items.Length - 1;
                }

                if (!CurrentItem.IsCompleted)
                {
                    break;
                }
            }
            while (_currentItemIndex != indexCache);
        }

        public void SaveCurrentItem(Person person)
        {
            CurrentItem.SaveItems(person);
        }

        public IEnumerable<LargeDistLabel> GetLabelForCurrentItem()
        {
            return CurrentItem.Items
                .GroupBy(x => x.GridPosition)
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

        internal bool IsCompleted()
        {
            return _items.All(x => x.IsCompleted);
        }
    }
}
