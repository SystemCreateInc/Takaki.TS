using ExportLib;
using Prism.Mvvm;
using System;
using System.Linq;

namespace ExportBase.Models
{
    public class ProcessorInfo : BindableBase
    {
        public IExportProcessor Processor { get; set; }

        public ProcessorInfo(IExportProcessor processor)
        {
            Processor = processor;
            Name = Processor.Name;
            FileName = Processor.FileName;
            ExportIntervalMin = Processor.EnableInterval ? Processor.IntervalSec / 60 : null;
            NextExportTime = Processor.NextExportTime;
            ExportTimings = Processor.EnableTiming ? string.Join(",", Processor.SpecifiedTimings.Select(x => x.ToString(@"hh\:mm"))) : "";
            AvailableCount = Processor.AvailableExportCount;
        }

        private bool _IsSelectable;
        public bool IsSelectable
        {
            get => _IsSelectable;
            set => SetProperty(ref _IsSelectable, value);
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (!IsSelectable)
                {
                    value = false;
                }
                SetProperty(ref _IsSelected, value);
            }
        }

        private string _Name = string.Empty;
        public string Name
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
        }

        private string _FileName = string.Empty;
        public string FileName
        {
            get => _FileName;
            set => SetProperty(ref _FileName, value);
        }

        private int _AvailableCount;
        public int AvailableCount
        {
            get => _AvailableCount;
            set
            {
                SetProperty(ref _AvailableCount, value);
                IsSelectable = _AvailableCount > 0;
            }
        }

        private DateTime? _NextExportTime;
        public DateTime? NextExportTime
        {
            get => _NextExportTime;
            set => SetProperty(ref _NextExportTime, value);
        }

        private int? _ExportIntervalMin;
        public int? ExportIntervalMin
        {
            get => _ExportIntervalMin;
            set => SetProperty(ref _ExportIntervalMin, value);
        }

        private int? _ExportDelayMin;
        public int? ExportDelayMin
        {
            get => _ExportDelayMin;
            set => SetProperty(ref _ExportDelayMin, value);
        }

        private string _ExportTimings = string.Empty;
        public string ExportTimings
        {
            get => _ExportTimings;
            set => SetProperty(ref _ExportTimings, value);
        }
    }
}
