using DbLib.Defs.DbLib.Defs;
using ImportLib.Models;
using System.IO;

namespace ImportLib.Engines
{
    public interface IImportEngine
    {
        DataType DataType { get; }
        string DataName { get; }
        string ImportFilePath { get; set; }

        public List<ImportFileInfo> TargetImportFiles { get; }

        void UpdateImportFileInfo();

        IEnumerable<ImportResult> Import(DataImportController controller, CancellationToken token);

        bool IsExistFile => TargetImportFiles.Any();

        void SetFolder(string path)
        {
            var filename = Path.GetFileName(ImportFilePath);
            ImportFilePath = Path.Combine(path, filename);
        }

        InterfaceFile GetInterfaceFile();
    }
}