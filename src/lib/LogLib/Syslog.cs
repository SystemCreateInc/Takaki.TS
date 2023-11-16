using DbLib;
using Microsoft.Extensions.Configuration;
using SyslogCS.Models;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices; // DLL Import
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace LogLib
{
    [StructLayout(LayoutKind.Sequential)]
    struct SLINIT
    {
        [MarshalAs(UnmanagedType.I4)]
        public int expdays;               /*	有効期限				*/
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string logroot;
        [MarshalAs(UnmanagedType.I4)]
        public int trunc;           /*	ログ削除フラグ			*/
    };

    public class Syslog
    {
        public const short SV_EMERG = 0;	/*	致命的				*/
        public const short SV_ALERT = 1;	/*	警戒				*/
        public const short SV_CRIT = 2;     /*	危機的				*/
        public const short SV_ERR = 3;      /*	エラー				*/
        public const short SV_WARN = 4;     /*	警告				*/
        public const short SV_NOTICE = 5;   /*	通知				*/
        public const short SV_INFO = 6;     /*	情報				*/
        public const short SV_DEBUG = 7;    /*	デバッグ			*/

#if DEBUG
        private const string dllname = "Syslog64d.dll";
#else
        private const string dllname = "Syslog64.dll";
#endif

        [DllImport(dllname)]
        static extern void SLInit(ref SLINIT si);

        public static void Init() 
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("common.json", true, true)
                .Build();

            var section = config.GetSection("syslog");

            SLINIT slinit;
            slinit.expdays = int.Parse(section["logExpDays"] ?? "30");
            slinit.logroot = section["root"] ?? "";
            slinit.trunc = 1;

            if (!Path.IsPathRooted(slinit.logroot))
            {
                var modulePath = Assembly.GetEntryAssembly()?.Location;
                var path = Path.GetDirectoryName(modulePath) ?? "";
                slinit.logroot = Path.GetFullPath(slinit.logroot, path);
            }

            SLInit(ref slinit);
            SyslogCS.SLInit(slinit.expdays, slinit.logroot, slinit.trunc);

        }

        public static void SLPrintf(Severity severity, string format) => SyslogCS.SLPrintf(severity, format);

        public static void Debug(string text) => SLPrintf(Severity.Debug, text);

        public static void Info(string text) => SLPrintf(Severity.Info, text);

        public static void Notice(string text) => SLPrintf(Severity.Notice, text);

        public static void Warn(string text) => SLPrintf(Severity.Warn, text);

        public static void Error(string text) => SLPrintf(Severity.Err, text);

        public static void Crit(string text) => SLPrintf(Severity.Crit, text);

        public static void Alert(string text) => SLPrintf(Severity.Alert, text);

        public static void Emerg(string text) => SLPrintf(Severity.Emerg, text);

        public static void SLCopy(string filename) => SyslogCS.SLCopy(new FileInfo(filename),true);
    }
}