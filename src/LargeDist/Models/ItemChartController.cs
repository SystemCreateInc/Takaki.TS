using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeDist.Models
{
    public class ItemChartController
    {
        public ChartItem[] Items { get; private set; }

        public IEnumerable<LargeDistItem> LargeDistItems { get; private set; } = Enumerable.Empty<LargeDistItem>();

        public ChartItem? SelectedItem => Items.FirstOrDefault(x => x.IsSelected);

        public ItemChartController() 
        {
            Items = Enumerable
                .Range(0, 24)
                .Select(x => new ChartItem(x == 0))
                .ToArray();
        }

        internal void Clear()
        {
            foreach (var item in Items)
            {
                item.Item = null;
            }
        }

        public void SetItems(IEnumerable<LargeDistItem> items)
        {
            Clear();

            // 先頭に合計用のフィールドを追加する
            var totalItems = items
                .SelectMany(x => x.Items)
                .ToArray();
            Items[0].Item = new LargeDistItem(items.First().Group, totalItems);

            int i = 1;
            foreach (var item in items)
            {
                if (i >= Items.Length)
                {
                    break;
                }

                Items[i].Item = item;
                ++i;
            }

            LargeDistItems = items;
            UpdateAllItems();
        }

        public void UnselectLineAll()
        {
            foreach (var item in Items)
            {
                item.IsSelected = false;
            }
        }

        public void UpdateAllItems()
        {
            foreach (var item in Items)
            {
                item.UpdateItem();
            }
        }

        public void SetBoxUnit(int unit)
        {
            foreach (var item in LargeDistItems)
            {
                item.SetBoxUnit(unit);
            }

            UpdateAllItems();
        }
    }
}
