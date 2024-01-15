using LogLib;
using Microsoft.Extensions.Configuration;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SelDistGroupLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private int _distGroupIndex;
        public int DistGroupIndex
        {
            get => _distGroupIndex;
            set
            {
                SetProperty(ref _distGroupIndex, value);
                DistGroup = DistGroupIndex < 0 || !DistGroupCombo.Any() ? null : DistGroupCombo[DistGroupIndex];
                CanOK = DistGroupCombo.Any();
            }
        }

        private IList<DistGroup> _distgroupCombo = Array.Empty<DistGroup>();
        public IList<DistGroup> DistGroupCombo
        {
            get => _distgroupCombo;
            set => SetProperty(ref _distgroupCombo, value);
        }

        private DateTime _dt_delivery = DateTime.Now.AddDays(1);
        public DateTime DtDelivery
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

        private bool _canOK = false;
        public bool CanOK
        {
            get => _canOK;
            set => SetProperty(ref _canOK, value);
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

                // ブロック設定
                if (DistGroup!=null)
                {
                    DistGroup.CdBlock = BlockLoader.GetBlock();

                    var config = new ConfigurationBuilder()
                    .AddJsonFile("common.json", true, true)
                    .Build();

                    DistGroup.IdPc = int.Parse(config.GetSection("pc")?["idpc"] ?? "1");
                }

                // ダイアログを閉じる
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
                {
                    { "DistGroup", DistGroup },
                    { "DtDelivery", DtDelivery },
                }));
            }).ObservesCanExecute(() => CanOK);

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
                DistGroupIndex = -1;
                DistGroupCombo = DistGroupComboLoader.GetDistGroupCombos(DtDelivery.ToString("yyyyMMdd"));
                DistGroupIndex = 0;
            }
            catch (Exception e)
            {
                Syslog.Error($"DistGroupComboLoader():{e.Message}");
                ErrorMessage = e.Message;
            }
        }
    }
}
