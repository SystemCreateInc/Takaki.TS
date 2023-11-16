using ColumnVisibilityLib.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;

namespace ColumnVisibilityLib.ViewModels
{
    public class SelectColumnVisibilityDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand OK { get; }
        public DelegateCommand Cancel { get; }

        public event Action<IDialogResult>? RequestClose;

        public string Title => "表示列選択";

        private List<string> _columnNames { get; set; } = new List<string>();

        private List<ColumnVisibilityInfo> _columnVisibilities = new List<ColumnVisibilityInfo>();
        public List<ColumnVisibilityInfo> ColumnVisibilities
        {
            get => _columnVisibilities;
            set => SetProperty(ref _columnVisibilities, value);
        }

        public SelectColumnVisibilityDlgViewModel()
        {
            OK = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
                {
                    { "ColumnVisibilities", ColumnVisibilities },
                }));
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
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            ColumnVisibilities = parameters.GetValue<List<ColumnVisibilityInfo>>("ColumnVisibilities");
        }
    }
}
