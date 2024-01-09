using DbLib.Defs;
using DbLib.Extensions;
using DistGroup.Loader;
using DistGroup.Models;
using LogLib;
using Microsoft.IdentityModel.Tokens;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using ReferenceLogLib;
using System.Collections.ObjectModel;
using TakakiLib.Models;
using WindowLib.Utils;

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
        private Models.DistGroupInfo _distGroup = new Models.DistGroupInfo();

        // 参照日
        private DateTime _referenceDate;
        public DateTime ReferenceDate
        {
            get => _referenceDate;
            set
            {
                SetProperty(ref _referenceDate, value);
                ReloadTekiyoName();
            }
        }

        // 拠点コード
        private string _cdKyoten = string.Empty;
        public string CdKyoten
        {
            get => _cdKyoten;
            set
            {
                SetProperty(ref _cdKyoten, value);
                _isChange = true;
                NmKyoten = NameLoader.GetKyoten(CdKyoten);
            }
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
            set
            {
                SetProperty(ref _cdDistGroup, value);
                _isChange = true;
            }
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
            set
            {
                SetProperty(ref _binSumType, value);
                _isChange = true;
            }
        }

        // 適用開始日
        private DateTime _dtTekiyoKaishi;
        public DateTime DtTekiyoKaishi
        {
            get => _dtTekiyoKaishi;
            set
            {
                SetProperty(ref _dtTekiyoKaishi, value);
                _isChange = true;
            }
        }

        // 適用無効日
        private DateTime _dtTekiyoMuko;
        public DateTime DtTekiyoMuko
        {
            get => _dtTekiyoMuko;
            set
            {
                SetProperty(ref _dtTekiyoMuko, value);
                _isChange = true;
            }
        }

        // 登録日時
        private DateTime? _dtTorokuNichiji;
        public DateTime? DtTorokuNichiji
        {
            get => _dtTorokuNichiji;
            set => SetProperty(ref _dtTorokuNichiji, value);
        }

        // 更新日時
        private DateTime? _dtKoshinNichiji;
        public DateTime? DtKoshinNichiji
        {
            get => _dtKoshinNichiji;
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

        // 対象出荷バッチリスト(バッチリスト ＋ 大仕分けリスト＋コースリスト)
        private ObservableCollection<BatchInfo> _batches = new ObservableCollection<BatchInfo>();
        public ObservableCollection<BatchInfo> Batches
        {
            get => _batches;
            set => SetProperty(ref _batches, value);
        }

        // 入力有りバッチ
        private IEnumerable<BatchInfo> _inputedBatchs => Batches.Where(x => !x.CdShukkaBatch.Trim().IsNullOrEmpty());

        private int _selectBatchIndex;
        public int SelectBatchIndex
        {
            get => _selectBatchIndex;
            set => SetProperty(ref _selectBatchIndex, value);
        }

        // コース順リスト
        private ObservableCollection<Course> _courses = new ObservableCollection<Course>();
        public ObservableCollection<Course> Courses
        {
            get => _courses;
            set => SetProperty(ref _courses, value);
        }
        // 入力有りコース
        private IEnumerable<Course> _inputedCourses => Courses.Where(x => !x.CdCourse.Trim().IsNullOrEmpty());

        private int _selectCourseIndex;
        public int SelectCourseIndex
        {
            get => _selectCourseIndex;
            set => SetProperty(ref _selectCourseIndex, value);
        }

        // 履歴表示リスト
        private List<Log> _logs = new List<Log>();
        public List<Log> Logs
        {
            get => _logs;
            set => SetProperty(ref _logs, value);
        }

        private bool _isAdd = false;
        public bool IsAdd
        {
            get => _isAdd;
            set => SetProperty(ref _isAdd, value);
        }

        private bool _isEdit = false;
        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

        private bool _isDateRelease = false;
        public bool IsDateRelease
        {
            get => _isDateRelease;
            set => SetProperty(ref _isDateRelease, value);
        }

        private ShainInfo _shainInfo = new ShainInfo();

        public ReferenceLog ReferenceLog { get; set; } = new ReferenceLog();

        public DateTime? _lastReferenceDate;

        private bool _isChange = false;

        private readonly IDialogService _dialogService;

        public InputDistGroupDlgViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Clear = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Clear");
                ClearInfo(IsAdd);
                Courses.CollectionChanged += Courses_CollectionChanged;
            });

            Register = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Register");
                if (Regist())
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
            });

            Back = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Back");
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });

            Refer = new DelegateCommand(() =>
            {
                if (!ReferenceLog.LogInfos.Any() || IsAdd)
                {
                    return;
                }

                Syslog.Debug("InputDistGroupViewModelDlg:Refer");
                ClearInfo(IsAdd);
                SetReferenceInfo(false);
                Batches.CollectionChanged += Batches_CollectionChanged;
                Courses.CollectionChanged += Courses_CollectionChanged;
            });

            Release = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Release");
                IsDateRelease = true;
            });

            UpReplace = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:UpReplace");
                MoveCourse(true);
            });

            DownReplace = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:DownReplace");
                MoveCourse(false);
            });

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Add");
                InsertCourse();
            });

            Delete = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistGroupViewModelDlg:Delete");
                DeleteCourse();
            });
        }

        public bool CanCloseDialog() => ConfirmationExit();

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _distGroup = parameters.GetValue<Models.DistGroupInfo>("DistGroup");
            _shainInfo = parameters.GetValue<ShainInfo>("ShainInfo");
            IsAdd = _distGroup.CdDistGroup.IsNullOrEmpty();
            IsEdit = !IsAdd;
            InitDialog();
        }

        private void InitDialog()
        {
            ClearInfo(true);

            if (!IsAdd)
            {
                CdKyoten = _distGroup.CdKyoten;
                CdDistGroup = _distGroup.CdDistGroup;
                NmDistGroup = _distGroup.NmDistGroup;
                ReferenceLog.LogInfos = LogLoader.Get(_distGroup.CdDistGroup).ToList();

                SetReferenceInfo(true);
            }
            else
            {
                ReferenceLog.LogInfos.Clear();
            }

            Batches.CollectionChanged += Batches_CollectionChanged;
            Courses.CollectionChanged += Courses_CollectionChanged;
        }

        private void ClearInfo(bool isAll)
        {
            if (isAll)
            {
                ReferenceDate = DateTime.Today;
                CdKyoten = string.Empty;
                CdDistGroup = string.Empty;
            }

            NmDistGroup = string.Empty;
            BinSumType = BinSumType.No;
            Batches = new ObservableCollection<BatchInfo> { new BatchInfo() };
            Courses.Clear();

            DtTekiyoKaishi = DateTime.Today;
            DtTekiyoMuko = new DateTime(2999, 12, 31);
            DtTorokuNichiji = null;
            DtKoshinNichiji = null;
            CdShain = string.Empty;
            NmShain = string.Empty;

            _isChange = false;
        }

        // 参照日から情報取得
        private void SetReferenceInfo(bool isInit)
        {
            var tekiyoDate = ReferenceLog.GetStartDateInRange(ReferenceDate.ToString("yyyyMMdd"));
            var data = DistGroupLoader.GetFromKey(_distGroup.CdDistGroup, tekiyoDate);

            if (data is not null)
            {
                BinSumType = data.CdBinSum;
                Batches = new ObservableCollection<BatchInfo>(GetJoinBatch(data.Batches, data.LargeDists));
                Courses = new ObservableCollection<Course>(data.Courses);

                DtTorokuNichiji = data.CreatedAt;
                DtKoshinNichiji = data.UpdatedAt;
                CdShain = _shainInfo.HenkoshaCode;
                NmShain = _shainInfo.HenkoshaName;
                DtTekiyoKaishi = DateTime.Parse(data.Tekiyokaishi.GetDate());
                DtTekiyoMuko = DateTime.Parse(data.TekiyoMuko.GetDate());

                _lastReferenceDate = ReferenceDate;
            }
            else if (!isInit)
            {
                MessageDialog.Show(_dialogService, "参照する履歴はありません", "該当適用期間無し");
                if (_lastReferenceDate is not null)
                {
                    ReferenceDate = _lastReferenceDate.GetValueOrDefault();
                    SetReferenceInfo(true);
                }

            }
            _isChange = false;
        }

        // 出荷バッチ、大仕分け結合。コース読込
        private List<BatchInfo> GetJoinBatch(List<BatchInfo> batches, List<LargeDist> largeDists)
        {
            return (from batch in batches
                    join largeDist in largeDists
                    on new { batch.IdDistGroup, batch.Sequence } equals new { largeDist.IdDistGroup, largeDist.Sequence }
                    select new BatchInfo
                    {
                        IdDistGroup = batch.IdDistGroup,
                        CdShukkaBatch = batch.CdShukkaBatch,
                        CdLargeGroup = largeDist.CdLargeGroup,
                    }).ToList();
        }

        private bool ConfirmationExit()
        {
            if (_isChange)
            {
                if (MessageDialog.Show(_dialogService, "変更された情報が登録されていません。\n一覧画面に戻りますか？", "変更確認", ButtonMask.Yes | ButtonMask.No) != ButtonResult.Yes)
                {
                    return false;
                }
            }

            return true;
        }

        // 適用名称再取得
        private void ReloadTekiyoName()
        {
            NameLoader.selectDate = ReferenceDate.ToString("yyyyMMdd");

            NmKyoten = NameLoader.GetKyoten(CdKyoten);
            Batches = new ObservableCollection<BatchInfo>(Batches.Select(x => new BatchInfo { CdShukkaBatch = x.CdShukkaBatch, CdLargeGroup = x.CdLargeGroup }));
        }

        private bool Regist()
        {
            try
            {
                if (!ValidateInput())
                {
                    return false;
                }

                var targetData = new DistGroupInfo
                {
                    CdKyoten = CdKyoten,
                    CdDistGroup = CdDistGroup.PadLeft(5, '0'),
                    NmDistGroup = NmDistGroup,
                    CdBinSum = BinSumType,
                    Batches = _inputedBatchs.ToList(),

                    Tekiyokaishi = DtTekiyoKaishi.ToString("yyyyMMdd"),
                    TekiyoMuko = DtTekiyoMuko.ToString("yyyyMMdd"),
                };

                var existData = DistGroupLoader.GetFromKey(targetData.CdDistGroup, targetData.Tekiyokaishi);
                var isExist = existData is not null;

                if (!ValidateSummaryDate(isExist))
                {
                    return false;
                }

                if (!IsDuplicationCourse(existData?.IdDistGroup))
                {
                    return false;
                }

                if (IsAdd)
                {
                    if (isExist)
                    {
                        MessageDialog.Show(_dialogService, "同一組み合わせのデータが登録済みです" +
                            $"\n仕分グループ[{CdDistGroup}],適用開始日[{DtTekiyoKaishi.ToString("yyyyMMdd")}]\n"
                            , "入力エラー");
                        return false;
                    }

                    DistGroupEntityManager.Regist(targetData, _shainInfo, _inputedCourses);
                }
                else if (isExist)
                {
                    targetData.IdDistGroup = existData!.IdDistGroup;
                    DistGroupEntityManager.Update(targetData, _shainInfo, _inputedCourses);
                }
                else
                {
                    DistGroupEntityManager.Regist(targetData, _shainInfo, _inputedCourses);
                }

                _isChange = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
                return false;
            }
        }

        private bool ValidateInput()
        {
            if (CdKyoten.Trim().IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "拠点コードを入力してください。", "入力エラー");
                return false;
            }

            if (CdDistGroup.Trim().IsNullOrEmpty()||
                NmDistGroup.IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "仕分グループコード、名称を入力してください。", "入力エラー");
                return false;
            }

            if (NmKyoten.IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "拠点名称が取得出来ていません。", "入力エラー");
                return false;
            }

            if (BinSumType == BinSumType.No && Batches.Count > 1)
            {
                MessageDialog.Show(_dialogService, "配送便集計をしない場合は、出荷バッチは１バッチだけにして下さい。", "入力エラー");
                return false;
            }

            if (!_inputedBatchs.Any())
            {
                MessageDialog.Show(_dialogService, "対象出荷バッチを入力してください", "入力エラー");
                return false;
            }

            if (_inputedBatchs.Any(x => x.CdLargeGroup.Trim().IsNullOrEmpty()))
            {
                MessageDialog.Show(_dialogService, "対象出荷バッチ内に、大仕分けコードが入力されていない項目があります", "入力エラー");
                return false;
            }

            if (_inputedBatchs.Any(x => x.NmShukkaBatch.IsNullOrEmpty() || x.NmLargeGroup.IsNullOrEmpty()))
            {
                MessageDialog.Show(_dialogService, "対象出荷バッチ内に、名称が取得出来ていない項目があります", "入力エラー");
                return false;
            }

            if(_inputedBatchs.GroupBy(x => x.CdShukkaBatch).Any(x => x.Count() > 1))
            {
                MessageDialog.Show(_dialogService, "対象出荷バッチが重複しています", "入力エラー");
                return false;
            }

            if (!_inputedCourses.Any())
            {
                MessageDialog.Show(_dialogService, $"コースを追加してください", "入力エラー");
                return false;
            }

            if (_inputedCourses.Count() > 999)
            {
                MessageDialog.Show(_dialogService, $"コース", "入力エラー");
                return false;
            }

            return true;
        }

        private bool IsDuplicationCourse(long? idDistGroup)
        {
            var inputduplicationCourse = _inputedCourses.GroupBy(x => x.CdCourse).Where(x => x.Count() > 1);

            if (inputduplicationCourse.Any())
            {
                MessageDialog.Show(_dialogService, "入力したコース内で重複があります\n\n" +
                    $"コース[{string.Join(",", inputduplicationCourse.Select(x => x.Key))}]", "入力エラー");
                return false;
            }

            // 他仕分グループの同一バッチ
            var sameBatchDists = DistGroupLoader.GetSameBatchDists(_inputedBatchs.Select(x => x.PadBatch), idDistGroup ?? -1,
                DtTekiyoKaishi.ToString("yyyyMMdd"), DtTekiyoMuko.ToString("yyyyMMdd"));

            foreach(var batchDist in sameBatchDists)
            {
                var sameCourses = batchDist.Courses.Select(x => x.PadCourse).Intersect(Courses.Select(x => x.PadCourse));

                if (sameCourses.Any())
                {
                    MessageDialog.Show(_dialogService,
                    $"他の仕分グループに登録されたコースが入力されています"
                    + $"\n\n{GetSameDistMessage(batchDist)}"
                    + $"\n\nバッチ[{string.Join(",", batchDist.Batches.Select(x => x.PadBatch))}]"
                    + $"\nコース[{string.Join(",", sameCourses)}]", "入力エラー");
                    return false;
                }
            }

            return true;
        }

        private string GetSameDistMessage(DistGroupInfo sameDistData)
        {
            return $"拠点[{sameDistData.CdKyoten}] 仕分グループコード[{sameDistData.CdDistGroup}]\n" +
                   $"適用開始-適用無効[{sameDistData.Tekiyokaishi}-{sameDistData.TekiyoMuko}]";
        }

        // 適用期間チェック
        private bool ValidateSummaryDate(bool isUpdate)
        {
            try
            {
                ReferenceLog.LogInfos = LogLoader.Get(CdDistGroup.PadLeft(5, '0')).ToList();
                ReferenceLog.ValidateSummaryDate(DtTekiyoKaishi, DtTekiyoMuko, isUpdate);
                return true;
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "入力エラー");
                return false;
            }
        }

        private void Courses_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ReNumberCourse();
            _isChange = true;
        }

        private void Batches_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _isChange = true;
        }

        // コース編集関連
        private void ReNumberCourse()
        {
            var idx = 1;
            foreach (var course in Courses)
            {
                course.NuCourseSeq = idx;
                idx++;
            }
        }
        private bool ValidateCourseIndex(int idx)
        {
            if (idx < 0 || idx > Courses.Count - 1)
            {
                return false;
            }

            return true;
        }

        private void DeleteCourse()
        {
            if (!ValidateCourseIndex(SelectCourseIndex))
            {
                return;
            }

            Courses.Remove(Courses[SelectCourseIndex]);
        }

        private void InsertCourse()
        {
            if (!ValidateCourseIndex(SelectCourseIndex))
            {
                return;
            }
            Courses.Insert(SelectCourseIndex, new Course());
        }

        private void MoveCourse(bool isUp)
        {
            if (SelectCourseIndex == -1)
            {
                return;
            }

            var newIndex = isUp ? SelectCourseIndex - 1 : SelectCourseIndex + 1;

            if (!ValidateCourseIndex(SelectCourseIndex) || !ValidateCourseIndex(newIndex))
            {
                return;
            }

            Courses.Move(SelectCourseIndex, newIndex);
        }
    }
}
