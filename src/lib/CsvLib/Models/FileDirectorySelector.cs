using LogLib;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CsvLib.Models
{
    public static class FileDirectorySelector
    {
        public static string SelectSavePath(string defaultPath = "", string fileName = "", string filter = "CSVファイル (*.csv)|*.csv")
        {
            var dialog = new CommonSaveFileDialog();
            dialog.DefaultDirectory = defaultPath;
            dialog.DefaultFileName = fileName;
            dialog.Filters.Add(new CommonFileDialogFilter("CSVファイル", "*.csv"));
            dialog.Filters.Add(new CommonFileDialogFilter("全てのファイル", "*.*"));
            
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
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
