using DbLib.Defs;
using DistGroup.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace DistGroup.ViewModels
{
    public class InputDistGroupDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand Clear { get; }
        public DelegateCommand Register { get; }
        public DelegateCommand Back { get; }
        public DelegateCommand Refer { get; }
        public DelegateCommand Release { get; }
        public DelegateCommand UpReplace { get; }
        public DelegateCommand DownReplace { get; }
        public DelegateCommand Add { get; }
        public DelegateCommand Delete { get; }

        public string Title => "仕分グループ情報入力";

        public event Action<IDialogResult>? RequestClose;
        private Models.DistGroup _distGroup = new Models.DistGroup();

        // 参照日
        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        // 拠点コード
        private string _cdKyoten = string.Empty;
        public string CdKyoten
        {
            get => _cdKyoten;
            set => SetProperty(ref _cdKyoten, value);
        }

        // 拠点名称
        private string _nmKyoten = string.Empty;
        public string NmKyoten
        {
            get => _nmKyoten;
            set => SetProperty(ref _nmKyoten, value);
        }

        // 仕分グループコード
        private string _cdDistGroup = string.Empty;
        public string CdDistGroup
        {
            get => _cdDistGroup;
            set => SetProperty(ref _cdDistGroup, value);
        }

        // 仕分グループ名称
        private string _nmDistGroup = string.Empty;
        public string NmDistGroup
        {
            get => _nmDistGroup;
            set => SetProperty(ref _nmDistGroup, value);
        }

        // 配送便集計
        private BinSumType _binSumType = BinSumType.No;
        public BinSumType BinSumType
        {
            get => _binSumType;
            set => SetProperty(ref _binSumType, value);
        }

        // 適用開始日
        private DateTime _dtTekiyoKaishi;
        public DateTime DtTekiyoKaishi
        {
            get => _dtTekiyoKaishi;
            set => SetProperty(ref _dtTekiyoKaishi, value);
        }

        // 適用無効日
        private DateTime _dtTekiyoMuko;
        public DateTime DtTekiyoMuko
        {
            get => _dtTekiyoMuko;
            set => SetProperty(ref _dtTekiyoMuko, value);
        }

        // 登録日時
        private DateTime _dtTorokuNichiji;
        public DateTime DtTorokuNichiji
        {
            get => _dtTorokuNichiji;
            set => SetProperty(ref _dtTorokuNichiji, value);
        }

        // 更新日時
        private DateTime _dtKoshinNichiji;
        public DateTime DtKoshinNichiji
        {
            get => _dtTorokuNichiji;
            set => SetProperty(ref _dtKoshinNichiji, value);
        }

        // 更新者コード
        private string _cdShain = string.Empty;
        public string CdShain
        {
            get => _cdShain;
            set => SetProperty(ref _cdShain, value);
        }

        // 更新者名称
        private string _nmShain = string.Empty;
        public string NmShain
        {
            get => _nmShain;
            set => SetProperty(ref _nmShain, value);
        }

        // 対象出荷バッチリスト
        private List<Batch> _batches = new List<Batch>();
        public List<Batch> Batches
        {
            get => _batches;
            set => SetProperty(ref _batches, value);
        }

        // 履歴表示リスト
        private List<Log> _logs = new List<Log>();
        public List<Log> Logs
        {
            get => _logs;
            set => SetProperty(ref _logs, value);
        }

        // コース順リスト
        private List<Course> _courses = new List<Course>();
        public List<Course> Courses
        {
            get => _courses;
            set => SetProperty(ref _courses, value);
        }

        public InputDistGroupDlgViewModel()
        {
            Clear = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Clear");
                // fixme:クリアボタン押下
            });

            Register = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Register");
                // fixme:登録ボタン押下
            });

            Back = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Back");
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });

            Refer = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Refer");
                // fixme:参照ボタン押下
            });

            Release = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Release");
                // fixme:解除ボタン押下
            });

            UpReplace = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:UpReplace");
                // fixme:▲ボタン押下
            });

            DownReplace = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:DownReplace");
                // fixme:▼ボタン押下
            });

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Add");
                // fixme:追加ボタン押下
            });

            Delete = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Delete");
                // fixme:削除ボタン押下
            });
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _distGroup = parameters.GetValue<Models.DistGroup>("DistGroup");
            InitDialog();
        }

        private void InitDialog()
        {
            // 項目欄確認
            Date = DateTime.Today;
            CdKyoten = "4201";
            NmKyoten = "広島工場製品出荷";
            CdDistGroup = "02001";
            NmDistGroup = "広島常温1便(通常)";
            DtTekiyoKaishi = new DateTime(2023, 10, 1);
            DtTekiyoMuko = new DateTime(2023, 12, 31);
            DtTorokuNichiji = new DateTime(2023, 10, 11, 12, 34, 56);
            DtKoshinNichiji = new DateTime(2023, 10, 11, 12, 34, 56);
            CdShain = "0033550";
            NmShain = "小田 賢行";
            Batches = new List<Batch>
            {
                new Batch
                {
                    CdShukkaBatch = "02001",
                    NmShukkaBatch = "広島常温1便",
                    CdLargeGroup = "001",
                    CdLargeGroupName = "大仕分名称--"
                }
            };

            Logs = new List<Log>
            {
                new Log { Selected = false, DtTekiyoKaishi = "20230901", DtTekiyoMuko = "20231001", CdShain = "0033550", },
                new Log { Selected = true, DtTekiyoKaishi = "20231001", DtTekiyoMuko = "20231231", CdShain = "0033550", },
            };

            Courses = new List<Course>
            {
                new Course { NuCourseSeq = 1, CdCourse = "005" },
                new Course { NuCourseSeq = 2, CdCourse = "006" },
                new Course { NuCourseSeq = 3, CdCourse = "007" },
                new Course { NuCourseSeq = 4, CdCourse = "008" },
                new Course { NuCourseSeq = 5, CdCourse = "009" },
                new Course { NuCourseSeq = 6, CdCourse = "010" },
                new Course { NuCourseSeq = 7, CdCourse = "001" },
                new Course { NuCourseSeq = 8, CdCourse = "011" },
                new Course { NuCourseSeq = 9, CdCourse = "" },
                new Course { NuCourseSeq = 10, CdCourse = "012" },
                new Course { NuCourseSeq = 11, CdCourse = "002" },
                new Course { NuCourseSeq = 12, CdCourse = "003" },
                new Course { NuCourseSeq = 13, CdCourse = "004" },
                new Course { NuCourseSeq = 14, CdCourse = "013" },
                new Course { NuCourseSeq = 15, CdCourse = "014" },
                new Course { NuCourseSeq = 16, CdCourse = "015" },
                new Course { NuCourseSeq = 17, CdCourse = "016" },
                new Course { NuCourseSeq = 18, CdCourse = "" },
            };
        }
    }
}
