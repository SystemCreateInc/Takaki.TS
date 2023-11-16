using ProcessorLib.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProcessorLib
{
    public static class ServiceInstaller
    {
        public static bool ParseCommandLine(string[] args, string serviceName, string? displayName = null)
        {
            if (displayName == null)
            {
                displayName = serviceName;
            }

            try
            {
                if (args.SingleOrDefault(x => x == "-i") != null)
                {
                    RegistService(serviceName, displayName);
                    MessageBox.Show($"サービスに登録しました。 {serviceName}/{displayName}", serviceName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }

                if (args.SingleOrDefault(x => x == "-u") != null)
                {
                    UnregistService(serviceName);
                    MessageBox.Show("サービスを削除しました。", serviceName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, serviceName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }

        private static void RegistService(string serviceName, string displayName)
        {
            using (var scm = new ServiceManager())
            {
                var ea = Assembly.GetEntryAssembly();
                if (ea == null)
                {
                    throw new Exception("アッセンブリ情報が取得できませんでした。");
                }


                var path = Process.GetCurrentProcess().MainModule?.FileName
                    ?? throw new Exception("実行ファイルのパスを取得できませんでした");
                scm.CreateService(serviceName, displayName, path);
            }
        }

        private static void UnregistService(string serviceName)
        {
            using (var scm = new ServiceManager())
            {
                scm.UnregisterService(serviceName);
            }
        }
    }
}
