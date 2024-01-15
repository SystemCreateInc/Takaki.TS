using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using StowageListPrint.Models;

namespace StowageListPrint.ViewModels
{
    public class SelectDistGroupDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand Enter { get; }
        public DelegateCommand OK { get; }
        public DelegateCommand Cancel { get; }

        public event Action<IDialogResult>? RequestClose;

        private string _title = "仕分グループ選択";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private Combo? _distgroup;
        public Combo? DistGroup
        {
            get => _distgroup;
            set => SetProperty(ref _distgroup, value);
        }

        private int _distGroupIndex;
        public int DistGroupIndex
        {
            get => _distGroupIndex;
            set
            {
                SetProperty(ref _distGroupIndex, value);
                DistGroup = DistGroupIndex < 0 || !DistGroupCombo.Any() ? null : DistGroupCombo[DistGroupIndex];
                CanOK = DistGroupCombo.Any();
            }
        }

        private IList<Combo> _distgroupCombo = Array.Empty<Combo>();
        public IList<Combo> DistGroupCombo
        {
            get => _distgroupCombo;
            set => SetProperty(ref _distgroupCombo, value);
        }

        private DateTime _dtDelivery = DateTime.Now.AddDays(1);
        public DateTime DtDelivery
        {
            get => _dtDelivery;
            set
            {
                SetProperty(ref _dtDelivery, value);
                LoadCombo();
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _canOK = false;
        public bool CanOK
        {
            get => _canOK;
            set => SetProperty(ref _canOK, value);
        }


        public SelectDistGroupDlgViewModel(IDialogService dialogService)
        {
            OK = new DelegateCommand(() =>
            {
                ErrorMessage = string.Empty;

                if (DistGroup == null)
                {
                    return;
                }

                // ダイアログを閉じる
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
                {
                    { "CdDistGroup", DistGroup.CdDistGroup },
                    { "NmDistGroup", DistGroup.NmDistGroup },
                    { "DtDelivery", DtDelivery.ToString("yyyyMMdd") },
                }));
            }).ObservesCanExecute(() => CanOK);

            Enter = new DelegateCommand(() =>
            {
                OK.Execute();
            });

            Cancel = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            return;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            LoadCombo();
        }

        private void LoadCombo()
        {
            try
            {
                DistGroupIndex = -1;
                DistGroupCombo = ComboCreator.Create(DtDelivery.ToString("yyyyMMdd"));
                DistGroupIndex = 0;
            }
            catch (Exception e)
            {
                Syslog.Error($"DistGroupComboLoader():{e.Message}");
                ErrorMessage = e.Message;
            }
        }
    }
}