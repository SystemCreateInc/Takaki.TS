using LogLib;
using System;
using System.Printing;

namespace PrintLib
{
    public static class ReservedPrinter
    {
        public static string Name { get; set; } = "";
        public static string GetNetWorkServer()
        {
            if (Name.Substring(0, 2) == @"\\")
            {
                int idx = Name.LastIndexOf('\\');
                if (idx != -1)
                {
                    return Name.Substring(0, idx);
                }
            }

            return "";
        }

        public static string GetNetWorkServerPrinter()
        {
            if (Name.Substring(0, 2) == @"\\")
            {
                int idx = Name.LastIndexOf('\\');
                if (idx != -1)
                {
                    return Name.Substring(idx + 1, Name.Length - idx - 1);
                }
            }

            return "";
        }

        public static PrintQueue GetPrintQueue()
        {
            // プリンタ指定がある場合、指定プリンタを選択済みにする
            if (Name != "")
            {
                try
                {
                    Syslog.Debug($"printerName::[{Name}] sv[{GetNetWorkServer()}] pr[{GetNetWorkServerPrinter()}]");

                    // ネットワークプリンタ
                    if (GetNetWorkServer() == "")
                    {
                        return new PrintQueue(new LocalPrintServer(), Name);
                    }
                    else
                    {
                        return new PrintQueue(new PrintServer(GetNetWorkServer()), GetNetWorkServerPrinter());
                    }
                }
                catch (Exception e)
                {
                    Syslog.Warn($"GetPrintQueue: Can not found printer {e.Message}");

                    foreach (PrintQueue queue in new PrintServer().GetPrintQueues())
                    {
                        // プリンタ名を出力
                        Syslog.Warn($"printerlists:[{queue.FullName}][{queue.Name}]");
                    }

                    throw;
                }
            }

            return LocalPrintServer.GetDefaultPrintQueue();
        }
    }
}
