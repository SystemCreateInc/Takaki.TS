
using DbLib.Defs;
using DbLib.Defs.DbLib.Defs;
using DbLib.Extensions;
using System.IO;

namespace ImportLib.Engines
{
    public interface IImportEngine
    {
        DataType DataType { get; }
        string DataName { get; }
        string ImportFilePath { get; set; }
        long? ImportFileSize { get; }
        DateTime? ImportFileLastWriteDateTime { get; }

        void UpdateImportFileInfo();

        Task<ImportResult> ImportAsync(CancellationToken token);

        bool IsExistFile => ImportFileSize != null;

        void SetFolder(string path)
        {
            var filename = Path.GetFileName(ImportFilePath);
            ImportFilePath = Path.Combine(path, filename);
        }

        InterfaceFile GetInterfaceFile();
    }
}