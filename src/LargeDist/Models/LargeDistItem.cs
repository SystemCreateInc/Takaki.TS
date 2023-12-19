using DbLib.Defs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LargeDist.Models
{
    public class LargeDistItem
    {
        // UpdateQuantityでコンストラクタで値を入れているが、ワーニングが出るのでプラグマで消した
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public LargeDistItem(LargeDistGroup group, IEnumerable<DistItem> items)
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        {
            Group = group;
            Items = items;
            ScanOrder = items.First().ScanOrder;
            UpdateQuantity();
        }

        private void UpdateQuantity()
        {
            Order = new BoxedQuantity(NuBoxUnit, Items.Sum(x => x.OrderPiece));
            Result = new BoxedQuantity(NuBoxUnit, Items.Sum(x => x.ResultPiece));
            Remain = new BoxedQuantity(NuBoxUnit, Items.Sum(x => x.RemainPiece));
            Input = new BoxedQuantity(NuBoxUnit, Items.Sum(x => x.InputPiece));
        }

        public LargeDistGroup Group { get; set; }
        public IEnumerable<DistItem> Items { get; private set; }

        public int? ScanOrder { get; set; }

        public Status Status =>
            Result.Total == Order.Total ? Status.Completed
            : Result.Total == 0 ? Status.Ready : Status.Inprog;

        public Status DistStatus
        {
            get
            {
                var max = Items.Max(x => x.FgDStatus);
                var min = Items.Min(x => x.FgDStatus);
                return max == min ? max : Status.Inprog;
            }
        }

        public string LargeDistStatusText => GetStatusText(Status);

        public string DistStatusText => GetStatusText(DistStatus);

        // 大仕分数量
        public BoxedQuantity Order { get; private set; }
        public BoxedQuantity Result { get; private set; }
        public BoxedQuantity Remain { get; private set; }
        public BoxedQuantity Input { get; private set; }

        // 仕分数量
        public int DistOrderTotalPieceCount => Items.Sum(x => x.DistOrderPiece);
        public int DistResultTotalPieceCount => Items.Sum(x => x.DistResultPiece);

        public string? CdBlock => Items.First().CdBlock;
        public string CdHimban => Items.First().CdHimban;
        public string CdGtin13 => Items.First().CdGtin13;
        public string? NmHinSeishikimei => Items.First().NmHinSeishikimei;
        public int StBoxType => Items.First().StBoxType;
        public int NuBoxUnit => Items.First().NuBoxUnit;
        public string CdJuchuBin => Items.First().CdJuchuBin;
        public string? CdDistGroup => Items.First().CdDistGroup;
        public string? NmDistGroup => Items.First().NmDistGroup;
        public string CdShukkaBatch => Items.First().CdShukkaBatch;
        public string? NmShukkaBatch => Items.First().NmShukkaBatch;

        public int GridPosition { get; set; }

        public void SetStopped(bool stopped)
        {
            foreach (var item in Items)
            {
                item.IsStopped = stopped;
            }
        }

        public LargeDistItemKey GroupKey
        {
            get
            {
                var item = Items.First();
                return new LargeDistItemKey(Group.CdLargeGroup, item.CdHimban, item.CdJuchuBin, item.CdShukkaBatch);
            }
        }

        public IEnumerable<LargeDistCustomerItem> CustomerItems =>
            Items.GroupBy(x => new LargeDistCustomerKey(x.Address, x.CdCourse, x.CdRoute, x.CdTokuisaki),
                (key, value) => new LargeDistCustomerItem(key, value))
            .OrderBy(x => x.Address)
            .ThenBy(x => x.CdCourse)
            .ThenBy(x => x.CdRoute)
            .ThenBy(x => x.CdTokuisaki);

        public static bool operator==(LargeDistItem? lhs, LargeDistItem? rhs) => lhs?.GroupKey == rhs?.GroupKey;

        public static bool operator!=(LargeDistItem? lhs, LargeDistItem? rhs) => lhs?.GroupKey != rhs?.GroupKey;

        public override bool Equals(object? obj)
        {
            return this == (obj as LargeDistItem);
        }

        public override int GetHashCode() => GroupKey.GetHashCode();

        private string GetStatusText(Status status)
        {
            switch (status)
            {
                case Status.Completed:
                    return "配布済";

                case Status.Ready:
                    return "未配布";

                case Status.Inprog:
                    return "配途中";

                default:
                    return "不明";
            }

        }

        public void SetInputTotalPiece(int qty)
        {
            AssignInputPiece(qty);
            Input = new BoxedQuantity(NuBoxUnit, Items.Sum(x => x.InputPiece));
        }

        public void SetBoxUnit(int unit)
        {
            foreach (var item in Items)
            {
                item.NuBoxUnit = unit;
            }

            UpdateQuantity();
        }


        private void AssignInputPiece(int qty)
        {
            foreach (var item in Items.OrderBy(x => x.Address))
            {
                item.InputPiece = Math.Min(qty, item.RemainPiece);
                qty -= item.InputPiece;
            }
        }

        public void Undo()
        {
            foreach (var item in Items)
            {
                item.Undo();
            }
        }
    }
}
