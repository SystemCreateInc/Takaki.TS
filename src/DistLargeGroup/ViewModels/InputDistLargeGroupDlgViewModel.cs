using DistLargeGroup.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace DistLargeGroup.ViewModels
{
    public class InputDistLargeGroupDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand Clear { get; }
        public DelegateCommand Register { get; }
        public DelegateCommand Back { get; }
        public DelegateCommand Refer { get; }
        public DelegateCommand Release { get; }

        public string Title => "大仕分グループ情報入力";

        public event Action<IDialogResult>? RequestClose;
        private Models.DistLargeGroup _distLargeGroup = new Models.DistLargeGroup();

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

        // 大仕分グループ
        private string _cdLargeGroup = string.Empty;
        public string CdLargeGroup
        {
            get => _cdLargeGroup;
            set => SetProperty(ref _cdLargeGroup, value);
        }

        // 大仕分グループ名称
        private string _cdLargeGroupName = string.Empty;
        public string CdLargeGroupName
        {
            get => _cdLargeGroupName;
            set => SetProperty(ref _cdLargeGroupName, value);
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

        // 履歴表示リスト
        private List<Log> _logs = new List<Log>();
        public List<Log> Logs
        {
            get => _logs;
            set => SetProperty(ref _logs, value);
        }

        public InputDistLargeGroupDlgViewModel()
        {
            Clear = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistLargeGroupDlgViewModel:Clear");
                // fixme:クリアボタン押下
            });

            Register = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistLargeGroupDlgViewModel:Register");
                // fixme:登録ボタン押下
            });

            Back = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistLargeGroupDlgViewModel:Back");
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });

            Refer = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistLargeGroupDlgViewModel:Refer");
                // fixme:参照ボタン押下
            });

            Release = new DelegateCommand(() =>
            {
                Syslog.Debug("InputDistLargeGroupDlgViewModel:Release");
                // fixme:解除ボタン押下
            });
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _distLargeGroup = parameters.GetValue<Models.DistLargeGroup>("DistLargeGroup");
            InitDialog();
        }

        private void InitDialog()
        {
            // 項目欄確認
            Date = DateTime.Today;
            CdKyoten = "4201";
            NmKyoten = "広島工場製品出荷";
            CdLargeGroup = "1";
            CdLargeGroupName = "大仕分グループ名称";
            DtTekiyoKaishi = new DateTime(2023, 10, 1);
            DtTekiyoMuko = new DateTime(2023, 12, 31);
            DtTorokuNichiji = new DateTime(2023, 10, 11, 12, 34, 56);
            DtKoshinNichiji = new DateTime(2023, 10, 11, 12, 34, 56);
            CdShain = "0033550";
            NmShain = "小田 賢行";

            Logs = new List<Log>
            {
                new Log { Selected = false, DtTekiyoKaishi = "20230901", DtTekiyoMuko = "20231001", CdShain = "0033550", },
                new Log { Selected = true, DtTekiyoKaishi = "20231001", DtTekiyoMuko = "20231231", CdShain = "0033550", },
            };
        }
    }
}
