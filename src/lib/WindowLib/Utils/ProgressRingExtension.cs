using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using WindowLib.Views;

namespace WindowLib.Utils
{
    public static class ProgressRingExtension
    {
        public static void ShowProgressRing(this IDialogService dialogService, Task waitTask, string title)
        {
            var prms = new DialogParameters { { "Title", title }, { "Task", waitTask } };
            dialogService.ShowDialog(nameof(ProgressRing), prms, r => { });
        }
    }
}
