using DbLib.Extensions;
using Prism.Mvvm;

namespace DistProg.Models
{
    public class DistProg : BindableBase
    {
        // エリア
        private int? _idPc;
        public int? IdPc
        {
            get => _idPc;
            set => SetProperty(ref _idPc, value);
        }

        // ブロック
        private string? _cdBlock;
        public string? CdBlock
        {
            get => _cdBlock;
            set => SetProperty(ref _cdBlock, value);
        }

        // 担当者
        private string? _nmShain;
        public string? NmShain
        {
            get => _nmShain;
            set => SetProperty(ref _nmShain, value);
        }

        // 仕分グループコード
        private string? _cdDistGroup;
        public string? CdDistGroup
        {
            get => _cdDistGroup;
            set => SetProperty(ref _cdDistGroup, value);
        }

        // 仕分グループ名
        private string? _nmDistGroup;
        public string? NmDistGroup
        {
            get => _nmDistGroup;
            set => SetProperty(ref _nmDistGroup, value);
        }

        // 仕分グループ(表示用)
        public string DistGroupInfo => CdDistGroup + " " + NmDistGroup;

        // 納品日(8文字)
        private string _dtDelivery = string.Empty;
        public string DtDelivery
        {
            get => _dtDelivery;
            set => SetProperty(ref _dtDelivery, value);
        }

        // 納品日(表示用)
        public string DispDtDelivery => DtDelivery.GetDate();

        // 仕分開始
        private DateTime? _dtStart;
        public DateTime? DtStart
        {
            get => _dtStart;
            set => SetProperty(ref _dtStart, value);
        }

        // 仕分終了

        private DateTime? _dtEnd;
        public DateTime? DtEnd
        {
            get => _dtEnd;
            set => SetProperty(ref _dtEnd, value);
        }

        // 経過時間(計算)
        public TimeSpan? ElapsedTime
        {
            get
            {
                if (DtStart == null)
                {
                    return null;
                }

                // 終了時刻がnullの場合は、現在時刻から計算
                if (DtEnd == null)
                {
                    return DateTime.Now - DtStart;
                }

                return DtEnd - DtStart;
            }
        }

        // アイテム数(済)
        private int? _nuRitemcnt;
        public int? NuRitemcnt
        {
            get => _nuRitemcnt;
            set => SetProperty(ref _nuRitemcnt, value);
        }

        // アイテム数(予定)
        private int? _nuOitemcnt;
        public int? NuOitemcnt
        {
            get => _nuOitemcnt;
            set => SetProperty(ref _nuOitemcnt, value);
        }

        // 仕分個数(済)
        private int? _nuRps;
        public int? NuRps
        {
            get => _nuRps;
            set => SetProperty(ref _nuRps, value);
        }

        // 仕分個数(予定)
        private int? _nuOps;
        public int? NuOps
        {
            get => _nuOps;
            set => SetProperty(ref _nuOps, value);
        }

        // 個数進捗率
        public decimal? NuProg => NuRps == null || NuOps == null || NuOps == 0 ? null : (decimal)NuRps / (decimal)NuOps;
    }
}
