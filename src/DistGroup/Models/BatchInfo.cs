using LogLib;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using TakakiLib.Models;

namespace DistGroup.Models
{
    public class BatchInfo : BindableBase
    {
        private string _cdShukkaBatch = string.Empty;
        public string CdShukkaBatch
        {
            get => _cdShukkaBatch;
            set
            {
                SetProperty(ref _cdShukkaBatch, value);
                NmShukkaBatch = NameLoader.GetNmShukkaBatch(PadBatch);
                Syslog.Debug($"BatchInfo:GetNmShukkaBatch Cdbatch={CdShukkaBatch}, Name={NmShukkaBatch}");
            }            
        }

        private string _nmShukkaBatch = string.Empty;
        public string NmShukkaBatch
        {
            get => _nmShukkaBatch;
            set => SetProperty(ref _nmShukkaBatch, value);
        }   

        private string _cdLargeGroup = string.Empty;
        public string CdLargeGroup
        {
            get => _cdLargeGroup;
            set
            {
                SetProperty(ref _cdLargeGroup, value);
                NmLargeGroup = NameLoader.GetLargeGroup(PadLarge);
            }
        }

        private string _nmLargeGroup = string.Empty;
        public string NmLargeGroup
        {
            get => _nmLargeGroup;
            set => SetProperty(ref _nmLargeGroup, value);
        }
                
        // DB側サイズで0埋め(英字も入るが構わず続行)
        public string PadBatch => CdShukkaBatch.PadLeft(5, '0');
        public string PadLarge => CdLargeGroup.PadLeft(3, '0');

        // 内部結合Key
        public long IdDistGroup { get; set; }

        // リスト順
        public int Sequence { get; set; }

        public ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
    }
}
