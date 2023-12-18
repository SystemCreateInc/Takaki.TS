using DistBlock.Loader;
using Microsoft.IdentityModel.Tokens;
using Prism.Mvvm;

namespace DistBlock.Models
{
    public class Block : BindableBase
    {
        private int _nuBlockSeq;
        public int NuBlockSeq
        {
            get => _nuBlockSeq;
            set => SetProperty(ref _nuBlockSeq, value);
        }

        private string _cdBlock = string.Empty;
        public string CdBlock
        {
            get => _cdBlock;
            set
            {
                SetProperty(ref _cdBlock, value);
                ResetAddr();
            } 
        }

        private string _cdAddrFrom = string.Empty;
        public string CdAddrFrom
        {
            get => _cdAddrFrom;
            set => SetProperty(ref _cdAddrFrom, value);
        }

        private string _cdAddrTo = string.Empty;
        public string CdAddrTo
        {
            get => _cdAddrTo;
            set => SetProperty(ref _cdAddrTo, value);
        }

        public long DistBlockId { get; set; }

        private string _referenceDate = string.Empty;
        public string ReferenceDate
        {
            get => _referenceDate;
            set
            {
                SetProperty(ref _referenceDate, value);
                ResetAddr();
            }
        }

        private void ResetAddr()
        {
            if (CdBlock.IsNullOrEmpty() || ReferenceDate.IsNullOrEmpty())
            {
                return;
            }

            // 表示器数取得、終了No入力
            var blockCount = BlockLoader.GetBlockCount(PadBlock, ReferenceDate);
            if (blockCount is not null)
            {
                IsExistTbBlock = true;
                CdAddrFrom = "0001";
                CdAddrTo = (1 + blockCount).ToString()!.PadLeft(4, '0');
            }
        }

        // 開始が終了より小さい
        public bool IsVaridRange => !CdAddrFrom.IsNullOrEmpty() && !CdAddrTo.IsNullOrEmpty() &&
                                    PadAddrFrom.CompareTo(PadAddrTo) == -1;

        public bool IsExistTbBlock = false;

        public string PadBlock => CdBlock.PadLeft(2, '0');
        public string PadAddrFrom => CdAddrFrom.PadLeft(4, '0');
        public string PadAddrTo => CdAddrTo.PadLeft(4, '0');
    }
}
