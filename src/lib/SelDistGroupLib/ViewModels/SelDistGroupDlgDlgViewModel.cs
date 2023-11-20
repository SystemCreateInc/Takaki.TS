using Azure;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SelDistGroupLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SelDistGroupLib.ViewModels
{
    public class SelDistGroupDlgViewModel : BindableBase, IDialogAware
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

        private DistGroup? _distgroup;
        public DistGroup? DistGroup
        {
            get => _distgroup;
            set => SetProperty(ref _distgroup, value);
        }

        private IList<DistGroup> _distgroupCombo = Array.Empty<DistGroup>();
        public IList<DistGroup> DistGroupCombo
        {
            get => _distgroupCombo;
            set => SetProperty(ref _distgroupCombo, value);
        }

        private DateTime _dt_delivery = DateTime.Now.AddDays(1);
        public DateTime DT_DELIVERY
        {
            get => _dt_delivery;
            set
            {
                SetProperty(ref _dt_delivery, value);
                LoadCombo();
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }


        public SelDistGroupDlgViewModel(IDialogService dialogService)
        {
            OK = new DelegateCommand(() =>
            {
                ErrorMessage = string.Empty;

                if (!Check())
                {
                    return;
                }

                // ダイアログを閉じる
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
                {
                    { "DistGroup", DistGroup },
                    { "DT_DELIVERY", DT_DELIVERY },
                }));
            });

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
        private bool Check()
        {
            if (DistGroup == null)
            {
                ErrorMessage = "仕分グループを選択してください";
                return false;
            }

            return true;
        }
        private void LoadCombo()
        {
            try
            {
                DistGroupCombo = DistGroupComboLoader.GetDistGroupCombos(DT_DELIVERY.ToString("yyyyMMdd"));
                DistGroup = DistGroupCombo.FirstOrDefault();
            }
            catch (Exception e)
            {
                Syslog.Error($"DistGroupComboLoader():{e.Message}");
                ErrorMessage = e.Message;
            }
        }
    }
}
