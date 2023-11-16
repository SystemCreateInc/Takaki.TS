using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using SyslogCS.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogLib
{
    internal class SyslogCS : IDisposable
    {
        private static string MachineName { get; set; } = string.Empty;
        private static string ExeName { get; set; } = string.Empty;
        private static ModuleInfo ModuleInfo { get; set; } = new ModuleInfo();
        private const int maxLogFiles = 10;
        private static List<LogFileInfo> LogFileInfos { get; set; } = new List<LogFileInfo>();

        private static int ExpDays { get; set; }
        private static bool Flush { get; set; }
        private static long SplitSize { get; set; }
        private static string Prefix { get; set; } = string.Empty;
        private static string FileName { get; set; } = string.Empty;
        private static Mutex? Mutex { get; set; }
        private static int ProcessId { get; set; }
        private static int ThreadId { get; set; } = 0;
        private static Severity LogMask { get; set; } = Severity.Debug;

        // Syslogが呼ばれた時の初期化
        public static void SLInit(int expDays, string logRoot, int trunc = 0)
        {
            Initialize();

            if (expDays != 0)
            {
                ExpDays = expDays;
            }

            if (!string.IsNullOrEmpty(logRoot))
            {
                var info = LogFileInfos.FirstOrDefault();
                if (info != null)
                {
                    info.LogRoot = logRoot;
                }
            }

            if (ExpDays != 0 || (Truncate)trunc == Truncate.Trunc)
            {
                SLTruncExpiredLog();
            }

            SLPrintf(Severity.Info, "------------- START");

            // スタート情報出力
            //OutStartInfo();
        }

        // 初期化
        public static void Initialize()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // マシン名の取得
            MachineName = Environment.MachineName;
            GetModuleInfo();
            ExeName = ModuleInfo!.ExeName;
            LoadConfig();
            Mutex = new Mutex(false, "Systemcreate.Syslog");
            ProcessId = Process.GetCurrentProcess().Id;
        }

        // 実行モジュールの情報取得
        public static void GetModuleInfo()
        {
            // 実行パスの取得
            var path = Process.GetCurrentProcess().MainModule?.FileName ?? "";

            // バージョンの取得
            var vi = FileVersionInfo.GetVersionInfo(path);
            ModuleInfo = new ModuleInfo();
            ModuleInfo.Path = path;
            ModuleInfo.Version = vi?.FileVersion ?? "";
            // スペシャルビルドの取得
            ModuleInfo.Special = vi?.SpecialBuild ?? "";
            GetFileDate();
        }

        // 日付の取得
        private static void GetFileDate()
        {
            var fi = new FileInfo(ModuleInfo.Path);

            if (!fi.Exists)
            {
                return;
            }

            ModuleInfo.CreationTime = fi.CreationTime;
            ModuleInfo.LastAccessTime = fi.LastAccessTime;
            ModuleInfo.LastWriteTime = fi.LastWriteTime;
            ModuleInfo.ExeName = fi.Name;
        }

        // コンフィグファイルの読み込み
        private static void LoadConfig()
        {
            ExpDays = 30;
            Flush = true;
            SplitSize =  0;
            Prefix = "DL";
            FileName = "log.txt";

            for (int i = 0; i < maxLogFiles; i++)
            {
                LogFileInfos.Add(new LogFileInfo
                {
                    Writer = null,
                    LogRoot = "",
                });
            }
        }

        // ログ削除
        private static void SLTruncExpiredLog()
        {
            var logRoot = LogFileInfos.First().LogRoot;
            if (string.IsNullOrEmpty(logRoot))
            {
                return;
            }

            var directories = new DirectoryInfo(logRoot);
            if (!directories.Exists)
            {
                return;
            }

            var dirAll = directories.GetDirectories($"{Prefix}*");

            // ログフォルダが無ければ中断
            if (dirAll.Length == 0)
            {
                return;
            }

            // 基準のフォルダ名を作成
            var standardDate = DateTime.Today.AddDays(-ExpDays);
            var standardDirName = Prefix + standardDate.ToString("yyyyMMdd");

            foreach (var dir in dirAll)
            {
                if ((dir.Attributes & FileAttributes.System) == FileAttributes.System
                    || (dir.Attributes & FileAttributes.Directory) != FileAttributes.Directory
                    || dir.Name.CompareTo(standardDirName) > 0)
                {
                    continue;
                }

                try
                {
                    DeleteDirectory(dir.FullName);
                    SLPrintf(Severity.Info, $"期限切れログを削除しました。 [{dir.FullName}]");
                }
                catch (Exception e)
                {
                    SLPrintf(Severity.Info, $"期限切れログを削除出来ませんでした。[{e.Message.TrimEnd('\r', '\n')}][{dir.FullName}]");
                }
            }
        }

        // ディレクトリ削除
        private static void DeleteDirectory(string path)
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*", SearchOption.AllDirectories);

            if (files.Any(x => (x.Attributes & FileAttributes.System) == FileAttributes.System))
            {
                return;
            }

            // サブフォルダ含む
            dir.Delete(true);
        }

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        public static void SLPrintf(Severity severity, string str)
        {
            if (severity > LogMask)
            {
                return;
            }

            try
            {
                try
                {
                    Mutex?.WaitOne();
                }
                catch (AbandonedMutexException)
                {
                    // 問題なし
                }

                if (ThreadId == 0)
                {
                    ThreadId = (int)GetCurrentThreadId();
                }

                var now = DateTime.Now;
                var header = $"{now:yyyy-MM-dd HH:mm:ss.fff} {MachineName} {ExeName}[{ProcessId}-{ThreadId}] {severity}:";
                var text = header + str;

                foreach (var info in LogFileInfos)
                {
                    if (string.IsNullOrEmpty(info.LogRoot))
                    {
                        break;
                    }

                    if (!CheckLogFile(now, info))
                    {
                        continue;
                    }

                    // 通常ログ書き込み
                    try
                    {
                        info.Writer?.BaseStream.Seek(0, SeekOrigin.End);
                        info.Writer?.WriteLine(text);

                        if (Flush)
                        {
                            info.Writer?.Flush();
                        }
                    }
                    catch
                    {
                        info.Writer?.Close();
                        info.Writer?.Dispose();
                        info.Writer = null;
                    }
                }
            }
            finally
            {
                // 確実に解放する
                Mutex?.ReleaseMutex();
            }
        }

        // ログファイル確認
        private static bool CheckLogFile(DateTime now, LogFileInfo info)
        {
            try
            {
                if (info.Writer == null || now.Date != info.CurrentDate)
                {
                    if (info.Writer != null)
                    {
                        info.Writer.Close();
                        info.Writer.Dispose();
                        info.Writer = null;
                    }

                    OpenLogFile(info);
                }

                if (SplitSize > 0)
                {
                    SplitCheck(info);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void OpenLogFile(LogFileInfo info)
        {
            UpdateLogDir(info);
            info.LogFilePath = Path.GetFullPath(Path.Combine(info.LogPath, FileName));
            FileInfo file = new FileInfo(info.LogFilePath);

            // フォルダが存在しない場合は事前に作成
            if (file.Directory?.Exists == false)
            {
                file.Directory.Create();
            }

            info.Writer = new StreamWriter(info.LogFilePath, Encoding.GetEncoding("SJIS"), new FileStreamOptions
            {
                Mode = FileMode.Append,
                Access = FileAccess.Write,
                Share = FileShare.ReadWrite,
            });
        }

        // ログ情報のパスを新規作成して更新
        private static void UpdateLogDir(LogFileInfo info)
        {
            info.CurrentDate = DateTime.Today;
            var path = $"{Prefix}{info.CurrentDate:yyyyMMdd}";
            info.LogPath = Path.GetFullPath(Path.Combine(info.LogRoot, path));
        }

        // ログファイル分割
        private static void SplitCheck(LogFileInfo info)
        {
            var fi = new FileInfo(info.LogFilePath);
            if (!fi.Exists || fi.Length < SplitSize)
            {
                return;
            }

            info.Writer?.Flush();

            if (SLCopy(fi, true))
            {
                using (var fs = new FileStream(fi.FullName, FileMode.Open))
                {
                    fs.SetLength(0);
                }
            }
        }

        // ログ領域にファイルコピー
        public static bool SLCopy(FileInfo fi, bool silent = false)
        {
            var path = Path.GetFullPath(Path.Combine(LogFileInfos.First().LogPath, Path.GetFileName(fi.FullName)));

            for (var i = 0; i < 1000; i++)
            {
                var fileName = $"{path}.{DateTime.Now.ToString("yyyyMMdd-HHmmss")}-{i}";

                try
                {
                    fi.CopyTo(fileName);
                    if (!silent)
                    {
                        SLPrintf(Severity.Info, $"ファイルをバックアップしました。[{fi.FullName}]->[{fileName}]");
                    }

                    return true;
                }
                catch (Exception e)
                {
                    if (!silent)
                    {
                        SLPrintf(Severity.Info, $"コピーに失敗しました。[{fi.FullName}]->[{fileName}] [{e.Message}]");
                    }
                    Thread.Sleep(100);
                    continue;
                }
            }

            return false;
        }

        // スタート情報出力
        private static void OutStartInfo()
        {
            OutOSInfo();
            OutModuleList();
        }

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int RtlGetVersion([In, Out] ref OSVersionInfoEx lpVersionInformation);


        // OS情報出力
        private static void OutOSInfo()
        {
            // バージョン情報取得
            var os = Environment.OSVersion;
            var osMajor = os.Version.Major;
            var osMinor = os.Version.Minor;
            var releaseId = GetRegValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId");
            var build = GetRegValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
            var ubr = GetRegValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR");

            var info = new OSVersionInfoEx();
            var result = RtlGetVersion(ref info);
            if (result == 0)
            {
                // OSのメジャーバージョンとマイナーバージョンだけはOSVersionでは正確に取得出来ないため、なるべくこちらの値を参照する
                osMajor = (int)info.MajorVersion;
                osMinor = (int)info.MinorVersion;
            }

            var str = $"-- {os.Platform} {osMajor} {osMinor} {releaseId} build:{build}.{ubr} {os.ServicePack}";
            SLPrintf(Severity.Info, str);
        }

        // モジュール一覧出力
        private static void OutModuleList()
        {
            // ヘッダー作成
            SLPrintf(Severity.Info, "--- CreationDate        LastWriteTime       LastAccessTime      ModulePath Version/Special");

            // 一覧取得
            var modules = Process.GetCurrentProcess().Modules;
            foreach (ProcessModule module in modules)
            {
                var fi = new FileInfo(module.FileName!);
                var vi = module.FileVersionInfo;
                SLPrintf(Severity.Info, $"--- {fi.CreationTime} {fi.LastWriteTime} {fi.LastAccessTime} {module.FileName}    {vi.FileVersion}/{vi.SpecialBuild}");
            }
        }

        private static string GetRegValue(string keyname, string valuename)
        {
#pragma warning disable CA1416 // プラットフォームの互換性を検証
            return Registry.GetValue(keyname, valuename, "")?.ToString() ?? "";
#pragma warning restore CA1416 // プラットフォームの互換性を検証
        }

        // 終了
        public static void Terminate()
        {
            SLPrintf(Severity.Info, "------------- EXIT");

            foreach (var info in LogFileInfos)
            {
                info.Writer?.Close();
            }

            Mutex?.ReleaseMutex();
            Mutex?.Close();
        }

        public void Dispose()
        {
            Terminate();
        }
    }
}
