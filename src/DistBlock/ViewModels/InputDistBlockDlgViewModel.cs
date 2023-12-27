using DbLib.Defs;
using DbLib.Extensions;
using DistBlock.Loader;
using DistBlock.Models;
using LogLib;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.IdentityModel.Tokens;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using ReferenceLogLib;
using ReferenceLogLib.Models;
using System.Collections.ObjectModel;
using TakakiLib.Models;
using WindowLib.Utils;

namespace DistBlock.ViewModels
{
    public class InputDistBlockDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand Clear { get; }
        public DelegateCommand Register { get; }
        public DelegateCommand Back { get; }
        public DelegateCommand Refer { get; }
        public DelegateCommand Release { get; }
        public DelegateCommand Add { get; }
        public DelegateCommand Delete { get; }

        public string Title => "仕分ブロック順情報入力";
        public event Action<IDialogResult>? RequestClose;
        private DistBlockInfo _distBlock = new DistBlockInfo();

        // 参照日
        private DateTime _referenceDate;
        public DateTime ReferenceDate
        {
            get => _referenceDate;
            set
            {
                SetProperty(ref _referenceDate, value);
                ReloadTekiyoData();
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
                NmDistGroup = NameLoader.GetDistGroup(CdDistGroup);
            }
        }

        // 仕分グループ名称
        private string _nmDistGroup = string.Empty;
        public string NmDistGroup
        {
            get => _nmDistGroup;
            set => SetProperty(ref _nmDistGroup, value);
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

        // ブロック割当順リスト
        private ObservableCollection<Block> _blocks = new ObservableCollection<Block>();
        public ObservableCollection<Block> Blocks
        {
            get => _blocks;
            set => SetProperty(ref _blocks, value);
        }

        private IEnumerable<Block> _inputedBlocks => Blocks.Where(x => !x.CdBlock.Trim().IsNullOrEmpty());

        private int _selectBlockIndex;
        public int SelectBlockIndex
        {
            get => _selectBlockIndex;
            set => SetProperty(ref _selectBlockIndex, value);
        }

        private bool _isDateRelease = false;
        public bool IsDateRelease
        {
            get => _isDateRelease;
            set => SetProperty(ref _isDateRelease, value);
        }

        private bool _isAdd = false;
        public bool IsAdd
        {
            get => _isAdd;
            set => SetProperty(ref _isAdd, value);
        }

        private bool _canAddBlockRows = true;
        public bool CanAddBlockRows
        {
            get => _canAddBlockRows;
            set => SetProperty(ref _canAddBlockRows, value);
        }

        private ShainInfo _shainInfo = new ShainInfo();

        private bool _isChange = false;
        private const int MAX_BLOCK_COUNT = 10;

        public ReferenceLog ReferenceLog { get; set; } = new ReferenceLog();

        private readonly IDialogService _dialogService;

        public InputDistBlockDlgViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Clear = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistBlockDlgViewModel:Clear");
                ClearInfo(IsAdd);
                Blocks.CollectionChanged += Blocks_CollectionChanged;
            });

            Register = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistBlockDlgViewModel:Register");
                if (Regist())
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
            });

            Back = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistBlockDlgViewModel:Back");
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });

            Refer = new DelegateCommand(() =>
            {
                if (IsAdd)
                {
                    return;
                }

                Syslog.Debug("InputDistBlockDlgViewModel:Refer");
                ClearInfo(false);
                SetReferenceInfo();
                Blocks.CollectionChanged += Blocks_CollectionChanged;
            });

            Release = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistBlockDlgViewModel:Release");
                IsDateRelease = true;
            });

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistBlockDlgViewModel:Add");
                InsertBlock();
            });

            Delete = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistBlockDlgViewModel:Delete");
                DeleteBlock();
            });
        }

        public bool CanCloseDialog() => ConfirmationExit();

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _distBlock = parameters.GetValue<DistBlockInfo>("DistBlock");
            _shainInfo = parameters.GetValue<ShainInfo>("ShainInfo");
            IsAdd = _distBlock.CdDistGroup.IsNullOrEmpty();
            InitDialog();
        }

        private void InitDialog()
        {
            ClearInfo(true);

            if (!IsAdd)
            {
                CdKyoten = _distBlock.CdKyoten;
                CdDistGroup = _distBlock.CdDistGroup;
                ReferenceLog.LogInfos = LogLoader.Get(_distBlock.CdDistGroup).ToList();

                // 参照日初期値で履歴から検索
                SetReferenceInfo();
                CanAddBlockRows = Blocks.Count < MAX_BLOCK_COUNT;
            }
            else
            {
                ReferenceLog.LogInfos.Clear();
            }

            ResetBlockReferenceDate();
            Blocks.CollectionChanged += Blocks_CollectionChanged;
        }

        private void ClearInfo(bool isAll)
        {
            IsDateRelease = false;

            if (isAll)
            {
                ReferenceDate = DateTime.Today;
                CdKyoten = string.Empty;
                NmKyoten = string.Empty;
                CdDistGroup = string.Empty;
            }

            DtTekiyoKaishi = DateTime.Today;
            DtTekiyoMuko = new DateTime(2999, 12, 31);
            DtTorokuNichiji = null;
            DtKoshinNichiji = null;
            CdShain = string.Empty;
            NmShain = string.Empty;
            Blocks = new ObservableCollection<Block>() { new Block { NuBlockSeq = 1, ReferenceDate = ReferenceDate.ToString("yyyyMMdd") } };

            _isChange = false;
        }

        // 参照
        private void SetReferenceInfo()
        {
            var tekiyoDate = ReferenceLog.GetStartDateInRange(ReferenceDate.ToString("yyyyMMdd"));
            var tekiyoData = DistBlockLoader.GetFromKey(_distBlock.CdDistGroup, tekiyoDate);

            if (tekiyoData is not null)
            {
                DtTorokuNichiji = tekiyoData.CreatedAt;
                DtKoshinNichiji = tekiyoData.UpdatedAt;
                CdShain = _shainInfo.HenkoshaCode;
                NmShain = _shainInfo.HenkoshaName;
                DtTekiyoKaishi = DateTime.Parse(tekiyoData.Tekiyokaishi.GetDate());
                DtTekiyoMuko = DateTime.Parse(tekiyoData.TekiyoMuko.GetDate());

                Blocks = new ObservableCollection<Block>(tekiyoData.Blocks);
            }
            _isChange = false;
        }

        private bool Regist()
        {
            try
            {
                if (!ValidateInput())
                {
                    return false;
                }

                if (!IsDuplicationAddr())
                {
                    return false;
                }

                var targetInfo = new DistBlockInfo
                {
                    CdKyoten = CdKyoten,
                    CdDistGroup = CdDistGroup.PadLeft(4, '0'),
                    Tekiyokaishi = DtTekiyoKaishi.ToString("yyyyMMdd"),
                    TekiyoMuko = DtTekiyoMuko.ToString("yyyyMMdd"),
                    Blocks = _inputedBlocks.ToList(),
                };

                var existCustomer = DistBlockLoader.GetFromKey(targetInfo.CdDistGroup, targetInfo.Tekiyokaishi);
                var isExist = existCustomer is not null;

                if (!ValidateSummaryDate(isExist))
                {
                    return false;
                }

                if (IsAdd)
                {
                    if (isExist)
                    {
                        MessageDialog.Show(_dialogService,
                            $"仕分グループコード[{targetInfo.CdDistGroup}],適用開始日[{targetInfo.Tekiyokaishi}]\n同一組み合わせのデータが登録済みです",
                            "入力エラー");
                        return false;
                    }

                    DistBlockEntityManager.Regist(targetInfo, _shainInfo);
                }
                else if (isExist)
                {
                    targetInfo.DistBlockId = existCustomer!.DistBlockId;
                    DistBlockEntityManager.Update(targetInfo, _shainInfo);
                }
                else
                {
                    DistBlockEntityManager.Regist(targetInfo, _shainInfo);
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

        private bool IsDuplicationAddr()
        {
            var inputBlocks = _inputedBlocks
                .Select((x, idx) => new SameDistBlock
                {

                    CdBlock = x.PadBlock,
                    CdAddrFrom = x.CdAddrFrom,
                    CdAddrTo = x.CdAddrTo,
                });

            var targetBlocks = inputBlocks.GroupBy(x => x.PadBlock).Select(x => x.Key);

            foreach (var targetBlock in targetBlocks)
            {
                var targetInputs = inputBlocks.Where(x => x.PadBlock == targetBlock);

                // 入力したリスト内でのみ検証
                if (targetInputs.Count() > 1)
                {
                    var dupliationAddrMsg = GetDuplicationRange(targetInputs.First(), targetInputs.Skip(1));
                    if (!dupliationAddrMsg.IsNullOrEmpty())
                    {
                        MessageDialog.Show(_dialogService, "入力したブロック割当範囲内で重複があります\n\n" +
                            $"ブロック[{targetBlock}]\n" +
                            $"[{dupliationAddrMsg}]", "入力エラー");
                        return false;
                    }
                }
            }

            return true;
        }

        private string GetDuplicationRange(SameDistBlock inputBlockRanges, IEnumerable<SameDistBlock> otherRanges)
        {
            foreach (var otherRange in otherRanges)
            {
                // 開始or終了が他範囲内 or 他開始or終了が自範囲内
                if (IsInRange(inputBlockRanges.PadAddrFrom, otherRange.PadAddrFrom, otherRange.PadAddrTo) ||
                    IsInRange(inputBlockRanges.PadAddrTo, otherRange.PadAddrFrom, otherRange.PadAddrTo) ||
                    IsInRange(otherRange.PadAddrFrom, inputBlockRanges.PadAddrFrom, inputBlockRanges.PadAddrTo) ||
                    IsInRange(otherRange.PadAddrTo, inputBlockRanges.PadAddrFrom, inputBlockRanges.PadAddrTo))
                {
                    var otherDataMessage = string.Empty;

                    if (!otherRange.CdKyoten.IsNullOrEmpty())
                    {
                        otherDataMessage = $"拠点[{otherRange.CdKyoten}] 仕分グループ[{otherRange.CdDistGroup}]\n" +
                                            $"適用開始-適用無効[{otherRange.Tekiyokaishi}-{otherRange.TekiyoMuko}]\n";
                    }

                    return $"開始アドレス[{inputBlockRanges.PadAddrFrom}] 終了アドレス[{inputBlockRanges.PadAddrTo}]\n\n" +
                           otherDataMessage +
                           $"開始アドレス[{otherRange.PadAddrFrom}] 終了アドレス[{otherRange.PadAddrTo}]";
                }
            }

            return string.Empty;
        }

        private bool IsInRange(string targetAddr, string startAddr, string invalidAddr)
        {
            // 開始以上(1or0) ＆ 終了以下(-1or0)
            return targetAddr.CompareTo(startAddr) != -1 && targetAddr.CompareTo(invalidAddr) != 1;
        }

        private bool ValidateInput()
        {
            if (CdKyoten.Trim().IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "拠点コードを入力してください。", "入力エラー");
                return false;
            }

            if (CdDistGroup.Trim().IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "仕分グループコードを入力してください。", "入力エラー");
                return false;
            }

            if (NmKyoten.IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "拠点名称が取得出来ていません。", "入力エラー");
                return false;
            }

            if (NmDistGroup.IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "仕分グループ名称が取得出来ていません。", "入力エラー");
                return false;
            }

            if (!_inputedBlocks.Any())
            {
                MessageDialog.Show(_dialogService, "ブロック割当順を追加して下さい", "入力エラー");
                return false;
            }

            if (_inputedBlocks.Count() > 10)
            {
                MessageDialog.Show(_dialogService, "ブロック割当順は10件まで登録可能です", "入力エラー");
                return false;
            }

            var noExistBlocks = _inputedBlocks.Where(x => !x.IsExistTbBlock);
            if (noExistBlocks.Any())
            {
                MessageDialog.Show(_dialogService, $"座席しきい値情報に未登録のブロックがあります\nブロック[{string.Join(",", noExistBlocks.Select(x => x.CdBlock))}]", "入力エラー");
                return false;
            }

            var notValidRangeBlocks = _inputedBlocks.Where(x => !x.IsVaridRange).Select(x => $"ブロック[{x.CdBlock}] 開始-終了[{x.CdAddrFrom}-{x.CdAddrTo}]");
            if (notValidRangeBlocks.Any())
            {
                MessageDialog.Show(_dialogService, $"無効な開始、終了アドレスがあります。\n{string.Join("\n", notValidRangeBlocks)}", "入力エラー");
                return false;
            }

            return true;
        }

        // 適用期間チェック
        private bool ValidateSummaryDate(bool isUpdate)
        {
            try
            {
                ReferenceLog.LogInfos = LogLoader.Get(CdDistGroup.PadLeft(4, '0')).ToList();
                ReferenceLog.ValidateSummaryDate(DtTekiyoKaishi, DtTekiyoMuko, isUpdate);
                return true;
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "入力エラー");
                return false;
            }
        }

        private void ReloadTekiyoData()
        {
            NameLoader.selectDate = ReferenceDate.ToString("yyyyMMdd");
            NmKyoten = NameLoader.GetKyoten(CdKyoten);
            NmDistGroup = NameLoader.GetDistGroup(CdDistGroup);

            ResetBlockReferenceDate();
        }

        // 全ブロック内参照日更新
        private void ResetBlockReferenceDate()
        {
            foreach (var block in Blocks)
            {
                block.ReferenceDate = ReferenceDate.ToString("yyyyMMdd");
            }
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

        // ブロック割当リスト編集関連
        private void ReNumberCourse()
        {
            var idx = 1;
            foreach (var course in Blocks)
            {
                course.NuBlockSeq = idx;
                idx++;
            }
        }

        private void Blocks_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ReNumberCourse();
            _isChange = true;
            SetDelayCanAddBlockrowsAsync();
            ResetBlockReferenceDate();
        }

        // 行追加可否セット
        private async void SetDelayCanAddBlockrowsAsync()
        {
            // 例外発生時にリトライ
            // MEMO:行追加直後or編集中の場合例外発生。リトライ時にエラーが発生しない原因が不明
            try
            {
                await Task.Delay(500);
                CanAddBlockRows = Blocks.Count < MAX_BLOCK_COUNT;
            }
            catch (Exception ex)
            {
                Syslog.Debug($"SetCanAddBlockrowsAsync is Fail Retry. Message={ex.Message}");
                SetDelayCanAddBlockrowsAsync();
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

        private void DeleteBlock()
        {
            if (!ValidateIndex(SelectBlockIndex, Blocks.Count))
            {
                return;
            }

            Blocks.Remove(Blocks[SelectBlockIndex]);
        }

        private void InsertBlock()
        {
            if (Blocks.Count >= MAX_BLOCK_COUNT)
            {
                return;
            }

            Blocks.Insert(SelectBlockIndex, new Block());
        }
    }
}
