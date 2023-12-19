﻿using DbLib;
using System.Linq;

namespace LargeDist.Models
{
    public class ItemLargeDistController
    {
        private LargeDistGroup _group;
        private ScanGridController _rootGridController;
        private int _currentItemIndex;
        private ScopeLogger _logger = new ScopeLogger(nameof(BlockLargeDistController));

        public ItemLargeDistItem[] Items { get; private set; }

        public ItemLargeDistItem CurrentItem => Items[_currentItemIndex];

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

        private ItemLargeDistItem[] CreateItemList()
        {
            return _rootGridController
                .LargeDistItems
                .SelectMany(x => x.Items)
                .GroupBy(x => new ItemLargeDistKey(x.CdHimban, x.CdJuchuBin, x.CdDistGroup, x.CdShukkaBatch),
                    (key, list) => new ItemLargeDistItem(key, list))
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
    }
}