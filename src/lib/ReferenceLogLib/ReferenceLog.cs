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

        // 参照日指定　開始日取得
        public string GetStartDateInRange(string selectDate)
        {
            foreach(var logInfo in LogInfos)
            {
                if(IsInRange(selectDate, logInfo.StartDate, logInfo.EndDate))
                {
                    UpdateSelected(logInfo.StartDate);
                    return logInfo.StartDate;
                }
            }

            return string.Empty;
        }

        // 指定期間の重複チェック
        public string CheckWithinRange(string startDate, string endDate, string? excludeDate)
        {
            foreach (var logInfo in LogInfos)
            {
                // 指定開始日チェックを除外
                if(logInfo.StartDate == excludeDate)
                {
                    continue;
                }

                if(IsInRange(startDate, logInfo.StartDate, logInfo.EndDate)
                    || IsInRange(endDate, logInfo.StartDate, logInfo.EndDate)
                    || IsInRange(logInfo.StartDate, startDate, endDate)
                    || IsInRange(logInfo.EndDate, startDate, endDate))
                {
                    return $"{logInfo.StartDate}-{logInfo.EndDate}";
                }
            }

            return string.Empty;
        }

        private bool IsInRange(string targetDate, string startDate, string endDate)
        {
            // 開始以上(1or0) ＆ 終了以下(-1or0)
            return targetDate.CompareTo(startDate) != -1 && targetDate.CompareTo(endDate) != 1;
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
    }
}
