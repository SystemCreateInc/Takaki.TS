using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SearchBoxLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchBoxLib.ViewModels
{
    public class SearchBoxDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand Enter { get; }

        public DelegateCommand OK { get; }
        public DelegateCommand Cancel { get; }

        public event Action<IDialogResult>? RequestClose;

        private string _title = "検索ボックス";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        private List<Row> _rows = new List<Row>();
        public List<Row> Rows
        {
            get => _rows;
            set => SetProperty(ref _rows, value);
        }

        public SearchBoxDlgViewModel()
        {
            OK = new DelegateCommand(() =>
            {
                // ダイアログを閉じる
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
                {
                    { "SearchFields", GetSearchFields() },
                    { "SearchText", Text.Trim() },
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
            try
            {
                // 3つ区切りで表示
                var contents = parameters.GetValue<List<Content>>("Contents");
                Rows = contents.Select((v, i) => new { v, i })
                    .GroupBy(x => x.i / 3)
                    .Select(y => new Row() { Contents = y.Select(x => x.v) })
                    .ToList();
            }
            catch (Exception e)
            {
                Syslog.Error($"SearchBoxDlgOpened():{e.Message}");
            }
        }

        private IEnumerable<string> GetSearchFields()
        {
            // 表示用に入れ子にしてあるので配列を平坦化
            var contents = Rows.SelectMany(x => x.Contents.Select(x => x));

            // チェックがあればその項目のみ、なければ全てが検索対象
            return contents.Any(x => x.IsChecked)
                ? contents.Where(x => x.IsChecked).Select(x => x.TableName)
                : contents.Select(x => x.TableName);
        }
    }
}
