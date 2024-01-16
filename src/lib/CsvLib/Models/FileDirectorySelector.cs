using LogLib;
using Microsoft.Win32;

namespace CsvLib.Models
{
    public static class FileDirectorySelector
    {
        public static string SelectSavePath(string defaultPath = "", string fileName = "", string filter = "CSVファイル (*.csv)|*.csv")
        {
            var dialog = new SaveFileDialog();

            dialog.InitialDirectory = defaultPath;
            dialog.FileName = fileName;
            dialog.Filter = filter;

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }

            return string.Empty;
        }

        public static string SelectLoadFile(string initPath, string filter = "CSVファイル (*.csv)|*.csv")
        {
            var dialog = new OpenFileDialog();

            dialog.InitialDirectory = initPath;
            dialog.Filter = filter;

            if (dialog.ShowDialog() == true)
            {
                Syslog.Debug($"CsvManager SelectLoadFile:{dialog.FileName}");
                return dialog.FileName;
            }

            return string.Empty;
        }
    }
}
