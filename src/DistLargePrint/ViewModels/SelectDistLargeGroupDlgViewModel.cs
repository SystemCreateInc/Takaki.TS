﻿using DistLargePrint.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace DistLargePrint.ViewModels
{
    public class SelectDistLargeGroupDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand OK { get; }
        public DelegateCommand Cancel { get; }

        public string Title => "大仕分グループ選択画面";

        public event Action<IDialogResult>? RequestClose;


        private int _largeDistGroupIndex;
        public int LargeDistGroupIndex
        {
            get => _largeDistGroupIndex;
            set => SetProperty(ref _largeDistGroupIndex, value);
        }

        private List<Combo> _largeDistGroupCombo = new List<Combo>();
        public List<Combo> LargeDistGroupCombo
        {
            get => _largeDistGroupCombo;
            set => SetProperty(ref _largeDistGroupCombo, value);
        }

        private DateTime _deliveryDate;
        public DateTime DeliveryDate
        {
            get => _deliveryDate;
            set
            {
                SetProperty(ref _deliveryDate, value);
                CreateCombo();
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public SelectDistLargeGroupDlgViewModel()
        {
            OK = new DelegateCommand(() =>
            {
                Syslog.Debug("SelectDistLargeGroupDlgViewModel:OK");
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
                {
                    { "CdLargeGroup", LargeDistGroupCombo[LargeDistGroupIndex].Name },
                    { "DtDelivery", DeliveryDate.ToString("yyyyMMdd") },
                }));
            });

            Cancel = new DelegateCommand(() =>
            {
                Syslog.Debug("SelectDistLargeGroupDlgViewModel:Cancel");
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            ErrorMessage = string.Empty;

            // システム日付 + 1
            DeliveryDate = DateTime.Today.AddDays(1);
        }

        private void CreateCombo()
        {
            try
            {
                LargeDistGroupIndex = -1;
                LargeDistGroupCombo = ComboCreator.Create(DeliveryDate.ToString("yyyyMMdd"));
                LargeDistGroupIndex = 0;
            }
            catch (Exception e)
            {
                Syslog.Error($"CreateCombo:{e.Message}");
                ErrorMessage = e.Message;
            }
        }
    }
}