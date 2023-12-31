﻿using ImTools;
using Microsoft.IdentityModel.Tokens;
using Picking.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using WindowLib.Utils;

namespace Picking.ViewModels
{
    public class ShainDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand EnterShainCommand1 { get; }
        public DelegateCommand EnterShainCommand2 { get; }
        public DelegateCommand EnterShainCommand3 { get; }
        public DelegateCommand EnterShainCommand4 { get; }
        public DelegateCommand EnterShainCommand5 { get; }

        private string _title = "作業担当色設定";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public event Action<IDialogResult>? RequestClose = null;

        private List<DistColor> _datas = new List<DistColor>();

        public DelegateCommand Save { get; }
        public DelegateCommand Cancel { get; }

        private Shain _shain1 = new Shain();
        public Shain Shain1
        {
            get => _shain1;
            set => SetProperty(ref _shain1, value);
        }
        private Shain _shain2 = new Shain();
        public Shain Shain2
        {
            get => _shain2;
            set => SetProperty(ref _shain2, value);
        }
        private Shain _shain3 = new Shain();
        public Shain Shain3
        {
            get => _shain3;
            set => SetProperty(ref _shain3, value);
        }
        private Shain _shain4 = new Shain();
        public Shain Shain4
        {
            get => _shain4;
            set => SetProperty(ref _shain4, value);
        }

        private Shain _shain5 = new Shain();
        public Shain Shain5
        {
            get => _shain5;
            set => SetProperty(ref _shain5, value);
        }
        private string _colorBtnName1 = string.Empty;
        public string ColorBtnName1
        {
            get => _colorBtnName1;
            set => SetProperty(ref _colorBtnName1, value);
        }

        private string _colorBtnName2 = string.Empty;
        public string ColorBtnName2
        {
            get => _colorBtnName2;
            set => SetProperty(ref _colorBtnName2, value);
        }

        private string _colorBtnName3 = string.Empty;
        public string ColorBtnName3
        {
            get => _colorBtnName3;
            set => SetProperty(ref _colorBtnName3, value);
        }

        private string _colorBtnName4 = string.Empty;
        public string ColorBtnName4
        {
            get => _colorBtnName4;
            set => SetProperty(ref _colorBtnName4, value);
        }

        private string _colorBtnName5 = string.Empty;
        public string ColorBtnName5
        {
            get => _colorBtnName5;
            set => SetProperty(ref _colorBtnName5, value);
        }

        private List<Shain> _shainCombo = new List<Shain>();
        public List<Shain> ShainCombo
        {
            get => _shainCombo;
            set => SetProperty(ref _shainCombo, value);
        }

        private Shain _btnshain1 = new Shain();
        public Shain BtnShain1
        {
            get => _btnshain1;
            set => SetProperty(ref _btnshain1, value);
        }

        private Shain _btnshain2 = new Shain();
        public Shain BtnShain2
        {
            get => _btnshain2;
            set => SetProperty(ref _btnshain2, value);
        }

        private Shain _btnshain3 = new Shain();
        public Shain BtnShain3
        {
            get => _btnshain3;
            set => SetProperty(ref _btnshain3, value);
        }

        private Shain _btnshain4 = new Shain();
        public Shain BtnShain4
        {
            get => _btnshain4;
            set => SetProperty(ref _btnshain4, value);
        }
        private Shain _btnshain5 = new Shain();
        public Shain BtnShain5
        {
            get => _btnshain5;
            set => SetProperty(ref _btnshain5, value);
        }

        private bool _enableshain1 = true;
        public bool EnableShain1
        {
            get => _enableshain1;
            set => SetProperty(ref _enableshain1, value);
        }
        private bool _enableshain2 = true;
        public bool EnableShain2
        {
            get => _enableshain2;
            set => SetProperty(ref _enableshain2, value);
        }
        private bool _enableshain3 = true;
        public bool EnableShain3
        {
            get => _enableshain3;
            set => SetProperty(ref _enableshain3, value);
        }
        private bool _enableshain4 = true;
        public bool EnableShain4
        {
            get => _enableshain4;
            set => SetProperty(ref _enableshain4, value);
        }
        private bool _enableshain5 = true;
        public bool EnableShain5
        {
            get => _enableshain5;
            set => SetProperty(ref _enableshain5, value);
        }
        private string _cdShain1 = string.Empty;
        public string CdShain1
        {
            get => _cdShain1;
            set => SetProperty(ref _cdShain1, value);
        }
        private string _cdShain2 = string.Empty;
        public string CdShain2
        {
            get => _cdShain2;
            set => SetProperty(ref _cdShain2, value);
        }
        private string _cdShain3 = string.Empty;
        public string CdShain3
        {
            get => _cdShain3;
            set => SetProperty(ref _cdShain3, value);
        }
        private string _cdShain4 = string.Empty;
        public string CdShain4
        {
            get => _cdShain4;
            set => SetProperty(ref _cdShain4, value);
        }
        private string _cdShain5 = string.Empty;
        public string CdShain5
        {
            get => _cdShain5;
            set => SetProperty(ref _cdShain5, value);
        }

