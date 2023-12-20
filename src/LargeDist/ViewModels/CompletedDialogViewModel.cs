using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LargeDist.ViewModels
{
    public class CompletedDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand OKCommand { get; }

        private string? _text;
        public string? Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }


        public CompletedDialogViewModel()
        {
            OKCommand = new(() => RequestClose?.Invoke(new DialogResult()));
        }

        public string Title => "大仕分終了";

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
            Text = parameters.GetValue<string>("Text");
        }
    }
}
