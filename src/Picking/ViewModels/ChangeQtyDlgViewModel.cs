using Azure;
using ControlzEx.Standard;
using LogLib;
using Picking.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SelDistGroupLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Picking.ViewModels
{
    public class ChangeQtyDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand Enter { get; }
        public DelegateCommand OK { get; }
        public DelegateCommand Cancel { get; }

        public event Action<IDialogResult>? RequestClose;

        private string _title = "数量変更";
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

        private DistDetail? _distdetail;
        public DistDetail? DistDetail
        {
            get => _distdetail;
            set => SetProperty(ref _distdetail, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private string _tokuisakitext = string.Empty;
        public string TokuisakiText
        {
            get => _tokuisakitext;
            set => SetProperty(ref _tokuisakitext, value);
        }
        private string _opstext = string.Empty;
        public string OpsText
        {
            get => _opstext;
            set => SetProperty(ref _opstext, value);
        }

        private string _cstitletext = string.Empty;
        public string CsTitleText
        {
            get => _cstitletext;
            set => SetProperty(ref _cstitletext, value);
        }

        private string _cs = "";
        public string Cs
        {
            get => _cs;
            set
            {
                SetProperty(ref _cs, value);
                UpdatePsAll();
            }
        }

        private string _ps = "";
        public string Ps
        {
            get => _ps;
            set
            {
                SetProperty(ref _ps, value);
                UpdatePsAll();
            }
        }

        private int _psall = 0;
        public int PsAll
        {
            get => _psall;
            set => SetProperty(ref _psall, value);
        }

        private int _opsall = 0;
        public int OpsAll
        {
            get => _opsall;
            set => SetProperty(ref _opsall, value);
        }

        private int _lrpsall = 0;
        public int LrpsAll
        {
            get => _lrpsall;
            set => SetProperty(ref _lrpsall, value);
        }

        private int _csunit = 0;
        public int Csunit
        {
            get => _csunit;
            set => SetProperty(ref _csunit, value);
        }


        private int _tokuisakitotal = 0;


        public ChangeQtyDlgViewModel(IDialogService dialogService)
        {
            OK = new DelegateCommand(() =>
            {
                ErrorMessage = string.Empty;

                if (!Check())
                {
                    return;
                }

                DistDetail!.Dops = PsAll;

                // ダイアログを閉じる
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
                {
                    { "DistDetail", DistDetail },
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
            DistDetail = parameters.GetValue<DistDetail>("CurrentDistDetail");
            _tokuisakitotal = parameters.GetValue<int>("TokuisakiTotal");

            Csunit = DistDetail.GetCsunit;
            OpsAll = DistDetail.Ops - DistDetail.Drps;
            LrpsAll = DistDetail.Lrps - DistDetail.Drps;
            Cs = string.Format("{0}", (DistDetail.Dops - DistDetail.Drps) / Csunit);
            Ps = string.Format("{0}", (DistDetail.Dops - DistDetail.Drps) % Csunit);
            UpdatePsAll();

            if (_tokuisakitotal == 0)
            {
                TokuisakiText = string.Format("得意先コード{0} {1}"
                    , DistDetail.CdTokuisaki
                    , DistDetail.NmTokuisaki
                );

                OpsText = string.Format("配分数:{0}　配分済数:{1}"
                    , DistDetail.Display_Ops_CsPs_Text
                    , DistDetail.Display_DRps_CsPs_Text
                );

                CsTitleText = "得意先配分数";
            }
            else
            {
                TokuisakiText = "";
                OpsText = string.Format("予定総数:{0}　済総数:{1}"
                    , DistDetail.Display_Ops_CsPs_Text
                    , DistDetail.Display_DRps_CsPs_Text
                );

                CsTitleText = "配分総数";
            }

        }
        private bool Check()
        {
            int cs = 0, ps = 0;
            int.TryParse(Cs, out cs);
            int.TryParse(Ps, out ps);

            int all = (cs * DistDetail!.GetCsunit) + ps;

            if (OpsAll < all)
            {
                if (_tokuisakitotal == 0)
                    ErrorMessage = "配分数をオーバーしています。";
                else
                    ErrorMessage = "予定総数をオーバーしています。";
                return false;
            }
            if (OpsAll < DistDetail!.Drps)
            {
                ErrorMessage = "済み数より少ないです。";
                return false;
            }
            if (LrpsAll < all)
            {
                ErrorMessage = "大仕分済み数をオーバーしています。";
                return false;
            }
            if (LrpsAll < DistDetail!.Drps)
            {
                ErrorMessage = "済み数より少ないです。";
                return false;
            }

            return true;
        }

        private void UpdatePsAll()
        {
            int cs=0, ps=0;
            int.TryParse(Cs, out cs);
            int.TryParse(Ps, out ps);

            PsAll = (cs * Csunit) + ps;
            ErrorMessage = string.Empty;
        }
    }
}
