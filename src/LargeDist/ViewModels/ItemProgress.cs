using Prism.Mvvm;
using System;

namespace LargeDist.ViewModels
{
    public class ItemProgress : BindableBase
    {
        private int _completed;
        public int Completed
        {
            get => _completed;
            set
            {
                SetProperty(ref _completed, value);
                UpdatePercent();
            }
        }

        private int _total;
        public int Total
        {
            get => _total;
            set 
            {
                SetProperty(ref _total, value);
                UpdatePercent();
            }
        }

        private int _percent;
        public int Percent
        {
            get => _percent;
            set => SetProperty(ref _percent, value);
        }

        private void UpdatePercent()
        {
            Percent = Total == 0 ? 0 : Completed * 100 / Total;
        }
    }
}