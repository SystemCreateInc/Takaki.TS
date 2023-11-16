using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace WindowLib.ViewModels
{
    public class TabletMessageDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand? OK { get; set; }
        public DelegateCommand? Cancel { get; set; }
        public DelegateCommand? Yes { get; set; }
        public DelegateCommand? No { get; set; }
        public DelegateCommand? Abort { get; set; }
        public DelegateCommand? None { get; set; }
        public DelegateCommand? Retry { get; set; }
        public DelegateCommand? Ignore { get; set; }

        private bool _OKVisible = false;
        public bool OKVisible { get => _OKVisible; set => SetProperty(ref _OKVisible, value); }

        private bool _CancelVisible = false;
        public bool CancelVisible { get => _CancelVisible; set => SetProperty(ref _CancelVisible, value); }

        private bool _Yes = false;
        public bool YesVisible { get => _Yes; set => SetProperty(ref _Yes, value); }

        private bool _NoVisible = false;
        public bool NoVisible { get => _NoVisible; set => SetProperty(ref _NoVisible, value); }

        private bool _AbortVisible = false;
        public bool AbortVisible { get => _AbortVisible; set => SetProperty(ref _AbortVisible, value); }

        private bool _NoneVisible = false;
        public bool NoneVisible { get => _NoneVisible; set => SetProperty(ref _NoneVisible, value); }

        private bool _RetryVisible = false;
        public bool RetryVisible { get => _RetryVisible; set => SetProperty(ref _RetryVisible, value); }

        private bool _IgnoreVisible = false;
        public bool IgnoreVisible { get => _IgnoreVisible; set => SetProperty(ref _IgnoreVisible, value); }


        private string _title = "";
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        private string _message = "";
        public string Message { get => _message; set => SetProperty(ref _message, value); }
        private string _Icon = "information";
        public string Icon { get => _Icon; set => SetProperty(ref _Icon, value); }

        public TabletMessageDialogViewModel()
        {
            foreach (ButtonResult x in Enum.GetValues(typeof(ButtonResult)))
            {
                typeof(TabletMessageDialogViewModel).GetProperty(x.ToString())?.SetValue(this,
                    new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(x))));
            }
        }


        public event Action<IDialogResult>? RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = parameters.GetValue<string>("title");
            Message = parameters.GetValue<string>("message");
            Icon = parameters.GetValue<string>("icon") ?? "information";
            var buttons = parameters.GetValue<ButtonResult[]>("buttons");
            if (buttons == null)
            {
                buttons = new ButtonResult[] { ButtonResult.OK, ButtonResult.Cancel };
            }

            //  使用するボタンを表示する
            buttons.ForEach(x =>
            {
                var prop = typeof(TabletMessageDialogViewModel).GetProperty($"{x.ToString()}Visible");
                if (prop != null)
                {
                    prop.SetValue(this, true);
                }
            });
        }
    }
}
