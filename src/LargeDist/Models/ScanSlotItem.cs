using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeDist.Models
{
    public class ScanSlotItem : BindableBase
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

        private string? _title = string.Empty;
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private int? _csUnit;
        public int? NuBoxUnit
        {
            get => _csUnit;
            set => SetProperty(ref _csUnit, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _isHead;
        public bool IsHead
        {
            get => _isHead;
            set => SetProperty(ref _isHead, value);
        }
        public bool IsEmpty => Item == null;

        private bool _stopped;
        public bool IsStopped
        {
            get => _stopped;
            set
            {
                SetProperty(ref _stopped, value);
                Item?.SetStopped(value);
            } 
        }

        private int? _totalPieceCount;
        public int? TotalPieceCount
        {
            get => _totalPieceCount;
            set => SetProperty(ref _totalPieceCount, value);
        }

        private int? _boxCount;
        public int? BoxCount
        {
            get => _boxCount;
            set => SetProperty(ref _boxCount, value);
        }

        private int? _pieceCount;
        public int? PieceCount
        {
            get => _pieceCount;
            set => SetProperty(ref _pieceCount, value);
        }

        public int GridPosition { get; }

        public int ScanOrder { get; set; }

        public ScanSlotItem(int position)
        {
            GridPosition = position;
        }

        internal void UpdateItem()
        {
            if (_item != null)
            {
                Title = _item.NmHinSeishikimei;
                NuBoxUnit = _item.NuBoxUnit;
                TotalPieceCount = _item.Input.Total;
                BoxCount = _item.Input.Box;
                PieceCount = _item.Input.Piece;
            }
            else
            {
                Title = "";
                NuBoxUnit = null;
                TotalPieceCount = null;
                BoxCount = null;
                PieceCount = null;
            }
        }
    }
}
