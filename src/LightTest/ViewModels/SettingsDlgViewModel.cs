using LightTest.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using TdDpsLib.Models;
using WindowLib.Utils;

namespace LightTest.ViewModels
{
    public class SettingsDlgViewModel : BindableBase, IDialogAware
    {
        private string _title = "点灯内容設定";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public event Action<IDialogResult> ?RequestClose;

        public DelegateCommand Save { get; }
        public DelegateCommand Cancel { get; }

        // 点灯点滅ボタン
        private bool[] _dispBlinkType = { true, false };

        public bool[] DispBlinkType
        {
            get => _dispBlinkType;
            set => SetProperty(ref _dispBlinkType, value);
        }

        // 点灯対象ボタン
        private bool[] _buttons = new bool[] { true, false, false, false, false };

        public bool[] Buttons
        {
            get => _buttons;
            set => SetProperty(ref _buttons, value);
        }

        // 表示器表示タイプ　0:論理 1:物理 2:888888
        private bool[] _dispType = { true, false, false };

        public bool[] DispType
        {
            get => _dispType;
            set => SetProperty(ref _dispType, value);
        }

        public DISPTYPE GetDispType
        {
            get
            {
                if (DispType[0])
                {
                    return DISPTYPE.DISPNO;
                }
                else if (DispType[1])
                {
                    return DISPTYPE.ADDR;
                }
                else
                {
                    return DISPTYPE.ALL8;
                }
            }
        }

        private readonly IDialogService _dialogService;

        public SettingsDlgViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Save = new DelegateCommand(() =>
            {
                try
                {
                    // 更新
                    LightDefaultManager.SaveDefaultButtons(Buttons);
                    LightDefaultManager.SaveDefaultDispTypes(DispType);
                    LightDefaultManager.SaveDefaultBlinkTypes(DispBlinkType);

                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
                catch (Exception e)
                {
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                    return;
                }
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
            Buttons = parameters.GetValue<bool[]>("_buttons");
            DispType = parameters.GetValue<bool[]>("_dispType");
            DispBlinkType = parameters.GetValue<bool[]>("_dispBlinkType");

#if false
            _datas = parameters.GetValue<List<TdPort>>("_datas");
            _cueernt = parameters.GetValue<TdPort>("_current");

            if (_cueernt == null)
            {
                Title = "追加";
                OkButtonText = "追加";
                TdUnitPortCodeReadonly = false;
            }
            else
            {
                TdUnitPortCode = _cueernt.TdUnitPortCode.ToString();
                TdPortCom = _cueernt.TdPortCom;
                TdUnitPortType = _cueernt.TdUnitPortType;
                TdComTypeeComboIndex = TdComTypeCombo.FindIndex(p => p.Index == _cueernt.TdUnitPortType);
                Title = "変更";
                OkButtonText = "変更";
                TdUnitPortCodeReadonly = true;
            }
#endif
        }
    }
}
