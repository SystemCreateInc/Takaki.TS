using DbLib;
using LargeDist.Infranstructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeDist.Models
{
    public class BlockLargeDistController
    {
        private LargeDistGroup _group;
        private ScanGridController _rootGridController;
        private BlockLargeDistItem[] _items;
        private int _currentItemIndex;
        private ScopeLogger _logger = new ScopeLogger(nameof(BlockLargeDistController));

        public BlockLargeDistItem CurrentItem => _items[_currentItemIndex];

        public BlockLargeDistController(LargeDistGroup group, ScanGridController gridController)
        {
            _group = group;
            _rootGridController = gridController;
            _currentItemIndex = 0;
            _items = CreateBlockedItem();
        }

        public void SetupGridController(ScanGridController gridController)
        {
            foreach (var items in CurrentItem.Items.GroupBy(x => x.GridPosition))
            {
                gridController.SetAt(items.Key, new LargeDistItem(_group, items));
            }
        }

        private BlockLargeDistItem[] CreateBlockedItem()
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
                .Select(x => new BlockLargeDistItem(x.Key, x))
                .ToArray();
        }

        public void MoveNext()
        {
            if (++_currentItemIndex >= _items.Length)
            {
                _currentItemIndex = 0;
            }
        }

        public void MovePrev()
        {
            if (--_currentItemIndex < 0)
            {
                _currentItemIndex = _items.Length - 1;
            }
        }

        public void SaveCurrentItem(Person person)
        {
            CurrentItem.SaveItems(person);
        }

        internal bool IsCompleted()
        {
            return _items.All(x => x.IsCompleted);
        }
    }
}
