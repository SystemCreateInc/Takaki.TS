using DryIoc;
using LargeDist.Infranstructures;
using LargeDist.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeDist.Models
{
    public class ScanGridController
    {
        public ScanSlotItem[] Items { get; }

        public ScanSlotItem[] CustomOrderItems { get; private set; }

        public bool HasScanItem => Items.Any(x => x.Item != null);

        public bool DisableHeading { get; set; }

        public IEnumerable<DistItem> DistItems => Items
                .Where(x => x.Item is not null)
                .SelectMany(x => x.Item!.Items)
                .ToArray();

        public IEnumerable<LargeDistItem> LargeDistItems => Items
                .Where(x => x.Item is not null)
                .Select(x => 
                {
                    // グリッドの位置とスキャン順番をセット
                    x.Item!.GridPosition = x.GridPosition;
                    x.Item!.ScanOrder = x.ScanOrder;
                    return x.Item;
                })
                .OrderBy(x => x.ScanOrder)
                .ToArray();

        public ScanSlotItem? SelectedItem => Items.FirstOrDefault(x => x.IsSelected);

        public ScanGridController()
        {
            Items = Enumerable
                .Range(0, 18)
                .Select(x => new ScanSlotItem(x))
                .ToArray();

            CustomOrderItems = Items
                .Select((x, idx) =>
                {
                    x.ScanOrder = idx + 1;
                    return x;
                })
                .ToArray();
        }

        public void UnselectSlotAll()
        {
            foreach (var item in Items.Where(x => x.IsSelected))
            {
                item.IsSelected = false;
            }
        }

        public void SetNextHeadSlot()
        {
            if (DisableHeading)
            {
                return;
            }

            foreach (var item in Items.Where(x => x.IsHead))
            {
                item.IsHead = false;
            }

            foreach (var item in CustomOrderItems)
            {
                if (item.IsEmpty)
                {
                    item.IsHead = true;
                    break;
                }
            }
        }

        public bool IsPushedItem(LargeDistItem item)
        {
            return Items
                .Where(x => x.Item is not null)
                .Any(x => x.Item == item);
        }

        public void PushItem(string cdLargeGroup, LargeDistItem item)
        {
            var slot = CustomOrderItems.FirstOrDefault(x => x.IsHead);
            if (slot is null)
            {
                LargeLockRepository.Unlock(cdLargeGroup, item);
                throw new Exception("商品スキャンオーバー");
            }

            slot.Item = item;
            SetNextHeadSlot();
        }

        public void DeleteSelectedItem()
        {
            bool shift = false;

            for (int i = 0; i < CustomOrderItems.Length; ++i)
            {
                var item = CustomOrderItems[i];
                var next = i < CustomOrderItems.Length - 1 ? CustomOrderItems[i + 1] : null;

                if (!shift && item.IsSelected)
                {
                    shift = true;
                }

                if (shift)
                {
                    item.Item = next?.Item;
                }
            }

            SetNextHeadSlot();
        }

        internal void SetAt(int gridPosition, LargeDistItem item)
        {
            Items[gridPosition].Item = item;
            Items[gridPosition].ScanOrder = item.ScanOrder ?? 0;
        }

        internal void Clear()
        {
            foreach (var item in Items)
            {
                item.Item = null;
            }

            SetNextHeadSlot();
        }

        internal void SetOrder(int?[] order)
        {
            CustomOrderItems = order
                .Select((index, id) => (index, id))
                .Where(x => x.index != null)
                .OrderBy(x => x.index)
                .Select(x =>
                {
                    var slot = Items[x.id];
                    slot.ScanOrder = (int)x.index!;
                    return slot;
                })
                .ToArray();
        }
    }
}
