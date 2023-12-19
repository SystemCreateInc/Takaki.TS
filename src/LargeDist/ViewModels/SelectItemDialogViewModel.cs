using ImTools;
using LargeDist.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LargeDist.ViewModels
{
    public class SelectItemDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand OKCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand UpCommand { get; }
        public DelegateCommand DownCommand { get; }

        private string? _cdGtin13;
        public string? CdGtin13
        {
            get => _cdGtin13;
            set => SetProperty(ref _cdGtin13, value);
        }

        private ObservableCollection<LargeDistItem> _items = new ObservableCollection<LargeDistItem>();
        public ObservableCollection<LargeDistItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private LargeDistItem? _selectedItem;
        public LargeDistItem? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public SelectItemDialogViewModel()
        {
            OKCommand = new(OK);
            CancelCommand = new(Cancel);
            UpCommand = new(Up);
            DownCommand = new(Down);
        }

        private void Down()
        {
            int idx = (SelectedItem == null ? -1 : Items.IndexOf(SelectedItem)) + 1;
            idx = idx < Items.Count ? idx : Items.Count - 1;
            SelectedItem = Items[idx];
        }

        private void Up()
        {
            int idx = (SelectedItem == null ? -1 : Items.IndexOf(SelectedItem)) - 1;
            idx = idx > 0 ? idx : 0;
            SelectedItem = Items[idx];
        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private void OK()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters()
            {
                {
                    "SelectedItem", SelectedItem
                }
            }));
        }

        public string Title => "商品選択";

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
            var param = parameters.GetValue<SelectItemDialogParam>("Param");
            Items = new(param.Items);
            SelectedItem = Items.First();
            CdGtin13 = Items.First().CdGtin13;
        }
    }
}
