using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;
using WindowLib.Views;

namespace WindowLib.Utils
{
    public class WaitProgressDialog
    {
        // ウエイトダイアログ表示
        // PreMethod    前処理  
        // MainMethod   主処理
        // PostMethod   後処理
        // IDialogService dialogService,
        // IEventAggregator eventAggregator,

        static public void ShowProgress(
            string title,
            string message,
            Action<CancellationTokenSource> ?PreMethod,
            Action<CancellationTokenSource> ?MainMethod,
            Action<CancellationTokenSource> ?PostMethod,
            IDialogService dialogService,
            IEventAggregator eventAggregator,
            string dialogName = nameof(ProgressDialog))
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {

                try
                {
                    var task = Task.Run(() =>
                    {
                        // 前処理
                        if (PreMethod != null)
                           PreMethod(cancellationTokenSource);

                        // 主処理
                        if (MainMethod != null)
                           MainMethod(cancellationTokenSource);

                        // 後処理
                        if (PostMethod != null)
                           PostMethod(cancellationTokenSource);

                    // 早すぎるとProgressMessageが消えない場合があるのでウエイト
//                    Thread.Sleep(100);
                    });

                    var prm = new DialogParameters();
                    prm.Add("canceller", cancellationTokenSource);
                    prm.Add("task", task);
                    prm.Add("title", title);
                    prm.Add("message", message);

                    dialogService.ShowDialog(dialogName, prm, r => { });
                }
                catch(Exception e)
                {
                    string a = e.Message;
                }
            }
        }

    }
}
