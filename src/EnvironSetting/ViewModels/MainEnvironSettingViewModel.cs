using EnvironSetting.Loader;
using EnvironSetting.Models;
using LogLib;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Windows;
using WindowLib.Utils;

namespace EnvironSetting.ViewModels
{
    public class MainEnvironSettingViewModel : BindableBase
    {
        public DelegateCommand RegistCmd { get; }
        public DelegateCommand CancelCmd { get; }

        private readonly IDialogService _dialogService;

        private List<string> _blocks = new List<string>();
        public List<string> Blocks
        {
            get => _blocks;
            set => SetProperty(ref _blocks, value);
        }

        private string _block = string.Empty;
        public string Block
        {
            get => _block;
            set
            {
                SetProperty(ref _block, value);
                CanRegist = !Block.IsNullOrEmpty();
            }
        }

        private bool _canRegist = false;
        public bool CanRegist
        {
            get => _canRegist;
            set => SetProperty(ref _canRegist, value);
        }

        private int _idPc;

        private bool _isExist { get; set; }

        public MainEnvironSettingViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            Initialize();

            RegistCmd = new DelegateCommand(Regist).ObservesCanExecute(() => CanRegist);
            CancelCmd = new DelegateCommand(() =>
            {
                Syslog.Debug("MainEnvironSettingViewModel:Cancel");
                Application.Current.MainWindow.Close();
            });
        }

        private void Initialize()
        {
            SetIdPc();
            SetBlockCombo();
            LoadDatas();
        }

        private void SetIdPc()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("common.json", true, true)
                .Build();

            _idPc = int.Parse(config.GetSection("pc")?["idpc"] ?? "1");
        }

        private void SetBlockCombo()
        {
            try
            {
                Blocks = BlockLoader.GetBlocks().ToList();
                if (!Blocks.Any())
                {
                    MessageDialog.Show(_dialogService, "ブロックを追加して下さい。終了します", "ブロック未設定");
                    Application.Current.MainWindow.Close();
                }

                Block = Blocks.First();
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private void LoadDatas()
        {
            try
            {
                var block = PcEnvironLoader.GetBlock(_idPc);
                _isExist = block is not null;
                Block = block ?? Blocks.First();
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private void Regist()
        {
            Syslog.Debug("MainEnvironSettingViewModel:Regist");

            try
            {
                if (_isExist)
                {
                    TbPcEntityManager.Update(_idPc, Block);
                }
                else
                {
                    TbPcEntityManager.Insert(_idPc, Block);
                }

                Application.Current.MainWindow.Close();
            }
            catch (Exception ex)
            {

                MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }
    }
}
