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

        // �w������܂ޔ͈͂̊J�n���擾
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

        // �K�p���ԃ`�F�b�N
        public bool ValidateSummaryDate(DateTime startDate, DateTime endDate, bool isUpdate)
        {
            if (startDate < DateTime.Today && !isUpdate)
            {
                throw new Exception("�K�p�J�n�����ߋ����ł�");
            }

            if (startDate > endDate)
            {
                throw new Exception("�K�p�J�n�����K�p���������ߋ����ł�");
            }

            if (startDate == endDate)
            {
                throw new Exception("�K�p�J�n���Ɩ������������ł�");
            }

            // �X�V�� �X�V�Ώۂ̓K�p�J�n�����r���珜�O
            var excludeDate = isUpdate ? startDate.ToString("yyyyMMdd") : null;
            var duplicationRangeDate = GetDuplicationRange(startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"), excludeDate);

            if (!string.IsNullOrEmpty(duplicationRangeDate))
            {
                throw new Exception($"���L�̓K�p���ԂƏd�����Ă��܂�\n\n�K�p�J�n��-�K�p������\n�u{duplicationRangeDate}�v");
            }

            return true;
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

        // �d���͈͎擾
        private string GetDuplicationRange(string startDate, string endDate, string? excludeDate)
        {
            foreach (var logInfo in LogInfos)
            {
                // �w��J�n���`�F�b�N�����O
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
            // �J�n�ȏ�(1or0) �� ����������(-1)
            return targetDate.CompareTo(startDate) != -1 && targetDate.CompareTo(invalidDate) == -1;
        }

        // �������Ώ� �͈͔���
        private bool IsInRangeInvalidDate(string targetInvalidDate, string startDate, string invalidDate)
        {
            // �J�n����(1) �� �I���ȉ�(-1or0)
            return targetInvalidDate.CompareTo(startDate) == 1 && targetInvalidDate.CompareTo(invalidDate) != 1;
        }
    }
}
