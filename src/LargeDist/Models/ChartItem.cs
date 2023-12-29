using Prism.Mvvm;
using System;

namespace LargeDist.Models
{
    public class ChartItem : BindableBase
    {
        private LargeDistItem? _item;
        public LargeDistItem? Item
        {
            get => _item;
            set
            {
                _item = value;
                UpdateItem();
            }
        }

        private string? _text1;
        public string? Text1
        {
            get => _text1;
            set => SetProperty(ref _text1, value);
        }

        private string? _text2;
        public string? Text2
        {
            get => _text2;
            set => SetProperty(ref _text2, value);
        }

        private string? _text3;
        public string? Text3
        {
            get => _text3;
            set => SetProperty(ref _text3, value);
        }

        private string? _text4;
        public string? Text4
        {
            get => _text4;
            set => SetProperty(ref _text4, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _total;

        public ChartItem(bool total = false)
        {
            _total = total;
        }


        internal void UpdateItem()
        {
            if (_item != null)
            {
                Text1 = _total ? "合計" : _item.CdBlock;
                Text2 = _item.Result.Total == 0 ? "" : $"{_item.Result.Box}/{_item.Result.Piece}";
                Text3 = $"{_item.Input.Box}/{_item.Input.Piece}";
                Text4 = _item.Input.Total.ToString();
            }
            else
            {
                Clear();
            }
        }

        private void Clear()
        {
            Text1 = "";
            Text2 = "";
            Text3 = "";
            Text4 = "";
        }
    }
}
