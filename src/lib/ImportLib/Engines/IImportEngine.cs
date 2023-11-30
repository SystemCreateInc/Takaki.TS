
using DbLib.Defs;
using DbLib.Defs.DbLib.Defs;
using DbLib.Extensions;
using ImportLib.Models;
using System.IO;

namespace ImportLib.Engines
{
    public interface IImportEngine
    {
        DataType DataType { get; }
        string DataName { get; }
        string ImportFilePath { get; set; }

        public List<TargetImportFile> _targetImportFiles { get; }

        void UpdateImportFileInfo();

        Task<List<ImportResult>> ImportAsync(CancellationToken token);

        bool IsExistFile => _targetImportFiles.Any();

        void SetFolder(string path)
        {
            var filename = Path.GetFileName(ImportFilePath);
            ImportFilePath = Path.Combine(path, filename);
        }

        InterfaceFile GetInterfaceFile();
    }
}