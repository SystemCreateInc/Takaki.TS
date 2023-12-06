using Customer.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReferenceLogLib
{
    public class ReferenceLog : BindableBase
    {
        // �J�n�`�I�������X�g
        private List<LogInfo> _logInfos = new List<LogInfo>();
        public List<LogInfo> LogInfos
        {
            get => _logInfos;
            set => SetProperty(ref _logInfos, value);
        }

        // �Q�Ɠ��w��@�J�n���擾
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

        // �w����Ԃ̏d���`�F�b�N
        public string CheckWithinRange(string startDate, string endDate, string? excludeDate)
        {
            foreach (var logInfo in LogInfos)
            {
                // �w��J�n���`�F�b�N�����O
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
            // �J�n�ȏ�(1or0) �� �I���ȉ�(-1or0)
            return targetDate.CompareTo(startDate) != -1 && targetDate.CompareTo(endDate) != 1;
        }

        // �I����ԍX�V
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
