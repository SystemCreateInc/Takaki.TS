using DbLib.Defs;
using DbLib.Extensions;
using DistGroup.Loader;
using DistGroup.Models;
using LogLib;
using MaterialDesignThemes.Wpf.Converters;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.IdentityModel.Tokens;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using ReferenceLogLib;
using System.Collections.ObjectModel;
using System.Linq;
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

        private int _selectBatchIndex;
        public int SelectBatchIndex
        {
            get => _selectBatchIndex;
            set
            {
                SetProperty(ref _selectBatchIndex, value);
                ReSetCourseFromBatch();
            }
        }

        // コース順リスト
        private ObservableCollection<Course> _courses = new ObservableCollection<Course>();
        public ObservableCollection<Course> Courses
        {
            get => _courses;
            set => SetProperty(ref _courses, value);
        }

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

        private bool _isDateRelease = false;
        public bool IsDateRelease
        {
            get => _isDateRelease;
            set => SetProperty(ref _isDateRelease, value);
        }

        private ShainInfo _shainInfo = new ShainInfo();

        public ReferenceLog ReferenceLog { get; set; } = new ReferenceLog();

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
                SetReferenceInfo();
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
                ReferenceLog.LogInfos = LogLoader.Get(_distGroup.CdKyoten, _distGroup.CdDistGroup).ToList();

                SetReferenceInfo();
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

            BinSumType = BinSumType.No;
            // デフォルトで出荷バッチ&コース1件
            Batches = new ObservableCollection<BatchInfo> { new BatchInfo() { Courses = new ObservableCollection<Course>() } };
            Courses = Batches[0].Courses;

            DtTekiyoKaishi = DateTime.Today;
            DtTekiyoMuko = new DateTime(2999, 12, 31);
            DtTorokuNichiji = null;
            DtKoshinNichiji = null;
            CdShain = string.Empty;
            NmShain = string.Empty;

            _isChange = false;
        }

        // 参照日から情報取得
        private void SetReferenceInfo()
        {
            var tekiyoDate = ReferenceLog.GetStartDateInRange(ReferenceDate.ToString("yyyyMMdd"));
            var data = DistGroupLoader.GetFromKey(_distGroup.CdKyoten, _distGroup.CdDistGroup, tekiyoDate);

            if (data is not null)
            {
                BinSumType = data.CdBinSum;
                Batches = new ObservableCollection<BatchInfo>(GetJoinBatch(data.Batches, data.LargeDists));
                Courses = Batches[0].Courses;
                ReSetCourseFromBatch();

                DtTorokuNichiji = data.CreatedAt;
                DtKoshinNichiji = data.UpdatedAt;
                CdShain = _shainInfo.HenkoshaCode;
                NmShain = _shainInfo.HenkoshaName;
                DtTekiyoKaishi = DateTime.Parse(data.Tekiyokaishi.GetDate());
                DtTekiyoMuko = DateTime.Parse(data.TekiyoMuko.GetDate());
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
                        Courses = new ObservableCollection<Course>(CourseLoader.Get(batch.IdDistGroup, batch.CdShukkaBatch)),
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
                DeleteNullCourse();
                // WARNING:Bindingが外れている場合がある為、再度代入する
                if(ValidateIndex(SelectBatchIndex, Batches.Count))
                {
                    Batches[SelectBatchIndex].Courses = Courses;
                }

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
                    Batches = Batches.Where(x => !x.CdShukkaBatch.IsNullOrEmpty()).ToList(),

                    Tekiyokaishi = DtTekiyoKaishi.ToString("yyyyMMdd"),
                    TekiyoMuko = DtTekiyoMuko.ToString("yyyyMMdd"),
                };

                var existData = DistGroupLoader.GetFromKey(targetData.CdKyoten, targetData.CdDistGroup, targetData.Tekiyokaishi);
                var isExist = existData is not null;

                if (!ValidateSummaryDate())
                {
                    return false;
                }

                if (!IsDuplicationBatchCourse(existData?.IdDistGroup))
                {
                    return false;
                }

                if (IsAdd)
                {
                    if (isExist)
                    {
                        MessageDialog.Show(_dialogService, "同一組み合わせのデータが登録済みです" +
                            $"\n拠点[{CdKyoten}],仕分グループ[{CdDistGroup}],適用開始日[{DtTekiyoKaishi.ToString("yyyyMMdd")}]\n"
                            , "入力エラー");
                        return false;
                    }

                    DistGroupEntityManager.Regist(targetData, _shainInfo);
                }
                else if (isExist)
                {
                    targetData.IdDistGroup = existData!.IdDistGroup;
                    DistGroupEntityManager.Update(targetData, _shainInfo);
                }
                else
                {
                    DistGroupEntityManager.Regist(targetData, _shainInfo);
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

        // 全コースのキー未入力行を削除
        private void DeleteNullCourse()
        {
            foreach (var batch in Batches)
            {
                batch.Courses = new ObservableCollection<Course>(batch.Courses.Where(x => !x.CdCourse.IsNullOrEmpty()));
            }
        }

        private bool ValidateInput()
        {
            if (CdKyoten.IsNullOrEmpty() ||
                CdDistGroup.IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "拠点コード、仕分グループコードを入力してください。", "入力エラー");
                return false;
            }

            if (NmKyoten.IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "拠点名称が取得出来ていません。", "入力エラー");
                return false;
            }

            if (NmDistGroup.IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "仕分グループ名称を入力してください", "入力エラー");
                return false;
            }

            if (!Batches.Any(x => !x.CdShukkaBatch.IsNullOrEmpty()))
            {
                MessageDialog.Show(_dialogService, "対象出荷バッチを入力してください", "入力エラー");
                return false;
            }

            if (BinSumType == BinSumType.No && Batches.Count > 1)
            {
                MessageDialog.Show(_dialogService, "配送便集計をするに変更。\nまたは出荷バッチの入力行を1行のみにして下さい。", "入力エラー");
                return false;
            }

            var validBatches = Batches.Where(x => !x.CdShukkaBatch.IsNullOrEmpty());

            if (validBatches.Any(x => x.NmShukkaBatch.IsNullOrEmpty() || x.NmLargeGroup.IsNullOrEmpty()))
            {
                MessageDialog.Show(_dialogService, "対象出荷バッチ内に、名称が取得出来ていない項目があります", "入力エラー");
                return false;
            }

            if (validBatches.Any(x => !x.CdShukkaBatch.IsNullOrEmpty() && x.CdLargeGroup.IsNullOrEmpty()))
            {
                MessageDialog.Show(_dialogService, "対象出荷バッチ内に、大仕分けコードが入力されていない項目があります", "入力エラー");
                return false;
            }

            if(validBatches.GroupBy(x => x.CdShukkaBatch).Any(x => x.Count() > 1))
            {
                MessageDialog.Show(_dialogService, "対象出荷バッチが重複しています", "入力エラー");
                return false;
            }

            // 全バッチのコース入力確認
            if (validBatches.Any(x => !x.Courses.Any()))
            {
                var emptyCourseBatchs = Batches.Where(x => !x.Courses.Any()).Select(x => x.CdShukkaBatch);
                MessageDialog.Show(_dialogService, $"コースが登録されていない出荷バッチがあります\n出荷バッチ「{string.Join(",", emptyCourseBatchs)}」", "入力エラー");
                return false;
            }

            return true;
        }

        private bool IsDuplicationBatchCourse(long? idDistGroup)
        {
            // バッチグループ化
            var batchGroup = Batches.Where(x => !x.PadBatch.IsNullOrEmpty()).GroupBy(x => x.PadBatch);

            foreach (var batch in batchGroup)
            {
                // 同一バッチ内で重複したコース
                var duplicationCourse = batch.SelectMany(x => x.Courses).GroupBy(x => x.PadCourse).Where(x => x.Count() > 1);
                if (duplicationCourse.Any())
                {
                    MessageDialog.Show(_dialogService, $"出荷バッチ[{batch.Key}]でコースが重複しています" +
                        $"\n\nコース[{string.Join(",", duplicationCourse.Select(x => x.Key))}]", "入力エラー");
                    return false;
                }
            }

            // 登録済み　同一出荷バッチコース
            var sameBatchs = DistGroupLoader.GetSameBatchCourse(batchGroup.Select(x => x.Key), idDistGroup ?? -1,
                DtTekiyoKaishi.ToString("yyyyMMdd"), DtTekiyoMuko.ToString("yyyyMMdd"));

            var joinBatchCourse = from sameBatch in sameBatchs
                                  join batch in batchGroup
                                  on sameBatch.CdShukkaBatch equals batch.Key
                                  select new
                                  {
                                      sameBatch.CdKyoten,
                                      sameBatch.CdDistGroup,
                                      sameBatch.CdShukkaBatch,
                                      sameBatch.Tekiyokaishi,
                                      sameBatch.TekiyoMuko,

                                      LoadCourses = sameBatch.Courses,
                                      inputCourses = batch.SelectMany(x => x.Courses.Select(x => x.PadCourse)),
                                  };

            foreach (var batch in joinBatchCourse)
            {
                var duplicationCourse = batch.LoadCourses.Intersect(batch.inputCourses);

                if(duplicationCourse.Any())
                {
                    var distInfo = new DistGroupInfo
                    {
                        CdKyoten = batch.CdKyoten,
                        CdDistGroup = batch.CdDistGroup,
                        Tekiyokaishi = batch.Tekiyokaishi,
                        TekiyoMuko = batch.TekiyoMuko
                    };

                    MessageDialog.Show(_dialogService,
                    $"他の仕分グループに登録されたコースが入力されています"
                    + $"\n\n{GetSameDistMessage(distInfo)}"
                    + $"\n\n出荷バッチ[{batch.CdShukkaBatch}] コース[{string.Join(",", duplicationCourse)}]", "入力エラー");

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
        private bool ValidateSummaryDate()
        {
            try
            {
                ReferenceLog.LogInfos = LogLoader.Get(CdKyoten.PadLeft(4, '0'), CdDistGroup.PadLeft(5, '0')).ToList();
                ReferenceLog.ValidateSummaryDate(DtTekiyoKaishi, DtTekiyoMuko, !IsAdd);
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
            ReSetCourseFromBatch();
            _isChange = true;
        }

        // 対象バッチのコースに置き換え
        private void ReSetCourseFromBatch()
        {
            if (SelectBatchIndex != -1)
            {
                if (Batches.Count > SelectBatchIndex)
                {
                    Courses.CollectionChanged -= Courses_CollectionChanged;
                    Courses = Batches[SelectBatchIndex].Courses;
                    Courses.CollectionChanged += Courses_CollectionChanged;
                }
            }
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
        private bool ValidateIndex(int index, int count)
        {
            if (index < 0 || index > count - 1)
            {
                return false;
            }

            return true;
        }

        private void DeleteCourse()
        {
            if (!ValidateIndex(SelectCourseIndex, Courses.Count))
            {
                return;
            }

            Courses.Remove(Courses[SelectCourseIndex]);
        }

        private void InsertCourse()
        {
            Courses.Insert(SelectCourseIndex, new Course());
        }

        private void MoveCourse(bool isUp)
        {
            if (SelectCourseIndex == -1)
            {
                return;
            }

            var newIndex = isUp ? SelectCourseIndex - 1 : SelectCourseIndex + 1;

            if (!ValidateIndex(newIndex, Courses.Count))
            {
                return;
            }

            Courses.Move(SelectCourseIndex, newIndex);
        }
    }
}
