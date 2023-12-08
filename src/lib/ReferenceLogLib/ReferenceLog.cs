using Customer.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReferenceLogLib
{
    public class ReferenceLog : BindableBase
    {
        // 開始～終了日リスト
        private List<LogInfo> _logInfos = new List<LogInfo>();
        public List<LogInfo> LogInfos
        {
            get => _logInfos;
            set => SetProperty(ref _logInfos, value);
        }

        // 指定日を含む範囲の開始日取得
        public string GetStartDateInRange(string selectDate)
        {
            var startDate = string.Empty;

            foreach(var logInfo in LogInfos)
            {
                if(IsInRange(selectDate, logInfo.StartDate, logInfo.EndDate))
                {
                    startDate = logInfo.StartDate;
                    break;
                }
            }

            UpdateSelected(startDate);
            return startDate;
        }

        // 適用期間チェック
        public bool ValidateSummaryDate(DateTime startDate, DateTime endDate, bool isUpdate)
        {
            if (startDate < DateTime.Today && !isUpdate)
            {
                throw new Exception("適用開始日が過去日です");
            }

            if (startDate > endDate)
            {
                throw new Exception("適用開始日より適用無効日が過去日です");
            }

            if (startDate == endDate)
            {
                throw new Exception("適用開始日と無効日が同日です");
            }

            // 更新時 更新対象の適用開始日を比較から除外
            var excludeDate = isUpdate ? startDate.ToString("yyyyMMdd") : null;
            var duplicationRangeDate = GetDuplicationRange(startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"), excludeDate);

            if (!string.IsNullOrEmpty(duplicationRangeDate))
            {
                throw new Exception($"下記の適用期間と重複しています\n\n適用開始日-適用無効日\n「{duplicationRangeDate}」");
            }

            return true;
        }

        // 選択状態更新
        private void UpdateSelected(string tekiyokaishiDate)
        {
            LogInfos = LogInfos.Select(x => new LogInfo
            {
                Selected = x.StartDate == tekiyokaishiDate,
                ShainCode = x.ShainCode,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
            }).ToList();
        }

        // 重複範囲取得
        private string GetDuplicationRange(string startDate, string endDate, string? excludeDate)
        {
            foreach (var logInfo in LogInfos)
            {
                // 指定開始日チェックを除外
                if(logInfo.StartDate == excludeDate)
                {
                    continue;
                }

                if(IsInRange(startDate, logInfo.StartDate, logInfo.EndDate)
                    || IsInRangeInvalidDate(endDate, logInfo.StartDate, logInfo.EndDate)
                    || IsInRange(logInfo.StartDate, startDate, endDate)
                    || IsInRangeInvalidDate(logInfo.EndDate, startDate, endDate))
                {
                    return $"{logInfo.StartDate}-{logInfo.EndDate}";
                }
            }

            return string.Empty;
        }
        
        private bool IsInRange(string targetDate, string startDate, string invalidDate)
        {
            // 開始以上(1or0) ＆ 無効日未満(-1)
            return targetDate.CompareTo(startDate) != -1 && targetDate.CompareTo(invalidDate) == -1;
        }

        // 無効日対象 範囲判定
        private bool IsInRangeInvalidDate(string targetInvalidDate, string startDate, string invalidDate)
        {
            // 開始より上(1) ＆ 終了以下(-1or0)
            return targetInvalidDate.CompareTo(startDate) == 1 && targetInvalidDate.CompareTo(invalidDate) != 1;
        }
    }
}
