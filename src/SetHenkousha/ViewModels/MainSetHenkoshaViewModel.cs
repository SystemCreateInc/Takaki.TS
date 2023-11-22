using DbLib.Defs;
using DbLib.Extensions;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SetHenkosha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WindowLib.Utils;

namespace SetHenkosha.ViewModels
{
    public class MainSetHenkoshaViewModel : BindableBase
    {
        public DelegateCommand OK { get; }
        public DelegateCommand Cancel { get; }

        private readonly IDialogService _dialogService;

        private Shain? _shain;
        public Shain? Shain
        {
            get => _shain;
            set => SetProperty(ref _shain, value);
        }

        private IList<Shain> _shaingroupCombo = Array.Empty<Shain>();
        public IList<Shain> ShainCombo
        {
            get => _shaingroupCombo;
            set => SetProperty(ref _shaingroupCombo, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }



        public MainSetHenkoshaViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            LoadCombo();

            OK = new DelegateCommand(() =>
            {
                ErrorMessage = string.Empty;

                if (!Check())
                {
                    return;
                }

                Syslog.Debug("MainSetHenkoshaViewModel:OK");
                if (UpdateDB(Shain))
                {
                    Application.Current.MainWindow.Close();
                }
            });

            Cancel = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSetHenkoshaViewModel:Cancel");
                UpdateDB(null);
                Application.Current.MainWindow.Close();
            });
        }

        private bool UpdateDB(Shain? _syain)
        {
            try
            {
                ShainManager.Update(_syain);
            }
            catch (Exception e)
            {
                MessageDialog.Show(_dialogService, e.Message, "エラー");
                return false;
            }

            return true;
        }

        private bool Check()
        {
            if (Shain == null)
            {
                ErrorMessage = "社員を選択してください";
                return false;
            }

            return true;
        }
        private void LoadCombo()
        {
            try
            {
                ShainCombo = ShainComboLoader.GetShainCombos();
            }
            catch (Exception e)
            {
                Syslog.Error($"SyainComboLoader():{e.Message}");
                ErrorMessage = e.Message;
            }
        }
    }
}
