using DistBlock.Loader;
using LogLib;
using Microsoft.IdentityModel.Tokens;
using Prism.Mvvm;
using System.Text.RegularExpressions;

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
            set
            {
                // 数値以外を削除
                var numValue = Regex.Replace(value, @"[^0-9]", "");
                SetProperty(ref _cdAddrFrom, numValue);
            }
        }

        private string _cdAddrTo = string.Empty;
        public string CdAddrTo
        {
            get => _cdAddrTo;
            set
            {
                var numValue = Regex.Replace(value, @"[^0-9]", "");
                SetProperty(ref _cdAddrTo, numValue);
            } 
        }

        public long DistBlockId { get; set; }

        private string _referenceDate = string.Empty;
        public string ReferenceDate
        {
            get => _referenceDate;
            set => SetProperty(ref _referenceDate, value);
        }

        private void ResetAddr()
        {            
            if (CdBlock.IsNullOrEmpty() || ReferenceDate.IsNullOrEmpty())
            {
                return;
            }

            try
            {
                // 表示器数取得、終了No入力
                var blockCount = BlockLoader.GetBlockCount(PadBlock, ReferenceDate);
                if (blockCount is not null)
                {
                    IsExistTbBlock = true;
                    CdAddrFrom = "0001";
                    CdAddrTo = (blockCount).ToString()!.PadLeft(4, '0');
                }
                else
                {
                    IsExistTbBlock = false;
                }
            }
            catch (Exception ex)
            {
                Syslog.Debug($"GetBlockCount Error{ex.Message}");
            }            
        }

        // 開始が終了より小さい
        public bool IsVaridRange => !CdAddrFrom.IsNullOrEmpty() && !CdAddrTo.IsNullOrEmpty() &&
                                    PadAddrFrom.CompareTo(PadAddrTo) != 1;

        public bool IsExistTbBlock = true;

        public string PadBlock => CdBlock.PadLeft(2, '0');
        public string PadAddrFrom => CdAddrFrom.PadLeft(4, '0');
        public string PadAddrTo => CdAddrTo.PadLeft(4, '0');
    }
}