        private readonly IDialogService _dialogService;

        public ShainDlgViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Save = new DelegateCommand(() =>
            {
                // チェック
                if (Shain1==null)
                {
                    Shain1 = new Shain();
                }
                if (Shain2 == null)
                {
                    Shain2 = new Shain();
                }
                if (Shain3 == null)
                {
                    Shain3 = new Shain();
                }
                if (Shain4 == null)
                {
                    Shain4 = new Shain();
                }
                if (Shain5 == null)
                {
                    Shain5 = new Shain();
                }

                try
                {
                    var result = new DialogResult(ButtonResult.OK);

                    result.Parameters.Add("Shain1", Shain1);
                    result.Parameters.Add("Shain2", Shain2);
                    result.Parameters.Add("Shain3", Shain3);
                    result.Parameters.Add("Shain4", Shain4);
                    result.Parameters.Add("Shain5", Shain5);

                    RequestClose?.Invoke(result);
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

            EnterShainCommand1 = new(EnterShain1);
            EnterShainCommand2 = new(EnterShain2);
            EnterShainCommand3 = new(EnterShain3);
            EnterShainCommand4 = new(EnterShain4);
            EnterShainCommand5 = new(EnterShain5);

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
            _datas = parameters.GetValue<List<DistColor>>("_datas");
            BtnShain1 = parameters.GetValue<Shain>("colorbtn1");
            BtnShain2 = parameters.GetValue<Shain>("colorbtn2");
            BtnShain3 = parameters.GetValue<Shain>("colorbtn3");
            BtnShain4 = parameters.GetValue<Shain>("colorbtn4");
            BtnShain5 = parameters.GetValue<Shain>("colorbtn5");

            ShainCombo = PersonMstComboCreater.GetComboLists();
            ShainCombo.Insert(0, new Shain() { CdShain = "", NmShain = "" });

            ColorBtnName1 = _datas[0].DistColor_name;
            ColorBtnName2 = _datas[1].DistColor_name;
            ColorBtnName3 = _datas[2].DistColor_name;
            ColorBtnName4 = _datas[3].DistColor_name;
            ColorBtnName5 = _datas[4].DistColor_name;

            Shain1 = GetShainCombo(BtnShain1.CdShain);
            Shain2 = GetShainCombo(BtnShain2.CdShain);
            Shain3 = GetShainCombo(BtnShain3.CdShain);
            Shain4 = GetShainCombo(BtnShain4.CdShain);
            Shain5 = GetShainCombo(BtnShain5.CdShain);

            EnableShain1 = _datas[0].IsWorking() ? false : true;
            EnableShain2 = _datas[1].IsWorking() ? false : true;
            EnableShain3 = _datas[2].IsWorking() ? false : true;
            EnableShain4 = _datas[3].IsWorking() ? false : true;
            EnableShain5 = _datas[4].IsWorking() ? false : true;
        }
        Shain GetShainCombo(string cd_shain)
        {
            return ShainCombo.Find(x => x.CdShain == cd_shain) ?? new Shain();
        }
        private void EnterShain1()
        {
            EnterShain(1);
        }
        private void EnterShain2()
        {
            EnterShain(2);
        }
        private void EnterShain3()
        {
            EnterShain(3);
        }
        private void EnterShain4()
        {
            EnterShain(4);
        }
        private void EnterShain5()
        {
            EnterShain(5);
        }
        private void EnterShain(int syainidx)
        {
            var cdshain = CdShain1;
            switch(syainidx)
            {
                case 1: cdshain = CdShain1; break;
                case 2: cdshain = CdShain2; break;
                case 3: cdshain = CdShain3; break;
                case 4: cdshain = CdShain4; break;
                case 5: cdshain = CdShain5; break;
            }


            // 入力テキストが7桁未満なら先頭０を追加して７桁にする
            if (cdshain.Length < 7)
            {
                cdshain = cdshain.PadLeft(7, '0');
            }

            var tmp = GetShainCombo(cdshain);
            if (tmp != null)
            {
                switch (syainidx)
                {
                    case 1:
                        CdShain1 = cdshain;
                        Shain1 = tmp;
                        break;
                    case 2:
                        CdShain2 = cdshain;
                        Shain2 = tmp;
                        break;
                    case 3:
                        CdShain3 = cdshain;
                        Shain3 = tmp;
                        break;
                    case 4:
                        CdShain4 = cdshain;
                        Shain4 = tmp;
                        break;
                    case 5:
                        CdShain5 = cdshain;
                        Shain5 = tmp;
                        break;
                }
            }

            var direction = Keyboard.Modifiers == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
            UIElement? elementWithFocus = Keyboard.FocusedElement as UIElement;
            if (elementWithFocus != null)
            {
                // カーソル移動
                elementWithFocus.MoveFocus(new TraversalRequest(direction));

            }
        }
    }
}
