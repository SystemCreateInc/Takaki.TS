using LogLib;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Reflection;

namespace ProcessorLib
{
    public class TaskTrayIcon
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly Icon _icon;

        public TaskTrayIcon(IHostApplicationLifetime lifetime, Icon icon) 
        {
            _lifetime = lifetime;
            _icon = icon;
        }

        public void Show()
        {
            if (!WindowsServiceHelpers.IsWindowsService())
            {
                Syslog.Info("Running user mode");
                StartStaTask(() => ShowIcon());
            }
        }

        private void ShowIcon()
        {
            var icon = new NotifyIcon()
            {
                Icon = _icon,
                Text = Assembly.GetEntryAssembly()?.GetName().Name,
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
                {
                    Items =
                    {
                        new ToolStripMenuItem
                        {
                            Text = "終了"
                        }
                    }
                }
            };

            icon.ContextMenuStrip.Items[0].Click += (sender, e) =>
            {
                _lifetime.StopApplication();
                Application.Exit();
            };

            Application.Run();
            Syslog.Debug("Icon Exit");
        }

        private Task StartStaTask(Action start)
        {
            var tcs = new TaskCompletionSource<bool>();
            var thread = new Thread(() =>
            {
                try
                {
                    start();
                    tcs.SetResult(true);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }
    }
}
