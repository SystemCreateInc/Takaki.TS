using LogLib;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WindowLib.Views;

namespace WindowLib.Utils
{
    [Flags]
    public enum ButtonMask
    {
        OK = 0x01,
        Cancel = 0x02,
        Abort = 0x04,
        Retry = 0x08,
        Ignore = 0x0f,
        Yes = 0x10,
        No = 0x20,
    }

    public class MessageDialog
    {
        public static ButtonResult Show(IDialogService dialogService, string messageBoxText, string caption, ButtonMask buttonMask = ButtonMask.OK, MessageBoxImage icon = MessageBoxImage.Warning)
        {
            return Show(dialogService, messageBoxText, caption, buttonMask, icon, nameof(MessageDialog));
        }

        private static ButtonResult Show(IDialogService dialogService, string messageBoxText, string caption, ButtonMask buttonMask = ButtonMask.OK, MessageBoxImage icon = MessageBoxImage.Warning, string dialogName = nameof(MessageDialog))
        {
            var buttons = new List<ButtonResult>();
            if (buttonMask.HasFlag(ButtonMask.OK))
            {
                buttons.Add(ButtonResult.OK);
            }

            if (buttonMask.HasFlag(ButtonMask.Cancel))
            {
                buttons.Add(ButtonResult.Cancel);
            }

            if (buttonMask.HasFlag(ButtonMask.Abort))
            {
                buttons.Add(ButtonResult.Abort);
            }

            if (buttonMask.HasFlag(ButtonMask.Retry))
            {
                buttons.Add(ButtonResult.Retry);
            }

            if (buttonMask.HasFlag(ButtonMask.Ignore))
            {
                buttons.Add(ButtonResult.Ignore);
            }

            if (buttonMask.HasFlag(ButtonMask.Yes))
            {
                buttons.Add(ButtonResult.Yes);
            }

            if (buttonMask.HasFlag(ButtonMask.No))
            {
                buttons.Add(ButtonResult.No);
            }

            return Show(dialogService, messageBoxText, caption, buttons.ToArray(), icon, dialogName);
        }

        public static Task<ButtonResult> ShowTabletAsync(IDialogService dialogService, string messageBoxText, string caption, ButtonMask buttonMask = ButtonMask.OK, MessageBoxImage icon = MessageBoxImage.Warning)
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return Show(dialogService, messageBoxText, caption, buttonMask, icon, nameof(TabletMessageDialog));
            }).Task;
        }

        public static Task<ButtonResult> ShowAsync(IDialogService dialogService, string messageBoxText, string caption, ButtonResult[] buttons, MessageBoxImage icon = MessageBoxImage.Warning, string dialogName = nameof(MessageDialog))
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return Show(dialogService, messageBoxText, caption, buttons, icon, dialogName);
            }).Task;
        }

        public static ButtonResult Show(IDialogService dialogService, string messageBoxText, string caption, ButtonResult[] buttons, MessageBoxImage icon = MessageBoxImage.Warning, string dialogName = nameof(MessageDialog))
        {
            string iconText = "";
            switch (icon)
            {
                case MessageBoxImage.Question:
                    iconText = "HelpCircle";
                    break;

                //case MessageBoxImage.Warning:
                case MessageBoxImage.Exclamation:
                    iconText = "AlertCircle";
                    break;

                //case MessageBoxImage.Hand:
                //case MessageBoxImage.Stop:
                case MessageBoxImage.Error:
                    iconText = "CloseCircle";
                    break;

                //case MessageBoxImage.Asterisk:
                case MessageBoxImage.Information:
                    iconText = "Information";
                    break;
            }

            ButtonResult result = ButtonResult.None;
            try
            {
                dialogService.ShowDialog(dialogName, new DialogParameters
                {
                    { "title", caption },
                    { "message", messageBoxText },
                    { "buttons", buttons },
                    { "icon", iconText }
                }, r => result = r.Result);
            }
            catch (Exception e)
            {
                //  prismで表示できないので代わりに表示
                //  しかしここに来るということはたぶんこの後の処理は続行できない
                Syslog.Warn($"FAIL WindowLib Dialog {e.Message}");

                switch (MessageBox.Show(messageBoxText, caption))
                {
                    case MessageBoxResult.None:
                        result = ButtonResult.None;
                        break;

                    case MessageBoxResult.OK:
                        result = ButtonResult.OK;
                        break;

                    case MessageBoxResult.Cancel:
                        result = ButtonResult.Cancel;
                        break;

                    case MessageBoxResult.Yes:
                        result = ButtonResult.Yes;
                        break;

                    case MessageBoxResult.No:
                        result = ButtonResult.No;
                        break;
                }
            }

            Syslog.Info($"MessageDialog: {caption}/{messageBoxText} buttons: {string.Join(",", buttons.Select(x => x.ToString()))} icon: {iconText}, result: {result}");

            return result;
        }
    }
}
