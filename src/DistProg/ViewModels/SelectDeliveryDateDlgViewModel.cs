using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace DistProg.ViewModels
{
    public class SelectDeliveryDateDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand OK { get; }
        public DelegateCommand Cancel { get; }

        public string Title => "納品日選択";
        public event Action<IDialogResult>? RequestClose;

        private DateTime _date = DateTime.Today.AddDays(1);
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public SelectDeliveryDateDlgViewModel()
        {
            OK = new DelegateCommand(() =>
            {
                Syslog.Debug("SelectDeliveryDateDlgViewModel:OK");
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
                {
                    { "Date", Date }
                }));
            });

            Cancel = new DelegateCommand(() =>
            {
                Syslog.Debug("SelectDeliveryDateDlgViewModel:Cancel");
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
